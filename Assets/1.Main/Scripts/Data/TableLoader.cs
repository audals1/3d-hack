using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TableLoader : Singleton<TableLoader>
{
    List<Dictionary<string, string>> m_table = new List<Dictionary<string, string>>(); // string 2개(행번호,값) 값들로 이루어진게 여러개인 리스트만 인스턴스
    public int Length { get { return m_table.Count; } }

    public string GetString(string key, int index)
    {
        return m_table[index][key]; //예) [1],[id] 1번행의 id 값을 출력
    }
    public int GetByte(string key, int index)
    {
        return byte.Parse(GetString(key, index));
    }
    public int GetInteger(string key, int index)
    {
        return int.Parse(GetString(key, index));
    }
    public float GetFloat(string key, int index)
    {
        return float.Parse(GetString(key, index));
    }
    public bool GetBool(string key, int index)
    {
        return bool.Parse(GetString(key, index));
    }
    public void Load(string tableName)
    {
        var data = Resources.Load<TextAsset>("ExcelData/" + tableName);
        MemoryStream ms = new MemoryStream(data.bytes);
        BinaryReader br = new BinaryReader(ms);
        int rowCount = br.ReadInt32();
        int colCount = br.ReadInt32();
        var strDatas = br.ReadString();
        var datas = strDatas.Split('\t');
        List<string> listKey = new List<string>(); //0번째 행(항목명들)의 리스트를 key값으로 저장할 리스트 생성
        m_table.Clear();
        int offset = 1; // 현재위치 변수
        for (int i = 0; i < rowCount; i++)
        {
            offset++;
            if(i == 0) //항목명 key값으로 사용
            {
                for (int j = 0; j < colCount - 1; j++)
                {
                    listKey.Add(datas[offset]);
                    offset++;
                }
            }
            else
            {
                Dictionary<string, string> dicData = new Dictionary<string, string>(); //실제 행 데이터 인스턴스
                for(int j = 0; j < colCount - 1; j++)
                {
                    dicData.Add(listKey[j], datas[offset]);
                    offset++;
                }
                m_table.Add(dicData); //한 행별 데이터들을 table list 인스턴스한거에 저장
            }
        }
        br.Close();
        ms.Close();

    }
    public void Clear()
    {
        m_table.Clear();
    }
}
