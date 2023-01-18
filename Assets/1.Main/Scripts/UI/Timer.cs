using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Timer : SingletonMonoBehaviour<Timer>
{
    [SerializeField]
    UILabel m_timer;
    public StringBuilder m_sb;
    public float m_sec;
    public int m_min;
    public bool m_isPause;
    public void CheckTime()
    {
        m_sec += Time.deltaTime;
        m_sb.AppendFormat("{1:00}:{0:00}", (int)m_sec, m_min);
        m_timer.text = m_sb.ToString();
        m_sb.Clear();
        if ((int)m_sec > 59)
        {
            m_sec = 0f;
            m_min++;
        }
    }
    public void SetPause()
    {
        Time.timeScale = Time.timeScale != 0 ? 0f : 1f;
        if (Time.timeScale == 0)
        {
            m_isPause = true;
        }
        else
            m_isPause = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_sb = new StringBuilder();
    }
    void Update()
    {
        CheckTime();
    }
}
