using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class ActionButton : MonoBehaviour
{
    [SerializeField]
    ActionButtonManager.ButtonType m_type;
    [SerializeField]
    UISprite m_coolGauge;
    [SerializeField]
    UISprite m_darkGauge;
    [SerializeField]
    UILabel m_remainTimeLable;
    float m_coolTime;
    float m_time;
    bool IsReady { get; set; }
    public bool IsPress { get; set; }
    ButtonDelegate m_pressDel;
    ButtonDelegate m_releaseDel;
    StringBuilder m_sb;

    public void SetButton(float coolTime, ButtonDelegate pressDel, ButtonDelegate releaseDel)
    {
        m_coolTime = coolTime;
        m_pressDel = pressDel;
        m_releaseDel = releaseDel;
        IsReady = true;
        if (coolTime <= 0f)
        {
            m_coolGauge.gameObject.SetActive(false);
            
        }
        else
        {
            
            m_coolGauge.fillAmount = 1f;
            m_darkGauge.fillAmount = 0f;
            m_remainTimeLable.text = string.Empty;
        }
        
    }
    public void OnPressButton()
    {
        if(!IsReady)
        {
            return;
        }
        IsPress = true;
        if(m_pressDel != null)
        {
            m_pressDel();
            if(m_coolTime > 0f)
            {
                IsReady = false;
            }
        }
    }
    public void OnReleaseButton()
    {
        if (!IsPress) return;
        IsPress = false;
        if(m_releaseDel != null)
        {
            m_releaseDel();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        m_sb = new StringBuilder();
        SetButton(10, null, null);
    }
    private void Update()
    {
        if(!IsReady)
        {
            m_time += Time.deltaTime;
            m_coolGauge.fillAmount = m_time / m_coolTime;
            m_darkGauge.fillAmount = 1 - m_time / m_coolTime;
            m_remainTimeLable.text = m_sb.Append(Mathf.CeilToInt(m_coolTime - m_time)).ToString();
            m_sb.Clear();
            if (m_time > m_coolTime)
            {
                IsReady = true;
                m_coolGauge.fillAmount = 1f;
                m_darkGauge.fillAmount = 0f;
                m_remainTimeLable.text = string.Empty;
                m_time = 0f;
            }
        }
    }
}
