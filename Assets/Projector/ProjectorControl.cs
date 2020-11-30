using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectorControl : MonoBehaviour
{

    public Vector3 OffsetPos;
    private Camera mainCamera = null;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3( mainCamera.transform.position.x + OffsetPos.x, transform.position.y, mainCamera.transform.position.z + OffsetPos.z);
    }
}
