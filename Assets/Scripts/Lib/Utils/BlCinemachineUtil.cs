using UnityEngine;
using System.Collections;
using Cinemachine;

public static class BlCinemachineUtil
{
	public static void SetFollowTarget(GameObject virtualCameraGameObject, GameObject target)
	{
		CinemachineVirtualCameraBase virtualCamera = virtualCameraGameObject.GetComponent<CinemachineVirtualCameraBase>();
        


        if (virtualCamera != null)
		{
			virtualCamera.Follow = target.transform;
		}
	}

	public static void SetLookAtTarget(GameObject virtualCameraGameObject, GameObject target)
	{
		CinemachineVirtualCameraBase virtualCamera = virtualCameraGameObject.GetComponent<CinemachineVirtualCameraBase>();

		if (virtualCamera != null)
		{
			virtualCamera.LookAt = target.transform;
		}
	}

	public static void SetDollyCameraPathPosition(GameObject virtualCameraGameObject, float value)
	{
		CinemachineVirtualCamera virtualCamera = virtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();

		if (virtualCamera != null)
		{
			CinemachineTrackedDolly cinemachineTrackedDolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();

			if(cinemachineTrackedDolly != null)
			{
				cinemachineTrackedDolly.m_PathPosition = value;
			}
		}
	}

	public static void MoveDollyCamera(GameObject virtualCameraGameObject, float value)
	{
		CinemachineVirtualCamera virtualCamera = virtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();

		if (virtualCamera != null)
		{
			CinemachineTrackedDolly cinemachineTrackedDolly = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();

			if (cinemachineTrackedDolly != null)
			{
				cinemachineTrackedDolly.m_PathPosition = 
					Mathf.Clamp(cinemachineTrackedDolly.m_PathPosition + value, 
					cinemachineTrackedDolly.m_Path.MinPos, 
					cinemachineTrackedDolly.m_Path.MaxPos);
			}
		}
	}

    public static void SetScreenX(GameObject virtualCameraGameObject, float x)
    {
        CinemachineVirtualCamera virtualCamera = virtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            CinemachineComposer cc = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
            cc.m_ScreenX = x;
        }
    }

    public static void SetPriority(GameObject virtualCameraGameObject, int val)
    {
        CinemachineVirtualCamera virtualCamera = virtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            virtualCamera.Priority = val;
        }
    }

    public static void ResetTargetGroup(GameObject targetGroupGameObject)
    {
        CinemachineTargetGroup targetGroup = targetGroupGameObject.GetComponent<CinemachineTargetGroup>();
        if (targetGroup != null)
        {
            for(int i = 0; i < targetGroup.m_Targets.Length; i++)
            {
                targetGroup.m_Targets[i].target = null;
            }
        }
    }

    public static void SetTargetGroup(GameObject targetGroupGameObject,int index, Transform target)
    {
        CinemachineTargetGroup targetGroup = targetGroupGameObject.GetComponent<CinemachineTargetGroup>();
        if (targetGroup != null)
        {
            targetGroup.m_Targets[index].target = target;
        }
    }

    public static void ActiveCameraCullingMask(GameObject cameraGameObject, bool isOn, int layer)
    {
        Camera camera = cameraGameObject.GetComponent<Camera>();
        if(camera != null)
        {
            if (isOn == true)
            {
                camera.cullingMask |= (1 << layer);
            }
            else
            {
                camera.cullingMask &= ~(1 << layer);
            }
        }
  
    }

}
