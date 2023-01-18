using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : SingletonMonoBehaviour<EffectPool>
{
    int m_presetSize = 1;
    List<string> m_listEffectName = new List<string>();
    Dictionary<string, GameObjectPool<EffectPoolUnit>> m_dirEffectPool = new Dictionary<string, GameObjectPool<EffectPoolUnit>>();
    void Load()
    {
        TableEffect.Instance.Load();
        foreach(KeyValuePair<int, TableEffect.Data> pair in TableEffect.Instance.m_dirData)
        {
            for (int i = 0; i < pair.Value.m_prefab.Length; i++)
            {
                if (!m_listEffectName.Contains(pair.Value.m_prefab[i])) //리스트 내 프리팹 중복검사
                {
                    m_listEffectName.Add(pair.Value.m_prefab[i]);
                }
            }
        }
        for (int i = 0; i < m_listEffectName.Count; i++)
        {
            EffectPoolUnit poolUnit = null;
            var effectName = m_listEffectName[i];
            var prefab = Resources.Load("Prefab/Effect/" + effectName) as GameObject;
            GameObjectPool<EffectPoolUnit> pool = new GameObjectPool<EffectPoolUnit>();
            m_dirEffectPool.Add(effectName, pool);
            pool.CreatePool(m_presetSize, () =>
            {
                if(prefab != null)
                {
                    var obj = Instantiate(prefab);
                    poolUnit = obj.GetComponent<EffectPoolUnit>();
                    if(poolUnit == null)
                    {
                        poolUnit = obj.AddComponent<EffectPoolUnit>();
                    }
                    var autoDestroy = obj.GetComponent<ParticleAutoDestroy>();
                    if(autoDestroy == null)
                    {
                        autoDestroy = obj.AddComponent<ParticleAutoDestroy>();
                    }
                    poolUnit.SetObjectPool(effectName);
                    if(poolUnit.gameObject.activeSelf)
                    {
                        poolUnit.gameObject.SetActive(false);
                    }
                    else
                    {
                        AddPoolUnit(effectName, poolUnit);
                    }
                }
                return poolUnit;
            });
            
        }
    }
    IEnumerator Coroutine_SetActive(EffectPoolUnit unit, bool isActive)
    {
        yield return new WaitForEndOfFrame();
        unit.gameObject.SetActive(isActive);
    }
    public void AddPoolUnit(string effectName, EffectPoolUnit poolUnit)
    {
        var pool = m_dirEffectPool[effectName];
        if(pool != null)
        {
            pool.Set(poolUnit);
        }
    }
    public GameObject Create(string effectName, Vector3 pos, Quaternion rot)
    {
        EffectPoolUnit unit = null;
        var pool = m_dirEffectPool[effectName];
        if(pool == null)
        {
            return null;
        }
        for (int i = 0; i < pool.Count; i++)
        {
            unit = pool.Get();
            if(!unit.IsReady)
            {
                pool.Set(unit);
                unit = null;
            }
            else
            {
                break;
            }
        }
        if(unit == null)
        {
            unit = pool.New();
        }
        unit.transform.position = pos;
        unit.transform.rotation = rot;
        StartCoroutine(Coroutine_SetActive(unit, true));
        return unit.gameObject;
    }
    public GameObject Create(string effectName)
    {
        return Create(effectName, Vector3.zero, Quaternion.identity);
    }
    protected override void OnAwake()
    {
        Load();
    }
}
