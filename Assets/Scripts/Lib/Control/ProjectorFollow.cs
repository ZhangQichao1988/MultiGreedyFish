using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorFollow : MonoBehaviour
{
    public CameraFollow cameraFollow;
    public Vector3 offSet;
    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(cameraFollow.targetPlayerPos.x, transform.position.y, cameraFollow.targetPlayerPos.z) + offSet;
    }
}
