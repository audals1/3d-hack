using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    public enum DestroyType
    {
        Destroy,
        Inactive,
    }
    public DestroyType m_type = DestroyType.Inactive;
    public float m_lifeTime = 0f;
    float m_time; //파티클 동작 시작 시간 체크
    ParticleSystem[] m_particles;
    void DestroyParticle()
    {
        switch (m_type)
        {
            case DestroyType.Destroy:
                Destroy(gameObject);
                break;
            case DestroyType.Inactive:
                gameObject.SetActive(false);
                break;
        }
    }
    void OnEnable()
    {
        m_time = Time.time;    
    }
    // Start is called before the first frame update
    void Start()
    {
        m_particles = GetComponentsInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_lifeTime > 0f) 
        {
            if(Time.time > m_time + m_lifeTime) // 무한반복 재생인 경우
            {
                DestroyParticle();
            }
        }
        else //재생시간 한정인 경우
        {
            bool isPlaying = false;
            for (int i = 0; i < m_particles.Length; i++)
            {
                if (m_particles[i].isPlaying)
                {
                    isPlaying = true;
                    break;
                }
                if (!isPlaying)
                {
                    DestroyParticle();
                }
            }
        }
    }
}
