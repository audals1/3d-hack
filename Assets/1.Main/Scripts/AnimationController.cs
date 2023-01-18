using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    Animator m_animator;
    string m_pervAniName = string.Empty;
    Dictionary<string, float> m_clipsLength = new Dictionary<string, float>();
    public void SetRootMotion(bool isActive)
    {
        m_animator.applyRootMotion = isActive;
    }
    public void Play(string animName, bool isBlend = true)
    {
        if(!string.IsNullOrEmpty(m_pervAniName))
        {
            m_animator.ResetTrigger(m_pervAniName);
            m_pervAniName = string.Empty;
        }
        if(isBlend)
        {
            m_animator.SetTrigger(animName);
        }
        else
        {
            m_animator.Play(animName, 0, 0f);
        }
        m_pervAniName = animName;
    }
    public float GetClipLength(string animName)
    {
        if (m_clipsLength.ContainsKey(animName))
            return m_clipsLength[animName];
        return -1f;
    }
    void Start()
    {
        var ac = m_animator.runtimeAnimatorController;
        for(int i = 0; i < ac.animationClips.Length; i++)
        {
            if(!m_clipsLength.ContainsKey(ac.animationClips[i].name))
            {
                m_clipsLength.Add(ac.animationClips[i].name, ac.animationClips[i].length);
            }
        }
    }
    void Awake()
    {
        m_animator = GetComponent<Animator>();
    }
}
