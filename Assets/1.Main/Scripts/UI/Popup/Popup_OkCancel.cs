using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Popup_OkCancel : MonoBehaviour
{
    [SerializeField]
    UILabel m_subject;
    [SerializeField]
    UILabel m_body;
    [SerializeField]
    UILabel m_okBtnText;
    [SerializeField]
    UILabel m_cancelBtnText;
    ButtonDelegate m_okDelegate;
    ButtonDelegate m_cancelDelegate;
    public void SetUI(string subject, string body, ButtonDelegate okDel, ButtonDelegate cancelDel, string okBtnText = "OK", string CancelBtnText = "Cancel")
    {
        m_subject.text = subject;
        m_body.text = body;
        m_okDelegate = okDel;
        m_cancelDelegate = cancelDel;
        m_okBtnText.text = okBtnText;
        m_cancelBtnText.text = CancelBtnText;
    }
    public void OnPressOK()
    {
        if(m_okDelegate != null)
        {
            m_okDelegate();
        }
        else
        {
            PopupManager.Instance.ClosePopup();
        }
    }

    public void OnPressCancel()
    {
        if(m_cancelDelegate != null)
        {
            m_cancelDelegate();
        }
        else
        {
            PopupManager.Instance.ClosePopup();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
