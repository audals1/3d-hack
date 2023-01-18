using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gameover : SingletonMonoBehaviour<Gameover>
{
    [SerializeField]
    UISprite m_gameoverimg;
    [SerializeField]
    UISprite m_victoryimg;
    [SerializeField]
    UILabel m_clearTime;
    [SerializeField]
    UILabel m_timer;
    public void Defeat()
    {
        m_gameoverimg.gameObject.SetActive(true);
        m_clearTime.text = m_timer.text;
        m_clearTime.gameObject.SetActive(true);
    }
    public void Victory()
    {
        m_victoryimg.gameObject.SetActive(true);
        m_clearTime.text = m_timer.text;
        m_clearTime.gameObject.SetActive(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        m_gameoverimg.gameObject.SetActive(false);
        m_victoryimg.gameObject.SetActive(false);
        m_clearTime.gameObject.SetActive(false);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            Defeat();
        }
    }
}
