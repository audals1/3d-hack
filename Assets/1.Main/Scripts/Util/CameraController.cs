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
    Transform m_prevPos;


    // Start is called before the first frame update
    void Start()
    {
        m_prevPos = transform;
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
