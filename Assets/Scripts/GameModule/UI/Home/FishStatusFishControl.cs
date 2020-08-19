using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FishStatusFishControl : MonoBehaviour
{

    public Camera camera;
    float offset = 5f;
    float spd = 5f;
    float animSpd = 0.025f;
    float changePosLen = 0.1f;
    //GameObject effectBlister;
    Animator animator;
    Transform transModel;
    Vector3 Dir, curDir, moveDir;
    Vector3 targetPos;
    Vector3 moveVec;

    public void SetFishModel(GameObject goFish)
    {
        if (transModel != null) { Destroy(transModel.gameObject); }
        transModel = goFish.transform;
        targetPos = transModel.position;
        animator = goFish.GetComponent<Animator>();
        GameObject goMouth = GameObject.Find("mouth");
        Debug.Assert(goMouth != null, "HomeFishControl.SetFishModel()_1");
        GameObject go = ResourceManager.LoadSync(Path.Combine(AssetPathConst.effectRootPath, "fx_blister_home"), typeof(GameObject)).Asset as GameObject;
        Instantiate(go, goMouth.transform);
    }
    private void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Ray camRay = camera.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit floorHit;
        //    int layMask = 1 << LayerMask.NameToLayer("HomeCollider");
        //    if (Physics.Raycast(camRay, out floorHit, 5000f , layMask))
        //    {
        //        targetPos = floorHit.point;
        //        spd = 5f;
        //    }
        //}
        //moveVec = new Vector3(targetPos.x, targetPos.y, targetPos .z - offset) - transModel.position;
        //Dir = moveVec.normalized;
        //// 靠近目的地后就开始闲逛
        //if (new Vector2(targetPos.x - transModel.position.x, targetPos.y - transModel.position.y).sqrMagnitude < changePosLen)
        //{
        //    targetPos = new Vector3(Wrapper.GetRandom(-80f, 80f), Wrapper.GetRandom(-30f, 40f), transModel.position.z);
        //    spd = 1f;
        //}
        //MoveUpdate();
    }
}
