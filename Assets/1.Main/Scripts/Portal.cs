using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : SingletonMonoBehaviour<Portal>
{
    [SerializeField]
    GameObject m_portalobj;
    [SerializeField]
    PlayerController m_player;
    [SerializeField]
    MonsterManager m_monstermanager;
    [SerializeField]
    Transform m_portalout;
    [SerializeField]
    Transform m_portalPos;
    CapsuleCollider m_collider;
    int m_portalcount = 8;
    bool m_isPortalOn = false;
    public void SetPortal()
    {
        m_portalobj = Resources.Load("Prefab/Portal/FX_Portal_Round") as GameObject;
        m_portalobj = Instantiate(m_portalobj);
        m_portalobj.transform.SetParent(transform);
        m_portalobj.transform.localPosition = Vector3.zero;
        m_portalobj.gameObject.SetActive(false);
        m_collider.enabled = false;
    }
    public void ResetPortal()
    {
        m_portalobj.transform.SetParent(transform);
        m_portalobj.transform.localPosition = Vector3.zero;
        m_portalobj.gameObject.SetActive(false);
        m_collider.enabled = false;
    }
    public IEnumerator Coroutin_PortalCheck()
    {
        if (m_monstermanager.m_killCount >= 0)
        {
            gameObject.transform.position = m_portalPos.transform.position;
            m_portalobj.gameObject.SetActive(true);
            m_collider.enabled = true;
        }
        yield return null;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            m_player.gameObject.SetActive(false);
            m_player.transform.position = m_portalout.transform.position;
            m_player.gameObject.SetActive(true);
            ResetPortal();
        }
    }
    protected override void OnStart()
    {
        m_collider = GetComponent<CapsuleCollider>();
        SetPortal();
    }
    void Update()
    {
        
        if (!m_isPortalOn)
        {
            if (m_monstermanager.m_killCount >= 4)
            {
                gameObject.transform.position = m_portalPos.transform.position;
                m_portalobj.gameObject.SetActive(true);
                m_collider.enabled = true;
                m_isPortalOn = true;
            }
        }

    }
}
