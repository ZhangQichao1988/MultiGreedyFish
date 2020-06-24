using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance { get; private set; }
    public static void Init()
    {
        instance = GameObject.Find("Main Camera").AddComponent<CameraFollow>();
    }
    public Transform Target;
    public float Angle;
    public float Distance;
    public float Offset;
    public float Radius;
    public Vector3 RadiusCenter;
    public Camera SelfCamera;
    public Vector3 OffsetPos;
    public Vector3 FixOffsetPos;

    public float BaseRadius;

    private Vector3 oldPos;
    private const float thinkingTime = 7;

    void Start()
    {
        SelfCamera = GetComponent<Camera>();
        SelfCamera.fieldOfView = 22.5f;
        Angle = 42.2f;
        Distance = 90f;
        Offset = 0f;
        Radius = 1f;
        BaseRadius = 90;
        curState = State.MovingTop;
        OffsetPos = Vector3.zero;
        FixOffsetPos = Vector3.forward * 1.6f;
    }
    public void SetTarget(Transform target)
    {
        Target = target;
        if (Target != null)
            RadiusCenter = Target.position;
    }
    Vector3 TargetPos;
    Vector3 ChangePos;
    void LateUpdate()
    {
		if (Time.time <= ChangeStateTime)
		{
			float lerp = 1 - (ChangeStateTime - Time.time) / thinkingTime;
			switch (curState)
			{
				case State.MovingTop:
					Angle = Mathf.Lerp(Angle, 42.2f, lerp);
					Distance = Mathf.Lerp(Distance, 36.6f, lerp);
					Offset = Mathf.Lerp(Offset, 0, lerp);
					ChangePos = Vector3.Lerp(ChangePos, RadiusCenter + OffsetPos, lerp);
					if (Mathf.Abs(Angle - 42.2f) < 0.05f)
					{
						Angle = 42.2f;
						Distance = 90f;
						Offset = 0;
						ChangePos = RadiusCenter + OffsetPos;
						ChangeStateTime = 0;
					}
					break;
				case State.Thinking:
					Angle = Mathf.Lerp(Angle, 29, lerp);
					Distance = Mathf.Lerp(Distance, 18, lerp);
					Offset = Mathf.Lerp(Offset, 2.5f, lerp);
					ChangePos = Vector3.Lerp(ChangePos, Target.position, lerp);
					break;
				case State.WatchPhone:
					Angle = Mathf.Lerp(Angle, 20, lerp);
					Distance = Mathf.Lerp(Distance, 17, lerp);
					Offset = Mathf.Lerp(Offset, 1.5f, lerp);
					ChangePos = Vector3.Lerp(ChangePos, Target.position, lerp);
					break;
				case State.ShowItem:
					Angle = Mathf.Lerp(Angle, 20, lerp);
					Distance = Mathf.Lerp(Distance, 17, lerp);
					Offset = Mathf.Lerp(Offset, 1f, lerp);
					ChangePos = Vector3.Lerp(ChangePos, Target.position, lerp);
					break;
			}
		}
		if (Target)
		{
			TargetPos = Target.position;
			if (curState == State.MovingTop && Time.time > ChangeStateTime)
			{

				if (Vector3.Distance(TargetPos, RadiusCenter) > Radius && oldPos != TargetPos)
				{
					var offsetDir = (TargetPos - RadiusCenter);
					RadiusCenter = TargetPos - offsetDir.normalized * Radius;

					OffsetPos += offsetDir * 0.5f * Time.deltaTime;
					if (OffsetPos.magnitude > Radius)
						OffsetPos.Normalize();
				}
				ChangePos = RadiusCenter + OffsetPos;
				oldPos = TargetPos;
			}
			TargetPos = ChangePos + Offset * Vector3.up + FixOffsetPos;
			transform.position = TargetPos + new Vector3(0, Distance * Mathf.Sin(Angle * Mathf.Deg2Rad), -Distance * Mathf.Cos(Angle * Mathf.Deg2Rad));
			transform.LookAt(TargetPos);
		}
	}


    public enum State
    {
        MovingTop,
        Thinking,
        WatchPhone,
        ShowItem,
    }

    public State curState { get; private set; }
    float ChangeStateTime = 0;
    public void SetState(State state)
    {
        curState = state;
        ChangeStateTime = Time.time + thinkingTime;
    }
}
