using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 20f)]
    float m_distance;
    [SerializeField]
    [Range(0f, 20f)]
    float m_height;
    [SerializeField]
    [Range(0f, 180f)]
    float m_angle;
    [SerializeField]
    [Range(0.1f, 5f)]
    float m_speed;
    [SerializeField]
    Transform m_target;
    //[SerializeField]
    //RenderTexture m_screenTex;
    Transform m_prevPos;
    //Material m_material;
    //private void OnPreCull()
    //{
    //    Camera.main.targetTexture = m_screenTex;
    //}
    //private void OnPostRender()
    //{
    //    Camera.main.targetTexture = null;
    //    Graphics.Blit(m_screenTex, m_material);
    //}


    // Start is called before the first frame update
    void Start()
    {
       // m_material = new Material(Shader.Find("Unlit/Texture"));
        m_prevPos = transform;
        //Screen.SetResolution((int)(Screen.width*0.9f), (int)(Screen.height*0.9f), true); 해상도 조절
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            Mathf.Lerp(m_prevPos.position.x, m_target.position.x, m_speed * Time.deltaTime),
            Mathf.Lerp(m_prevPos.position.y, m_target.position.y + m_height, m_speed * Time.deltaTime),
            Mathf.Lerp(m_prevPos.position.z, m_target.position.z - m_distance, m_speed * Time.deltaTime)
            );
        transform.rotation = Quaternion.Lerp(m_prevPos.rotation, Quaternion.Euler(m_angle, 0f, 0f), m_speed * Time.deltaTime);
        m_prevPos = transform;
    }
}
