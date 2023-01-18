using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButtonManager : SingletonMonoBehaviour<ActionButtonManager>
{
    public enum ButtonType
    {
        Main,
        Skill1,
        Skill2,
        Skill3,
        Max
    }
    ActionButton[] m_buttons;

    
    public void SetButton(ButtonType type, float coolTime, ButtonDelegate pressDel, ButtonDelegate releaseDel)
    {
        m_buttons[(int)type].SetButton(coolTime, pressDel, releaseDel);
    }
    public void OnPressButton(ButtonType type)
    {
        m_buttons[(int)type].OnPressButton();
    }
    public void OnRelreasseButton(ButtonType type)
    {
        m_buttons[(int)type].OnReleaseButton();
    }
    // Start is called before the first frame update
    protected override void OnAwake()
    {
        m_buttons = GetComponentsInChildren<ActionButton>();
    }
}
