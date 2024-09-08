using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    #region Filed
    Vector3 m_dir;
    Vector3 m_clickDir;
    Vector3 m_targetPos;
    [SerializeField]
    float m_speed = 2f;
    int m_comboIndex;
    bool m_isCombo;
    bool m_pressAttack;
    bool m_isDrag;
    bool m_pressSkill;
    public bool m_isDodge;
    public bool m_isDie;
    float m_deltaTime = 0.0166f;

    [SerializeField]
    HUDText m_hudText;
    [SerializeField]
    GameObject m_attackAreaObj; //공격영역 오브젝트 불러옴
    AttackAreaUnitFind[] m_attackArea; //공격영역 충돌체크
    [SerializeField]
    AttackAreaUnitFind m_detectArea; //공격타겟 인식영역
    public AttackAreaUnitFind _DetectArea => m_detectArea;
    CharacterController m_charCtr;
    PlayerAnimController m_animCtr;
    Animator m_animator;
    [SerializeField]
    TrailRenderer m_trail;
    NavMeshAgent m_navAgent;
    NavMeshPath m_navPath;
    LineRenderer m_linePath;
    MoveTween m_moveTween;
    [SerializeField]
    HudController m_hudCtr;
    Renderer[] m_renderer; //Material 제어
    MaterialPropertyBlock m_mpBlock;
    GameObject m_target;
    Status m_status;
    public PlayerAnimController.AnimState AnimState { get { return m_animCtr.State; } }
    public Status MyStatus { get { return m_status; } set { m_status = value; } }
    //스킬별로 공격영역, 넉백거리 얻어옴
    Dictionary<PlayerAnimController.AnimState, SkillData> m_skillTable = new Dictionary<PlayerAnimController.AnimState, SkillData>();
    List<PlayerAnimController.AnimState> m_comboList = new List<PlayerAnimController.AnimState>() { PlayerAnimController.AnimState.Attack1, PlayerAnimController.AnimState.Attack2, PlayerAnimController.AnimState.Attack3, PlayerAnimController.AnimState.Attack4};
    Queue<KeyCode> m_keyBuffer = new Queue<KeyCode>();
    #endregion

    #region Methods
    bool IsAttack()
    {
       if(AnimState == PlayerAnimController.AnimState.Attack1 ||
       AnimState == PlayerAnimController.AnimState.Attack2 ||
       AnimState == PlayerAnimController.AnimState.Attack3 ||
       AnimState == PlayerAnimController.AnimState.Attack4 ||
       AnimState == PlayerAnimController.AnimState.Skill1 ||
       AnimState == PlayerAnimController.AnimState.Skill2 ||
       AnimState == PlayerAnimController.AnimState.Roll)
            return true;
        return false;
    }
    void SetLocomotion(bool isAttack = false)
    {
        if (m_dir != Vector3.zero && !IsAttack()) //이동키 누른 상태이고 공격 안누름
        {
             if (AnimState == PlayerAnimController.AnimState.Idle)
                 m_animCtr.Play(PlayerAnimController.AnimState.Run);
            if(m_clickDir == Vector3.zero)
                transform.forward = m_dir;
        }
        else //이동키 x 공격키 o
        {
            if(isAttack)
            {
                m_animCtr.Play(PlayerAnimController.AnimState.Idle);
                m_clickDir = m_dir = Vector3.zero;
                m_targetPos = Vector3.zero;
            }
            else
            {
                if (AnimState == PlayerAnimController.AnimState.Run)
                    m_animCtr.Play(PlayerAnimController.AnimState.Idle);
            }
        }
    }
    void ResetKeyBuffer()
    {
        m_keyBuffer.Clear();
    }
    Vector3 GetPadDir()
    {
        Vector3 dir = Vector3.zero;
        var tempAxis = MovePad.Instance.GetAxis();
        if(tempAxis.x < 0f)
        {
            dir += Vector3.left * Mathf.Abs(tempAxis.x);
        }
        if(tempAxis.x > 0f)
        {
            dir += Vector3.right * tempAxis.x;
        }
        if(tempAxis.y < 0f)
        {
            dir += Vector3.back * Mathf.Abs(tempAxis.y);
        }
        if(tempAxis.y > 0f)
        {
            dir += Vector3.forward * tempAxis.y;
        }
        return dir;
    }
    public void SetDamage(AttackType attackType, float damage)
    {
        if (m_isDie) return;

        m_status.m_hp -= Mathf.CeilToInt(damage);
        m_hudCtr.DisplayDamage(attackType, damage, m_status.m_hp / (float)m_status.m_hpMax);
        if (attackType == AttackType.Miss) return;
        m_navAgent.ResetPath();
        m_navAgent.isStopped = true;
        StartCoroutine(SetColor(Color.red, 0.3f));
        if (m_status.m_hp <= 0)
        {
            m_isDie = true;
            m_animCtr.Play(PlayerAnimController.AnimState.Die);
            Gameover.Instance.Defeat();
        }
    }
    public void OnPressAttack()
    {
        m_navAgent.ResetPath();
        m_pressAttack = true;
        if (!IsAttack())
        {
            var dir = GetTargetDir();
            if (dir != Vector3.zero)
                transform.forward = dir;
            m_animCtr.Play(PlayerAnimController.AnimState.Attack1);
        }
        else
        {
            if (IsInvoking("ResetKeyBuffer"))
                CancelInvoke("ResetKeyBuffer");
            var tick = (m_animCtr.GetClipLength(m_comboList[m_comboIndex]) - m_deltaTime) * 0.5f;
            Invoke("ResetKeyBuffer", tick);
            m_keyBuffer.Enqueue(KeyCode.Space);
        }

    }
    public void OnReleaseAttack()
    {
        m_pressAttack = false;
    }
    public void OnReleaseSkill()
    {
        m_pressSkill = false;
    }
    public void OnPressSkill1()
    {
        m_navAgent.ResetPath();
        m_pressSkill = true;
        StopAllCoroutines();
        AnimatorClipInfo[] m_aniClips = m_animator.GetCurrentAnimatorClipInfo(0);
        m_trail.gameObject.SetActive(true);
        m_animCtr.Play(PlayerAnimController.AnimState.Skill1);
        StartCoroutine(ProcessTime(m_aniClips[0].clip.length));
    }
    IEnumerator ProcessTime(float m_time)
    {
        yield return new WaitForSeconds(m_time);
        m_trail.gameObject.SetActive(false);

    }
    public void OnPressSkill2()
    {
        m_navAgent.ResetPath();
        m_pressSkill = true;
        m_animCtr.Play(PlayerAnimController.AnimState.Skill2);
    }
    void OnPressDodge()
    {
        m_navAgent.ResetPath();
        m_pressSkill = true;
        m_moveTween.m_from = transform.position;
        m_moveTween.m_to = transform.position + transform.forward.normalized * 3f;
        m_moveTween.m_duration = 0.8f;
        m_moveTween.Play();
        m_animCtr.SetRootMotion(false);
        m_animCtr.Play(PlayerAnimController.AnimState.Roll);
    }
    IEnumerator SetColor(Color color, float duration) //맞았을때만 RimLight 적용
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
    protected virtual void ActionControl()
    {
        m_dir = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        var padDir = GetPadDir();
        if(Input.GetMouseButtonDown(0))
        {
            if (UICamera.hoveredObject != null)
                return;

            m_isDrag = true;
            var pos = GetClickPosition();
            if(pos != null)
            {
                m_clickDir = pos.Value - transform.position;
                m_targetPos = pos.Value;
                m_clickDir.y = 0f;
                m_navAgent.SetDestination(m_targetPos);
            }
            
        }
        if(Input.GetMouseButtonUp(0))
        {
            m_isDrag = false;
            
        }
        if(padDir != Vector3.zero && m_dir == Vector3.zero) //패드터치 눌림 && 키보드 안눌림일때
        {
            m_dir = padDir; //패드터치 방향으로
        }
        if (m_clickDir != Vector3.zero && m_dir == Vector3.zero) // 맵터치(클릭) 눌림 && 키보드 안눌림
        {
            m_dir = m_clickDir.normalized; // 맵터치(클릭) 방향으로
        }
        else if(m_dir != Vector3.zero) // 키보드 눌렸을때
        {
            m_clickDir = Vector3.zero; // 맵터치(클릭) 방향 초기화
            m_navAgent.ResetPath(); //nav 초기화
        }
        if(Input.GetKeyDown(KeyCode.Z))
        {            
            ActionButtonManager.Instance.OnPressButton(ActionButtonManager.ButtonType.Skill1);   
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            ActionButtonManager.Instance.OnPressButton(ActionButtonManager.ButtonType.Skill2);
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            ActionButtonManager.Instance.OnPressButton(ActionButtonManager.ButtonType.Skill3);
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ActionButtonManager.Instance.OnPressButton(ActionButtonManager.ButtonType.Main);
            //OnPressAttack();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnReleaseAttack();
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            OnReleaseSkill();
        }
        if (Input.GetKeyUp(KeyCode.X))
        {
            OnReleaseSkill();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            OnReleaseSkill();
        }
        if (m_clickDir != Vector3.zero)
        {
            var distance = m_targetPos - transform.position;
            var sqrVal = Mathf.Pow(m_navAgent.stoppingDistance , 2f);
            if (Mathf.Approximately(distance.sqrMagnitude, sqrVal ) || distance.sqrMagnitude < sqrVal)
            {
                m_clickDir = m_dir = Vector3.zero;
                m_targetPos = Vector3.zero;
            }
        }
        SetLocomotion();

        //transform.position += m_dir * m_speed * Time.deltaTime;
        //m_charCtr.SimpleMove(m_dir * m_speed);
        if(m_clickDir == Vector3.zero && !IsAttack())
            m_navAgent.Move(m_dir * m_speed * Time.deltaTime);
    }
    Vector3? GetClickPosition()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000f, 1 << LayerMask.NameToLayer("BackGround")))
        {
            return hit.point;
        }
        return null;
    }

    
    Vector3 GetTargetDir()
    {
        m_target = m_detectArea.GetMonsterNearest(transform.position);
        if (m_target == null || m_target.gameObject.activeSelf == false)
        {
            return Vector3.zero;
        }

        var dir = m_target.transform.position - transform.position;
        dir.y = 0f;
        return dir.normalized;
    }
    AttackType AttackProcess(MonsterController mon, SkillData skilldata, out float damage)
    {
        AttackType type = AttackType.Miss;
        if(DamageCal.AttackDecision(MyStatus.m_hitRate, mon.MyStatus.m_dodgeRate))
        {
            type = AttackType.Normal;
            damage = DamageCal.NormalDamage(MyStatus.m_attack, skilldata.m_attack, mon.MyStatus.m_defence);
            if(DamageCal.CriticalDecision(MyStatus.m_criRate))
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
    void InitSkillData() //스킬데이터 초기설정
    {
        m_skillTable.Add(PlayerAnimController.AnimState.Attack1, new SkillData() { m_attackArea = 0, m_knockbackDist = 0.4f, m_effectId = 1, m_debuff = DebuffType.None, m_attack = 5f});
        m_skillTable.Add(PlayerAnimController.AnimState.Attack2, new SkillData() { m_attackArea = 1, m_knockbackDist = 0.6f, m_effectId = 5, m_debuff = DebuffType.None, m_attack = 5f });
        m_skillTable.Add(PlayerAnimController.AnimState.Attack3, new SkillData() { m_attackArea = 2, m_knockbackDist = 0.8f, m_effectId = 3, m_debuff = DebuffType.None, m_attack = 5f });
        m_skillTable.Add(PlayerAnimController.AnimState.Attack4, new SkillData() { m_attackArea = 3, m_knockbackDist = 5f, m_effectId = 2, m_debuff = DebuffType.None, m_attack = 10f });
        m_skillTable.Add(PlayerAnimController.AnimState.Skill1, new SkillData() { m_attackArea = 4, m_knockbackDist = 7f, m_effectId = 13, m_debuff = DebuffType.Knockdown, m_debuffDration = 2f, m_coolTime = 3f, m_attack = 30f });
        m_skillTable.Add(PlayerAnimController.AnimState.Skill2, new SkillData() { m_attackArea = 4, m_knockbackDist = 5f, m_effectId = 15, m_debuff = DebuffType.Knockdown, m_debuffDration = 3f, m_coolTime = 5f, m_attack = 50f });
        m_skillTable.Add(PlayerAnimController.AnimState.Roll, new SkillData() { m_attackArea = 0, m_knockbackDist = 0f, m_effectId = 0, m_debuff = DebuffType.None, m_debuffDration = 0f, m_coolTime = 0.5f });
    }
    void InitStatus()
    {
        m_status = new Status(100, 75f, 5f, 10f, 20f, 25f, 100f);
    }
    #endregion
    #region Animation Event Methods
    public void AnimEvent_AttackFinished()
    {
        m_isCombo = false;
        if (m_pressAttack) m_isCombo = true;
        if (m_keyBuffer.Count > 0)
        {
            if(m_keyBuffer.Count > 1) //연타한 경우
            {
                m_isCombo = false;
                ResetKeyBuffer();
            }
            else
            {
               var key = m_keyBuffer.Dequeue();
                m_isCombo = true;
            }
        }
        if(m_isCombo)
        {
            m_comboIndex++;
            if(m_comboIndex > m_comboList.Count - 1)
            {
                m_comboIndex = 0;
            }
            var dir = GetTargetDir();
            if (dir != Vector3.zero)
                transform.forward = dir;
            m_animCtr.Play(m_comboList[m_comboIndex]);
        }
        else
        {
            m_comboIndex = 0;
            SetLocomotion(true);
        }
    }
    void AnimEvent_SetIdle()
    {
        m_comboIndex = 0;
        m_animCtr.SetRootMotion(true);
        SetLocomotion(true);
    }
    void AnimEvent_Attack()
    {
        SkillData skilldata;
        m_skillTable.TryGetValue(AnimState, out skilldata); //
        float damage = 0f;
        if (skilldata == null) return;
          var unitList = m_attackArea[skilldata.m_attackArea].UnitList;//공격영역별 들어온 유닛리스트들 받아옴
        for (int i = 0; i < unitList.Count; i++)
        {
            var mon = unitList[i].GetComponent<MonsterController>();
            var attackType = AttackProcess(mon, skilldata, out damage);
            mon.SetDamage(attackType,damage,skilldata);

            if (attackType == AttackType.Miss) return;

            var effect = EffectPool.Instance.Create(TableEffect.Instance.GetData(skilldata.m_effectId).m_prefab[attackType == AttackType.Critical ? 1 : 0]);
            var dummy = Util.FindChildObject(mon.gameObject, "Dummy_Hit");
            var dir = transform.position - mon.transform.position;//주인공방향
            dir.y = 0f;
            effect.transform.position = dummy.transform.position;
            effect.transform.rotation = Quaternion.FromToRotation(Vector3.forward, dir.normalized);
            
        }
        for (int i = 0; i < m_attackArea.Length; i++)
        {
            m_attackArea[i].UnitList.RemoveAll(obj => obj.GetComponent<MonsterController>().IsDie);
        }
    }
    void AnimEvent_SwordTrailAttack()
    {
        var data = TableEffect.Instance.GetData(19);
        var projectile = EffectPool.Instance.Create(data.m_prefab[0]);
        var dummy = Util.FindChildObject(gameObject, data.m_dummy);
        projectile.transform.position = gameObject.transform.position + Vector3.up;
        projectile.transform.forward = gameObject.transform.forward;
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        TableEffect.Instance.Load();
        var data = TableEffect.Instance.m_dirData[18];
        m_animator = GetComponent<Animator>();
        m_animCtr = GetComponent<PlayerAnimController>();
        m_charCtr = GetComponent<CharacterController>();
        m_navAgent = GetComponent<NavMeshAgent>();
        m_navPath = new NavMeshPath();
        m_moveTween = GetComponent<MoveTween>();
        m_linePath = GetComponent<LineRenderer>();
        m_attackArea = m_attackAreaObj.GetComponentsInChildren<AttackAreaUnitFind>();
        m_trail = GetComponentInChildren<TrailRenderer>();
        m_trail.gameObject.SetActive(false);
        //m_vfxHitPrefab = Resources.Load("Prefab/Effect/FX_Attack01_01") as GameObject;
        InitStatus();
        InitSkillData();
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Main, 0f, OnPressAttack, OnReleaseAttack);
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Skill1, m_skillTable[PlayerAnimController.AnimState.Skill1].m_coolTime , OnPressSkill1, OnReleaseSkill);
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Skill2, m_skillTable[PlayerAnimController.AnimState.Skill2].m_coolTime, OnPressSkill2, OnReleaseSkill);
        ActionButtonManager.Instance.SetButton(ActionButtonManager.ButtonType.Skill3, m_skillTable[PlayerAnimController.AnimState.Roll].m_coolTime, OnPressDodge, OnReleaseSkill);
        m_renderer = GetComponentsInChildren<Renderer>();
        m_mpBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        ActionControl();
    }
}
