using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicManager : SingletonMonoBehaviour<MagicManager>
{
    public enum MagicType
    {
        Magicball,
        Magicrocet,
        Max
    }
    [SerializeField]
    PlayerController m_player;
    [SerializeField]
    GameObject[] m_magicPrefabs;
    [SerializeField]
    MonsterRange m_rangeMonster;
    Dictionary<MagicType, GameObjectPool<MagicController>> m_magicPool = new Dictionary<MagicType, GameObjectPool<MagicController>>();
    // Start is called before the first frame update
    public void CreateMagic1()
    {
        var magic = m_magicPool[MagicType.Magicball].Get();
        var dummy = Util.FindChildObject(m_rangeMonster.gameObject, "Dummy_Fire");
        magic.transform.position = dummy.transform.position;
        magic.transform.forward = dummy.transform.forward;
        magic.gameObject.SetActive(true);
    }
    public void CreateMagic2()
    {
        var magic = m_magicPool[MagicType.Magicrocet].Get();
        var dummy = Util.FindChildObject(m_rangeMonster.gameObject, "Dummy_Fire");
        magic.transform.position = dummy.transform.position;
        magic.transform.forward = dummy.transform.forward;
        magic.gameObject.SetActive(true);
    }
    public void RemoveMagic(MagicController magic)
    {
        magic.gameObject.SetActive(false);
        m_magicPool[magic.Type].Set(magic);
        
    }
    void Start()
    {
        m_magicPrefabs = Resources.LoadAll<GameObject>("Prefab/Magic/");
        for (int i = 0; i < m_magicPrefabs.Length; i++)
        {
            var prefab = m_magicPrefabs[i];
            MagicType type = (MagicType)int.Parse(prefab.name.Split('.')[0]) - 1;
            GameObjectPool<MagicController> pool = new GameObjectPool<MagicController>(3, () =>
            {
                var obj = Instantiate(prefab);
                var magic = obj.GetComponent<MagicController>();
                var dummy = Util.FindChildObject(m_rangeMonster.gameObject, "Dummy_Fire");
                obj.transform.position = dummy.transform.position;
                obj.transform.localPosition = Vector3.zero;
                magic.SetTarget(m_player);
                obj.SetActive(false);
                return magic;
            });
            m_magicPool.Add(type, pool);
        }
    }
}
