using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// by 명민, 몬스터AI
/// 기본상태에서 플레이어와의 거리, 시간경과에 따라 몬스터 행동을 결정
/// </summary>

public class MonsterController : MonoBehaviour
{

    #region Filed
    public enum MonsterState
    {
        Idle,
        Attack,
        Patrol,
        Chase,
        Debuff,
        Damaged,
        Hit,
        Die,
        Max
    }
    [SerializeField]
    protected Collider m_collider;
    [SerializeField]
    protected MonsterState m_state;//몬스터상태값
    protected Status m_status;
    [SerializeField]
    protected PlayerController m_player;//플레이어 객체 가져옴
    [SerializeField]
    PathController m_path; //path 가져옴
    [SerializeField]
    protected float m_attackDist = 1.5f; //공격가능거리
    protected float m_sqrAttackDist;// 공격가능거리 제곱
    protected float m_detectDist = 10f; // 인식범위
    protected float m_sqrDetectDist;
    [SerializeField]
    protected float m_idleDuration = 4f;//idle 대기시간
    protected float m_idleTime;//프레임단위 흐르는 시간
    float m_dieDuration = 2f; //시체상태 대기시간
    float m_dieTime;//시체상태 시간체크
    float m_debuffTime; //디버프 시간체크
    float m_debuffDration; //디버프 대기시간
    int m_curWayPoint; //현재 waypoint
    bool m_isPatrol; //patrol 판단
    public Status MyStatus { get { return m_status; } set { m_status = value; }  }
    protected MonsterAnimController m_monAnimCtr;
    protected NavMeshAgent m_navAgent;
    MoveTween m_moveTween;
    [SerializeField]
    protected HudController m_hudCtr;
    protected Renderer[] m_renderer; //Material 제어
    protected MaterialPropertyBlock m_mpBlock;
    public MonsterManager.MonsterType Type { get; set; }
    public MonsterAnimController.AnimState AnimState { get { return m_monAnimCtr.State; } }
    public bool IsDie { get { return m_state == MonsterState.Die; } }
    #endregion
    #region AnimEvent Methods
    void AnimEvent_Attack()
    {
        NavMeshHit hit;
        var dir = (m_player.transform.position - transform.position);
        float damage = 0f;
        //거리체크
        if (Mathf.Approximately(dir.sqrMagnitude, m_sqrAttackDist) || dir.sqrMagnitude < m_sqrAttackDist)
        {
            //장애물체크
            if (!m_navAgent.Raycast(m_player.transform.position, out hit)) //장애물이 없으면
            {
                var dot = Vector3.Dot(transform.forward, dir.normalized);
                if(dot <= 0.5f)
                {
                    if (m_player.m_isDodge == true || m_player.m_isDie) return;
                    var attackType = AttackProcess(m_player, out damage);
                    m_player.SetDamage(attackType, damage);

                    if (attackType == AttackType.Miss) return;

                    var effect = EffectPool.Instance.Create(TableEffect.Instance.GetData(1).m_prefab[0]);
                    var dummy = transform.position + Vector3.up * 1f;
                    dir.y = 0f;
                    effect.transform.position = dummy;
                    effect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, dir.normalized);
                }
            }
        }
    }
    void AnimEvent_AttackFinished()
    {
        SetState(MonsterState.Idle);
        m_monAnimCtr.Play(MonsterAnimController.AnimState.Idle);
        SetIdleDuration(1f);//난이도조절

    }
    #endregion
    #region Methods
    AttackType AttackProcess(PlayerController player, out float damage)
    {
        AttackType type = AttackType.Miss;
        if (DamageCal.AttackDecision(MyStatus.m_hitRate, player.MyStatus.m_dodgeRate))
        {
            type = AttackType.Normal;
            damage = DamageCal.NormalDamage(MyStatus.m_attack, 0f, player.MyStatus.m_defence);
            if (DamageCal.CriticalDecision(MyStatus.m_criRate))
            {
                type = AttackType.Critical;
                damage = DamageCal.CriticalDamage(damage, MyStatus.m_criAttack);
            }
        }
        else
        {
            damage = 0f;
        }
        return type;
    }
    private void OnParticleCollision(GameObject other)
    {
        
    }
    protected virtual void SetState(MonsterState state)
    {
        m_state = state;
    }
    protected virtual void SetIdleDuration(float time) //매개변수값에 따라 대기상태 길이조절
    {
        m_idleTime = m_idleDuration - time;
    }
    protected virtual void SetIdle(float time)
    {
        m_isPatrol = false;
        SetState(MonsterState.Idle);
        SetIdleDuration(time);
        m_monAnimCtr.Play(MonsterAnimController.AnimState.Idle);
    }
    protected IEnumerator SetColor(Color color, float duration) //맞았을때만 RimLight 적용
    {
        for (int i = 0; i < m_renderer.Length; i++)
        {
            m_mpBlock.SetColor("_RimColor", color);
            m_renderer[i].SetPropertyBlock(m_mpBlock);
        }
        yield return new WaitForSeconds(duration);
        for (int i = 0; i < m_renderer.Length; i++)
        {
            m_mpBlock.SetColor("_RimColor", Color.black);
            m_renderer[i].SetPropertyBlock(m_mpBlock);
        }
    }
    protected virtual void InitStatus()
    {
        m_status = new Status(60, 60f, 10f, 5f, 15f, 5, 50f);
    }
    IEnumerator Coroutine_SerchTarget()
    {
        while(m_state == MonsterState.Chase)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return null;
            }
            m_navAgent.SetDestination(m_player.transform.position);
        }
    }
   protected bool CheckArea(Vector3 target, float area) //공격가능거리 도달 체크
    {
        var dist = target - transform.position;
        if(Mathf.Approximately(dist.sqrMagnitude, area) || dist.sqrMagnitude < area)
        {
            return true;
        }
        return false;
    }
    protected bool FindTarget(Vector3 target)
    {
        Vector3 start = transform.position + Vector3.up * 1f;
        Vector3 end = target + Vector3.up * 1f;
        var dir = target - transform.position;
        RaycastHit hit;
        
        if(Physics.Raycast(start, dir.normalized, out hit, m_detectDist, 1 << LayerMask.NameToLayer("BackGround") | 1 << LayerMask.NameToLayer("Player")))
        {
            if (hit.collider.CompareTag("Player")) //Player 맞았으면 트루반환
                return true;
                //result++;
        }
        /*var dot = Vector3.Dot(transform.forward, dir.normalized);//내적계산 내 기준 앞에있냐 뒤에 있냐 앞 양수 뒤 음수
        if(dot > 0.5f)
        {
            result++;
        }
        if (result == 2) return true;*/
        return false; //배경 맞은경우
    }
    public virtual void InitMonster(MonsterManager.MonsterType type)
    {
        m_attackDist = 1.5f;
        m_detectDist = 10f;
        m_sqrAttackDist = Mathf.Pow(m_attackDist, 2f);
        m_sqrDetectDist = Mathf.Pow(m_detectDist, 2f);
        Type = type;
        InitStatus();
    }
    public void InitHud(Camera uicamera)
    {
        m_hudCtr.InitHud(Camera.main, uicamera);
    }
    public void ResetHud()
    {
        m_hudCtr.gameObject.SetActive(false);
        m_hudCtr.transform.SetParent(transform);
        m_hudCtr.transform.position = Vector3.zero;
        m_hudCtr.transform.localScale = Vector3.one;
    }
    public void SetHud(Transform hudPool)
    {
        m_hudCtr.transform.SetParent(hudPool);
        m_hudCtr.transform.position = Vector3.zero;
        m_hudCtr.transform.localScale = Vector3.one;
        m_hudCtr.transform.localRotation = Quaternion.identity;
        m_hudCtr.gameObject.SetActive(true);
        m_hudCtr.SetHud();
        m_hudCtr.HidHud();
    }
    public void SetTarget(PlayerController player)
    {
        m_player = player;
    }
    public void SetMonster(PathController path)
    {
        m_path = path;
        m_status.m_hp = m_status.m_hpMax;
        //m_collider.enabled = true;
        

    }
    public virtual void SetDamage(AttackType attackType, float damage, SkillData skilldata)
    {
        if (IsDie) return;

        m_hudCtr.ShowHud();
        m_status.m_hp -= Mathf.CeilToInt(damage);
        m_hudCtr.DisplayDamage(attackType, damage, m_status.m_hp / (float)m_status.m_hpMax);
        if (attackType == AttackType.Miss) return;
        if(gameObject.activeInHierarchy)
        {
            m_navAgent.ResetPath();
            m_navAgent.isStopped = true;
            StartCoroutine(SetColor(Color.red, 0.3f));
        }
        if (skilldata.m_debuff == DebuffType.None && m_state != MonsterState.Debuff) //통상 데미지일때
        {
            SetState(MonsterState.Damaged);
            m_monAnimCtr.Play(MonsterAnimController.AnimState.Hit, false);
        }
        if(skilldata.m_debuff != DebuffType.None) // 디버프 있는 데미지 일때
        {
            SetState(MonsterState.Debuff);
            m_debuffDration = skilldata.m_debuffDration;
            m_debuffTime = 0f;
            var animState = GetDebuffAnimation(skilldata.m_debuff);
            m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            if(animState != MonsterAnimController.AnimState.None)
                m_monAnimCtr.Play(animState);
        }
        var dir = transform.position - m_player.transform.position;
        m_moveTween.m_from = transform.position;
        m_moveTween.m_to = m_moveTween.m_from + dir.normalized * skilldata.m_knockbackDist;
        m_moveTween.m_duration = SkillData.MaxKnockbackDuration * (skilldata.m_knockbackDist / SkillData.MaxKnockbackDist);
        m_moveTween.Play();
        if (m_status.m_hp <= 0)
        {
            
            SetState(MonsterState.Die);
            m_monAnimCtr.Play(MonsterAnimController.AnimState.Die);
            m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            //m_collider.enabled = false;
            m_player._DetectArea.UnitList.Remove(gameObject);
        }
    }

    MonsterAnimController.AnimState GetDebuffAnimation(DebuffType type)
    {
        switch (type)
        {
            case DebuffType.Knockdown:
                return MonsterAnimController.AnimState.Knockdown;
            case DebuffType.Stun:
                return MonsterAnimController.AnimState.Stun;
        }
        return MonsterAnimController.AnimState.None;
    }
    protected virtual void BehaviourProcess()
    {
        switch(m_state)
        {
            case MonsterState.Idle:
                m_idleTime += Time.deltaTime;
                if(m_idleTime >= m_idleDuration)
                {
                    if (FindTarget(m_player.transform.position)) //플레이어 인식 검사
                    {
                         if (CheckArea(m_player.transform.position, m_sqrAttackDist)) // 공격가능거리 도달
                         {
                            var dir = m_player.transform.position - transform.position;
                            dir.y = 0f;
                            transform.forward = dir;
                            SetState(MonsterState.Attack);
                            m_monAnimCtr.Play(MonsterAnimController.AnimState.Attack1);
                            return;
                         }
                            SetState(MonsterState.Chase);
                            m_navAgent.isStopped = false;
                            m_monAnimCtr.Play(MonsterAnimController.AnimState.Run);
                            StartCoroutine("Coroutine_SerchTarget");
                            m_navAgent.stoppingDistance = m_attackDist;
                    }
                    else //거리에 플레이어 없음
                    {
                        SetState(MonsterState.Patrol);
                        m_navAgent.isStopped = false;
                        m_monAnimCtr.Play(MonsterAnimController.AnimState.Run);
                        m_navAgent.stoppingDistance = m_navAgent.radius;
                    }
                    m_idleTime = 0f;
                }
                break;
            case MonsterState.Debuff:
                m_debuffTime += Time.deltaTime;
                if(m_debuffTime > m_debuffDration)
                {
                    m_debuffTime = 0f;
                    m_navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
                    SetState(MonsterState.Idle);
                    m_monAnimCtr.Play(MonsterAnimController.AnimState.Idle);
                }
                break;
            case MonsterState.Chase:
                if (FindTarget(m_player.transform.position))
                {
                    if (CheckArea(m_player.transform.position, m_sqrAttackDist))
                    {
                        SetIdle(1f);
                    }
                }
                else
                {
                    SetIdle(0f);
                }
                break;
            case MonsterState.Patrol:
                if (!FindTarget(m_player.transform.position)) //주인공 못찾으면
                {
                    if (!m_isPatrol) //patrol중이 아닐때
                    {
                        m_navAgent.SetDestination(m_path.m_waypoints[m_curWayPoint].transform.position); //waypoint 위치로 이동
                        m_isPatrol = true;
                    }
                    else //waypoint로 이동중일때
                    {
                        if (CheckArea(m_path.m_waypoints[m_curWayPoint].transform.position, Mathf.Pow(m_navAgent.radius, 3f))) //waypoint 도착검사
                        {
                            
                            m_curWayPoint++;
                            if (m_curWayPoint > m_path.m_waypoints.Length - 1)
                                m_curWayPoint = 0;
                            SetIdle(1f);
                        }
                    }
                }
                else
                {
                    SetIdle(0f);
                }
                break;
            case MonsterState.Die:
                m_dieTime += Time.deltaTime;
                if(m_dieTime >= m_dieDuration)
                {
                    MonsterManager.Instance.RemoveMonster(this);
                    m_dieTime = 0f;
                    SetIdle(3f);
                }
                break;
        }
    }
    #endregion
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        m_monAnimCtr = GetComponent<MonsterAnimController>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_moveTween = GetComponent<MoveTween>();
        m_renderer = GetComponentsInChildren<Renderer>();
        m_mpBlock = new MaterialPropertyBlock();
        

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        BehaviourProcess();
    }
}
