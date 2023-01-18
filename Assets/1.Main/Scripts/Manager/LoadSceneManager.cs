using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneManager : DontDestroy<LoadSceneManager>
{
    public enum SceneState
    {
        None = -1,
        Lobby,
        Title,
        Loading,
        Game
    }

    [SerializeField]
    UILabel m_progressLable;
    [SerializeField]
    UIProgressBar m_loadingprogress;
    AsyncOperation m_loadinginfo;
    SceneState m_scene;
    SceneState m_loadScene = SceneState.None;
    public SceneState Scene { get { return m_scene; } }
    public void LoadSceneAsync(string scene)
    {
       // if (m_loadScene != SceneState.None) return;
       // m_loadScene = scene;
        m_loadinginfo = SceneManager.LoadSceneAsync((scene));
        gameObject.SetActive(true);
    }

    protected override void OnStart()
    {
        gameObject.SetActive(false);
    }

    void HideUI()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_loadinginfo != null)
        {
            if (m_loadinginfo.isDone)
            {
                m_loadinginfo.allowSceneActivation = false;
                m_scene = m_loadScene;
                m_loadScene = SceneState.None;
                m_loadinginfo = null;
                m_loadingprogress.value = 1f;
                m_progressLable.text = "100%";
                //gameObject.SetActive(false);
                Invoke("HideUI", 0.5f);
                //m_loadinginfo.allowSceneActivation = true;

            }
            else
            {
                m_loadingprogress.value = m_loadinginfo.progress;
                m_progressLable.text = Mathf.RoundToInt(m_loadinginfo.progress * 100f).ToString();
                
            }
        }
    }
}
