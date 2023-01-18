using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

#endif
public delegate void ButtonDelegate();

public class PopupManager : DontDestroy<PopupManager>
{
    [SerializeField]
    GameObject m_popupOKCancelPrefab;
    [SerializeField]
    GameObject m_popupOkPrefab;
    int m_popupDeptgh = 100;
    int m_popupDeptghGap = 10;
    List<GameObject> m_popupList = new List<GameObject>();
    public void OpenPopup_OkCancel(string subject, string body, ButtonDelegate okDel, ButtonDelegate cancelDel, string okBtnText = "OK", string CancelBtnText = "Cancel")
    {
        var obj = Instantiate(m_popupOKCancelPrefab);
        var popup = obj.GetComponent<Popup_OkCancel>();
        var panels = obj.GetComponentsInChildren<UIPanel>();
        for(int i = 0; i < panels.Length; i++)
        {
            panels[i].depth = m_popupDeptgh + m_popupList.Count * m_popupDeptghGap + i;
        }
        popup.SetUI(subject, body, okDel, cancelDel, okBtnText, CancelBtnText);
        m_popupList.Add(obj);
    }
    public void OpenPopup_Ok(string subject, string body, ButtonDelegate okDel, string okBtnText = "OK")
    {
        var obj = Instantiate(m_popupOkPrefab);
        var popup = obj.GetComponent<PopupOK>();
        var panels = obj.GetComponentsInChildren<UIPanel>();
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].depth = m_popupDeptgh + m_popupList.Count * m_popupDeptghGap + i;
        }
        popup.SetUI(subject, body, okDel, okBtnText);
        m_popupList.Add(obj);
    }
    public void ClosePopup()
    {
        if (m_popupList.Count > 0)
        {
            Destroy(m_popupList[m_popupList.Count - 1].gameObject);
            m_popupList.RemoveAt(m_popupList.Count - 1);
        }
      
    }
       
        
    
    protected override void OnStart()
    {
        m_popupOKCancelPrefab = Resources.Load("UI/Popup/Popup_OKcancel") as GameObject;
        m_popupOkPrefab = Resources.Load("UI/Popup/Popup_OK") as GameObject;
    }




    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if (Random.Range(1, 101) % 2 == 0)
                OpenPopup_OkCancel("Notice", "[000000]안녕하세요 [00FF00]sbs[-]게임 아카데미입니다[-]", null, null, "확인", "취소");
            else
                OpenPopup_Ok("Notice", "Hello", null);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_popupList.Count > 0)
            {
                ClosePopup();
            }
            else
            {
                switch (LoadSceneManager.Instance.Scene)
                {
                    case LoadSceneManager.SceneState.Title:
                        OpenPopup_OkCancel("[000000]Notice", "[000000]Are you sure to Exit?", () =>
                        {
#if UNITY_EDITOR            
                            EditorApplication.isPlaying = false;


#else
                            Application.Quit();
#endif
                            ClosePopup();

                        }, null, "Yes", "No");
                        break;


                   /* case LoadSceneManager.SceneState.Lobby:
                        OpenPopup_OkCancel("[000000]Notice", "[000000]Are you sure to return to Title?", () => LoadSceneManager.Instance.LoadSceneAsync(LoadSceneManager.SceneState.Title), null, "Yes", "No");
                        break;
                    case LoadSceneManager.SceneState.Game:
                        OpenPopup_OkCancel("[000000]Notice", "[000000]Are you sure to return to Lobby?", () => LoadSceneManager.Instance.LoadSceneAsync(LoadSceneManager.SceneState.Lobby), null, "Yes", "No");
                        break;*/

                }
            }
        }
    }
}
