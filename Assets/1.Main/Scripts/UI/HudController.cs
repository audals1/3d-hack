using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudController : MonoBehaviour
{
    [SerializeField]
    UIFollowTarget m_followTarget;
    [SerializeField]
    UIProgressBar m_hpBar;
    HUDText[] m_hudText;
    public void InitHud(Camera gameCamera, Camera uiCamera)
    {
        m_followTarget.gameCamera = gameCamera;
        m_followTarget.uiCamera = uiCamera;
    }
    public void SetHud()
    {
        m_hpBar.value = 1f;
        m_hpBar.alpha = 1f;
    }
    public void ShowHud()
    {
        gameObject.SetActive(true);
        if(IsInvoking("HidHud"))
        {
            CancelInvoke("HidHud");
        }
        Invoke("HidHud", 5f);
    }
    public void HidHud()
    {
        gameObject.SetActive(false);
    }
    public void DisplayDamage(AttackType attackType, float damage, float normalizedHp)
    {
        switch(attackType)
        {
            case AttackType.Miss:
                m_hudText[2].Add("Miss", Color.white, 1f);
                break;
            case AttackType.Normal:
                m_hudText[0].Add(-damage, Color.red, 0f);
                break;
            case AttackType.Critical:
                m_hudText[1].Add(-damage, Color.yellow, 0f);
                break;
        }
        m_hpBar.value = normalizedHp;
        if(normalizedHp <= 0f)
        {
            m_hpBar.alpha = 0f;
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        m_hudText = GetComponentsInChildren<HUDText>();
    }
}
