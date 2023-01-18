using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAreaUnitFind : MonoBehaviour
{
    List<GameObject> m_unitList = new List<GameObject>();
    public List<GameObject> UnitList { get { return m_unitList; } }
    public GameObject GetMonsterNearest(Vector3 target) //가장 가까운 몬스터를 리스트에서 찾음
    {
        if (m_unitList.Count == 0) return null;

        float maxDist = (target - m_unitList[0].transform.position).sqrMagnitude;
        int index = 0;
        for (int i = 1; i < m_unitList.Count; i++)
        {
            var dist = (target - m_unitList[i].transform.position).sqrMagnitude;
            if(maxDist < dist)
            {
                maxDist = dist;
                index = i;
            }
        }
        return m_unitList[index];
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Monster"))
        {
            m_unitList.Add(other.gameObject);
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            m_unitList.Remove(other.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
