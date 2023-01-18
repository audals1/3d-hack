using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PathController : MonoBehaviour
{
   public WayPoint[] m_waypoints;
    [SerializeField]
    Color m_color = Color.yellow;
    void OnDrawGizmos()
    {
        
        for(int i = 0; i < m_waypoints.Length; i++)
        {
            m_waypoints[i].SetColor(m_color);
        }
        for (int i = 0; i < m_waypoints.Length - 1; i++)
        {
            Gizmos.DrawLine(m_waypoints[i].transform.position, m_waypoints[i + 1].transform.position);
        }    
    }
    // Start is called before the first frame update
    void Awake()
    {
        m_waypoints = GetComponentsInChildren<WayPoint>();//waypoint 객체 가진 자식들 다 불러옴
    }
}
