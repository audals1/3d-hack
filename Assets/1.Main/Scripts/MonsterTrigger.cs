using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterTrigger : MonoBehaviour
{
    [SerializeField]
    MonsterManager m_monManager;
    [SerializeField]
    SpawnPoint m_spawnPoint;
    [SerializeField]
    PathController m_path;
    public float m_time;
    float m_interval = 5f;
    public int m_count;
    public int m_maxCount = 10;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(m_count < m_maxCount)
            {
                InvokeRepeating("SpawnMonsters", 5f, m_interval);
            }
        }
    }
    void SpawnMonsters()
    {
        if(m_count < m_maxCount)
        {
            m_monManager.CreateMonsters(m_path, m_spawnPoint);
            m_count += m_monManager.SpawnNum;
        }   
    }
}
