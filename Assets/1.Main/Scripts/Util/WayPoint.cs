using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WayPoint : MonoBehaviour
{
    [SerializeField]
    Color m_color;
    public void SetColor(Color color)
    {
        m_color = color;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = m_color;
        Gizmos.DrawWireSphere(transform.position, 1f);    
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
