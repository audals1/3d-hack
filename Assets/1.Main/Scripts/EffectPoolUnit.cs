using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPoolUnit : MonoBehaviour
{
    [SerializeField]
    float m_delay = 1f;
    string m_effectName;
    float m_inactiveTime;
    public bool IsReady
    {
        get
        {
            if(!gameObject.activeSelf)
            {
                if (Time.time > m_inactiveTime + m_delay)
                    return true;
            }
            return false;
        }
    }
    public void ResetParent()
    {
        transform.SetParent(EffectPool.Instance.transform);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
    }
    public void SetObjectPool(string effectName)
    {
        m_effectName = effectName;
        ResetParent();
    }
    void OnDisable()
    {
        m_inactiveTime = Time.time;
        EffectPool.Instance.AddPoolUnit(m_effectName, this);        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
