using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBlendShapeBehaviour : MonoBehaviour
{

    int blendShapeCount;
    public SkinnedMeshRenderer skinnedMeshRenderer;
    Mesh skinnedMesh;
    public float process = 0f;

    void Awake()
    {
        //skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = skinnedMeshRenderer.sharedMesh;
    }

    void Start()
    {
        blendShapeCount = skinnedMesh.blendShapeCount;
    }

	private void Update()
	{
        skinnedMeshRenderer.SetBlendShapeWeight(0, process);
    }
	//void OnGUI()
 //   {
 //       //21，是指模型中设定发生形变张开嘴巴  0:左眼闭合 1:右眼闭合
 //       if (GUI.Button(new Rect(100, 100, 150, 80), "张开嘴巴 && 闭眼"))
 //       {
 //           skinnedMeshRenderer.SetBlendShapeWeight(0, 100);
 //           skinnedMeshRenderer.SetBlendShapeWeight(1, 100);
 //           skinnedMeshRenderer.SetBlendShapeWeight(21, 100);
 //       }
 //       if (GUI.Button(new Rect(100, 200, 150, 80), "闭合嘴巴 && 睁眼"))
 //       {
 //           skinnedMeshRenderer.SetBlendShapeWeight(0, 0);
 //           skinnedMeshRenderer.SetBlendShapeWeight(1, 0);
 //           skinnedMeshRenderer.SetBlendShapeWeight(21, 0);
 //       }
 //   }
}