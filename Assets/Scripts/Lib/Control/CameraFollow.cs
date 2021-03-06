using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance { get; private set; }

    public void Init()
    {
		currentRate = 0f;
		firstUpdate = true;

	}

	public Transform CameraTrans;
	public Transform Target;
    public float Angle;
    public float Distance;
	public float MinDistance = 35f;
	public float MaxDistance = 50f;

	public float Offset;
    public float Radius;
    public Vector3 RadiusCenter;
    public Camera SelfCamera;
    public Vector3 OffsetPos;
    public Vector3 FixOffsetPos;

    public float BaseRadius;

    private Vector3 oldPos;
    private const float thinkingTime = 7;

	private float currentRate = 0f;

	//private Vector3 currentPos;
	//private Vector3 targetPos;

	private Vector3 currentPlayerPos;
	public Vector3 targetPlayerPos;
	private bool firstUpdate;

	void Start()
    {
        SelfCamera = GetComponent<Camera>();
        SelfCamera.fieldOfView = 22.5f;
        Angle = 42.2f;
        Distance = MinDistance;
        Offset = 0f;
        Radius = 1f;
        BaseRadius = 90;
        curState = State.MovingTop;
		//OffsetPos = Vector3.zero;
		//FixOffsetPos = Vector3.forward * 1.6f;
	}

    public void SetTarget(Transform target)
    {
		var audio = Target.gameObject.GetComponent<AudioListener>();
		Destroy(audio);
		target.gameObject.AddComponent<AudioListener>();
		Target = target;
        if (Target != null)
            RadiusCenter = Target.position;
    }
    Vector3 PlayerPos;
    Vector3 ChangePos;
    void LateUpdate()
    {
		//if (Time.time <= ChangeStateTime)
		//{
		//	float lerp = 1 - (ChangeStateTime - Time.time) / thinkingTime;
		//	Angle = Mathf.Lerp(Angle, 38f, lerp);
		//	Distance = Mathf.Lerp(Distance, 90f, lerp);
		//	Offset = Mathf.Lerp(Offset, 0, lerp);
		//	ChangePos = Vector3.Lerp(ChangePos, RadiusCenter + OffsetPos, lerp);
		//	if (Mathf.Abs(Angle - 42.2f) < 0.05f)
		//	{
		//		Angle = 28f;
		//		Distance = 45f;
		//		Offset = 0;
		//		ChangePos = RadiusCenter + OffsetPos;
		//		ChangeStateTime = 0;
		//	}
		//}
		if (Target)
		{
			float rate = (Target.transform.localScale.x - 1f) / (BattleConst.instance.FishMaxScale - 1f);
			if (currentRate < rate)
			{
				currentRate += Time.deltaTime * 0.1f;
			}

			Distance = Mathf.Lerp(MinDistance, MaxDistance, currentRate);
			//FixOffsetPos.z = Mathf.Lerp(-5f, -13f, currentRate);
			SelfCamera.nearClipPlane = Mathf.Lerp(0.1f, 0.1f, currentRate);
			SelfCamera.farClipPlane = Mathf.Lerp(90f, 110f, currentRate);

			targetPlayerPos = Vector3.Lerp(Vector3.zero, Target.position, CameraTrans.localPosition.x);
			if (firstUpdate)
			{
				firstUpdate = false;
				currentPlayerPos = targetPlayerPos; 
			}
			else
			{ currentPlayerPos = currentPlayerPos + (targetPlayerPos - currentPlayerPos) * Time.deltaTime * 3f; }
			PlayerPos = currentPlayerPos;

			if (curState == State.MovingTop && Time.time > ChangeStateTime)
			{

				if (Vector3.Distance(PlayerPos, RadiusCenter) > Radius && oldPos != PlayerPos)
				{
					var offsetDir = (PlayerPos - RadiusCenter);
					RadiusCenter = PlayerPos - offsetDir.normalized * Radius;

					OffsetPos += offsetDir * 0.5f * Time.deltaTime;
					if (OffsetPos.magnitude > Radius)
						OffsetPos.Normalize();
				}
				ChangePos = RadiusCenter + OffsetPos;
				oldPos = PlayerPos;
			}
			PlayerPos = ChangePos + Offset * Vector3.up + FixOffsetPos;
			transform.position = PlayerPos + new Vector3(0, Distance * Mathf.Sin(Angle * Mathf.Deg2Rad), -Distance * Mathf.Cos(Angle * Mathf.Deg2Rad));
			


			transform.LookAt(PlayerPos);
		}
		else
		{
			var listPlayer = BattleManagerGroup.GetInstance().fishManager.GetAlivePlayer();
			if (listPlayer.Count > 0)
			{
				Target = listPlayer[0].transform;
			}
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
