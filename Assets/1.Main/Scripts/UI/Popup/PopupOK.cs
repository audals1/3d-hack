using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupOK : MonoBehaviour
{
    [SerializeField]
    UILabel m_subject;
    [SerializeField]
    UILabel m_body;
    [SerializeField]
    UILabel m_okBtnText;
    
    ButtonDelegate m_okDelegate;
    
    public void SetUI(string subject, string body, ButtonDelegate okDel, string okBtnText = "OK")
    {
        m_subject.text = subject;
        m_body.text = body;
        m_okDelegate = okDel;
        m_okBtnText.text = okBtnText;
        
        
    }
    public void OnPressOK()
    {
        if (m_okDelegate != null)
        {
            m_okDelegate();
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
    // Start is called before the first frame update

}
