using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// by 명민, NGUI sprite로 게임패드 구현
/// </summary>
public class MovePad : SingletonMonoBehaviour<MovePad>
{
    #region Filed
    [SerializeField]
    UISprite m_padBG;
    [SerializeField]
    UISprite m_button;
    [SerializeField]
    Camera m_uiCam;
    [SerializeField]
    UIButton m_buttonMain;
    [SerializeField]
    UIButton m_buttonSkill1;
    [SerializeField]
    UIButton m_buttonSkill2;
    [SerializeField]
    UIButton m_buttonSkill3;
    [SerializeField]
    UIButton m_buttonpause;
    Vector3 m_dir;
    RaycastHit m_hit;
    int m_fingerId;
    bool m_isDrag;
    float m_maxDist = 0.216f;
    public bool IsPress { get { return m_dir != Vector3.zero; } }
    #endregion
    // Start is called before the first frame update
    protected override void OnStart()
    {
        m_fingerId = -1;
    }
    public Vector2 GetAxis()
    {
        return m_dir;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        //마우스 클릭 지점을 따라 패드가 움직임
        if(Input.GetMouseButtonDown(0)) 
        {
            Ray ray = m_uiCam.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out m_hit, 100f, 1 << LayerMask.NameToLayer("UI"))) //Layer가 UI인 오브젝트에 Ray를 쏴서 
            {
                if(m_hit.collider.gameObject.transform == m_padBG.transform) // pad배경 오브젝트에 맞으면
                {
                    var dir = m_hit.point - m_padBG.transform.position; // 클릭한 방향으로
                    m_dir = dir;
                    m_dir.Normalize();
                    if(dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
                    {
                        m_button.transform.position = m_padBG.transform.position + dir; //버튼이 pad배경 영역 안에서 움직임
                    }
                    else
                    {
                        m_button.transform.position = m_padBG.transform.position + m_dir * m_maxDist;   //pad배경 영역을 벗어난 경우 패드가 배경 내에서만 움직이게 조건부여
                    }
                }
            }
            m_isDrag = true;// 누르고 있는 상태임을 체크
        }
        if(Input.GetMouseButtonUp(0)) //마우스 뗐을때 초기화
        {
            m_isDrag = false;
            if (m_hit.collider != null) m_hit = new RaycastHit();
            m_button.transform.localPosition = Vector3.zero;
            m_dir = Vector3.zero;
        }
        if(m_isDrag) //게임패드에 마우스를 계속 누르고 있는 상태일 경우
        {
            if (m_hit.collider != null)
            {
                var worldPos = m_uiCam.ScreenToWorldPoint(Input.mousePosition);
                var dir = worldPos - m_padBG.transform.position;
                m_dir = dir;
                m_dir.Normalize();
                if (dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
                {
                    m_button.transform.position = m_padBG.transform.position + dir;
                }
                else // 게임패드 버튼들 중 우측에 있는 스킬버튼을 눌렀을때 Layer가 ui로 같아서 이동패드가 자꾸 움직이는 에러 발생
                {
                    //스킬버튼을 눌렀을때는 이동패드 움직임 없게 조건부여
                    if (m_hit.collider.gameObject.transform == m_buttonMain.transform || m_hit.collider.gameObject.transform == m_buttonSkill1.transform || m_hit.collider.gameObject.transform == m_buttonSkill2.transform || m_hit.collider.gameObject.transform == m_buttonSkill3.transform || m_hit.collider.gameObject.transform == m_buttonpause.transform)
                    {
                        m_button.transform.localPosition = Vector3.zero;
                        m_dir = Vector3.zero;
                    }
                    else
                        m_button.transform.position = m_padBG.transform.position + m_dir * m_maxDist;
                }
            }
        }
#elif UNITY_ANDROID || UNITY_IOS
        if(Input.touchPressureSupported) //모바일 터치 관련
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if(Input.touches[i].phase == TouchPhase.Began) // 손가락 터치 상태
                {
                    Ray ray = m_uiCam.ScreenPointToRay(Input.touches[i].position);
                    if (Physics.Raycast(ray, out m_hit, 100f, 1 << LayerMask.NameToLayer("UI")))
                    {
                        if (m_hit.collider.gameObject.transform == m_padBG.transform)
                        {
                            var dir = m_hit.point - m_padBG.transform.position;
                            m_dir = dir;
                            m_dir.Normalize();
                            if (dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
                            {
                                m_button.transform.position = m_padBG.transform.position + dir;
                            }
                            else
                            {
                                m_button.transform.position = m_padBG.transform.position + m_dir * m_maxDist;
                            }
                            m_fingerId = Input.touches[i].fingerId; //누른 손가락 번호 기억
                        }
                    }
                }
                if(Input.touches[i].phase == TouchPhase.Moved && Input.touches[i].fingerId == m_fingerId) //손가락 드래그상태
                {
                    if (m_hit.collider != null)
                    {
                        var worldPos = m_uiCam.ScreenToWorldPoint(Input.touches[i].position);
                        var dir = worldPos - m_padBG.transform.position;
                        m_dir = dir;
                        m_dir.Normalize();
                        if (dir.sqrMagnitude < Mathf.Pow(m_maxDist, 2f))
                        {
                            m_button.transform.position = m_padBG.transform.position + dir;
                        }
                        else
                        {
                            m_button.transform.position = m_padBG.transform.position + m_dir * m_maxDist;
                        }
                    }
                }
                if((Input.touches[i].phase == TouchPhase.Canceled || Input.touches[i].phase == TouchPhase.Ended) && Input.touches[i].fingerId == m_fingerId) // 손가락 터치 떨어짐
                {
                    m_button.transform.localPosition = Vector3.zero;
                    m_dir = Vector3.zero;
                    m_fingerId = -1;
                    m_hit = new RaycastHit();
                }
            }
        }
#endif
    }
}
