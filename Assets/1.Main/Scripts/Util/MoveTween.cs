using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTween : MonoBehaviour
{
    //캐릭터컨트롤러로 ngui 트윈포지션 구현할때 사용
    public Vector3 m_from;
    public Vector3 m_to;
    public float m_duration = 1f;
    [SerializeField]
    AnimationCurve m_curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
    //CharacterController m_charCtr;
    NavMeshAgent m_navAgent;
    bool m_isStart;
    float m_time;
    public void Play()
    {
        m_time = 0f;
        StopAllCoroutines();
        if(gameObject.activeInHierarchy)
        {
            StartCoroutine("Coroutine_PlayCurve");
        }
        
    }
    IEnumerator Coroutine_PlayCurve()
    {
        while(m_time < 1.0f)
        {
            if (gameObject.activeInHierarchy == true)
            {
                var value = m_curve.Evaluate(m_time);
                var result = m_from * (1f - value) + m_to * value;
                var dir = result - transform.position;
                //m_charCtr.Move(dir);
                m_navAgent.Move(dir);
                m_time += Time.deltaTime / m_duration;
            }   
            yield return null;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //m_charCtr = GetComponent<CharacterController>();
        m_navAgent = GetComponent<NavMeshAgent>();
    }
    //void Update()
    //{
    //    if (m_isStart)
    //    {
    //       var value = m_curve.Evaluate(m_time);
    //       var result = m_from * (1f - value) + m_to * value;
    //        var dir = result - transform.position;
    //        //m_charCtr.Move(dir);
    //        m_navAgent.Move(dir);
    //       m_time += Time.deltaTime / m_duration;
    //        if(m_time >= 1.0f)
    //        {
    //            m_isStart = false;
    //        }
    //    }
    //}

}
