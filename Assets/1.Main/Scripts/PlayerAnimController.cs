using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PlayerAnimController : AnimationController
{
    public enum AnimState
    {
        Idle,
        Run,
        Attack1,
        Attack2,
        Attack3,
        Attack4,
        Skill1,
        Skill2,
        Roll,
        Die,
        Max
    }
    [SerializeField]
    AnimState m_state;
    StringBuilder m_sb = new StringBuilder();
    public AnimState State { get { return m_state; } }

    public void SetState(AnimState state)
    {
        m_state = state;
    }
    public float GetClipLength(AnimState state)
    {
        m_sb.AppendFormat("{0}", state);
        string animName = m_sb.ToString();
        m_sb.Clear();
        return GetClipLength(animName);
    }
    public void Play(AnimState state, bool isBlend = true)
    {
        m_state = state;
        m_sb.AppendFormat("{0}", state);
        Play(m_sb.ToString(), isBlend);
        m_sb.Clear();
    }
}
