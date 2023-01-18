using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterManager : SingletonMonoBehaviour<MonsterManager>
{
    public enum MonsterType
    {
        GoblinMale,
        GoblinFemale,
        GoblinWarriorMale,
        GoblinWarriorFemale,
        GoblinWarChief,
        GoblinShaman,
        Max
    }
    public int m_killCount;
    public int m_count;
    bool m_isSpawn = false;
    public bool IsSpawn { get { return m_isSpawn; } }
    int m_spawnNum = 5;
    public int SpawnNum { get { return m_spawnNum; } }
    [SerializeField]
    PlayerController m_player;
    [SerializeField]
    GameObject m_spawnPointObj;
    SpawnPoint[] m_spawnPoints;
    [SerializeField]
    GameObject[] m_monsterPrefabs;
    [SerializeField]
    Camera m_uiCamera;
    [SerializeField]
    Transform m_hudPoolTransform;
    Dictionary<MonsterType, GameObjectPool<MonsterController>> m_monsterPool = new Dictionary<MonsterType, GameObjectPool<MonsterController>>();
    public void RemoveMonster(MonsterController mon)
    {
        mon.gameObject.SetActive(false);
        mon.ResetHud();
        m_monsterPool[mon.Type].Set(mon);
        m_killCount++;
    }
    public void CreateMonsters(PathController path, SpawnPoint spawnPoint)
    {
        if (!m_isSpawn)
        {
            for (int i = 0; i < 2; i++)
            {
                
                    var mon = m_monsterPool[Random.Range(1, 101) <= 50 ? MonsterType.GoblinMale : MonsterType.GoblinFemale].Get();
                    mon.SetMonster(path);
                    mon.SetHud(m_hudPoolTransform);
                    mon.transform.position = spawnPoint.transform.position;
                    mon.transform.forward = spawnPoint.transform.forward;
                    mon.gameObject.SetActive(true);
                    m_count++;
                    //m_isSpawn = true;
                    //m_spawnPoints[i].IsReady = false;
                
            }
        }
    }
    // Start is called before the first frame update
    protected override void OnStart()
    {
        //m_spawnPoints = m_spawnPointObj.GetComponentsInChildren<SpawnPoint>();
        m_monsterPrefabs = Resources.LoadAll<GameObject>("Prefab/Monsters/");
        for (int i = 0; i < m_monsterPrefabs.Length; i++)
        {
            var prefab = m_monsterPrefabs[i];
            MonsterType type = (MonsterType)int.Parse(prefab.name.Split('.')[0]) - 1;
            GameObjectPool<MonsterController> pool = new GameObjectPool<MonsterController>(3, () =>
            {
                var obj = Instantiate(prefab);
                obj.transform.SetParent(transform);
                obj.transform.localPosition = Vector3.zero;
                var mon = obj.GetComponent<MonsterController>();
                mon.InitMonster(type);
                mon.SetTarget(m_player);
                mon.InitHud(m_uiCamera);
                obj.SetActive(false);
                return mon;
            });
            m_monsterPool.Add(type, pool);
        }
    }
}
