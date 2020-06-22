using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	static readonly Vector2 bound = new Vector2( 36,  26);

    public Transform target;
	public Vector3 offsetPos;
	//Camera camera;

	private void Awake()
	{
		//camera = Camera.main;
	}
	// Update is called once per frame
	void LateUpdate()
    {
		Vector3 pos = new Vector3(target.position.x + offsetPos.x, target.position.y + offsetPos.y, target.position.z + offsetPos.z);

		// 界限限制
		if (pos.x < 0) { pos.x = Math.Max(pos.x, -bound.x); }
		else if (pos.x > 0) { pos.x = Math.Min(pos.x, bound.x); }
		if (pos.y < 0) { pos.y = Math.Max(pos.y, -bound.y); }
		else if (pos.y > 0) { pos.y = Math.Min(pos.y, bound.y); }

		transform.position = pos;
	}
}
