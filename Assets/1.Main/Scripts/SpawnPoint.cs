using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField]
    BoxCollider m_monsterTrigger;
    [SerializeField]
    float m_interval = 5f; //생성인터벌타임
    [SerializeField]
    PathController m_path;
    [SerializeField]
    int m_count;
    int m_maxCount = 10;
    float m_time; // 현 시간 체크
    public bool IsReady { get; set; } //생성 불린변수
    public PathController Path { get { return m_path; } }
    public int Count { get { return m_count; } }
    public int MaxCount { get { return m_maxCount; } }
    // Start is called before the first frame update
    void Start()
    {
      
    }
    public void CheckPoolReady()
    {
        if (!IsReady)
        {
            if (m_maxCount <= m_count)
            {
                IsReady = false;
            }
            else
            {
                if (m_monsterTrigger)
                {
                    IsReady = true;
                    m_count++;
                    m_time = 0f;
                }
            }
        }
    }
}
