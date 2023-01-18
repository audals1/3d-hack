using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    float m_duration = 1f;
    [SerializeField]
    float m_power = 0.1f;
    Vector3 m_orgPos;
    IEnumerator Coroutin_Shake()
    {
        float time = 0f;
        while(true)
        {
            time += Time.deltaTime;
            Vector3 dir = Random.insideUnitCircle * m_power;
            dir.z = m_orgPos.z;
            transform.localPosition = dir;
            if(time > m_duration)
            {
                transform.position = m_orgPos;
                yield break;
            }
            yield return null;
        }

    }
    public void PlayShake()
    {
        StopAllCoroutines();
        StartCoroutine("Coroutin_Shake");
    }
    public void PlayShake(float duration, float power = 0.1f)
    {
        m_duration = duration;
        m_power = power;
        PlayShake();
    }
    // Start is called before the first frame update
    void Start()
    {
        m_orgPos = transform.position;
    }
}
