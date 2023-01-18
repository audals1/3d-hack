using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class BossManager : MonoBehaviour
{
    [SerializeField]
    GameObject m_boss;
    [SerializeField]
    MonsterManager m_monManager;
    [SerializeField]
    GameObject m_bossUI;
    bool m_isReady = false;
    void HideBossUI()
    {
        m_bossUI.gameObject.SetActive(false);
        
    }
    void SetBoss(Transform boss)
    {
        m_boss.transform.SetParent(boss);
        m_boss.gameObject.SetActive(true);
        m_bossUI.gameObject.SetActive(true);
        
    }
    void CheckBossReady()
    {
    
            if (m_monManager.m_killCount >= 4)
            {
                SetBoss(gameObject.transform);
            }
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        m_boss.gameObject.GetComponent<BossController>();
        HideBossUI();
    }
    void Update()
    {
        CheckBossReady();    
    }
}
