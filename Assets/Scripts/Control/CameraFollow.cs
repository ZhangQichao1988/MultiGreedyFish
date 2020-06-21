using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform m_target;
	public Vector3 m_offsetPos;
	Camera m_camera;

	private void Awake()
	{
		m_camera = Camera.main;
	}
	// Update is called once per frame
	void LateUpdate()
    {
		transform.position = new Vector3(m_target.position.x + m_offsetPos.x, m_target.position.y + m_offsetPos.y, m_target.position.z + m_offsetPos.z);
	}
}
