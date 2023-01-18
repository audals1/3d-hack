using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableEffect : Singleton<TableEffect>
{
    public class Data
    {
        public int m_id;
        public string m_dummy;
        public string[] m_prefab = new string[4];
    }
    public Dictionary<int, Data> m_dirData = new Dictionary<int, Data>(); // id값 기준으로 다른 데이터 접근
    public Data GetData(int id)
    {
        return m_dirData[id];
    }

    public void Load()
    {
        TableLoader.Instance.Load("Effect");
        m_dirData.Clear();
        for (int i = 0; i < TableLoader.Instance.Length; i++)
        {
            Data data = new Data();
            data.m_id = TableLoader.Instance.GetInteger("Id", i);
            data.m_dummy = TableLoader.Instance.GetString("Dummy", i);
            for(int j = 0; j < 4; j++)
            {
                data.m_prefab[j] = TableLoader.Instance.GetString("Prefab_" + (j + 1), i);
            }
            m_dirData.Add(data.m_id, data);
        }
        TableLoader.Instance.Clear();
    }
}
