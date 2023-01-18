using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicController : MonoBehaviour
{
    Vector3 m_targetPos;
    Vector3 m_dir = Vector3.forward;
    float m_speed = 3f;
    [SerializeField]
    PlayerController m_player;
    public MagicManager.MagicType Type { get; set; }
    public void SetTarget(PlayerController player)
    {
        m_player = player;
    }
    Vector3 GetTargetDir()
    {
        var dir = m_player.transform.position - transform.position;
        dir.y = 0f;
        return dir.normalized;
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            m_player.SetDamage(AttackType.Normal, 10f);
            MagicManager.Instance.RemoveMagic(this);
            
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var dir = GetTargetDir();
        transform.position += dir * m_speed * Time.deltaTime;
    }
}
