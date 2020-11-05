using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FishStatusFishControl : MonoBehaviour
{

    public Camera camera;
    Animator animator;
    Transform transModel;

    float FishRotAngle = 90f;
    float preTouchPosX = 0f;
    float nowTouchPosX = 0f;
    bool isDrage = false;

    public void CreateFishModel(int fishId)
    {
        var fishBaseData = FishDataTableProxy.Instance.GetDataById(fishId);
        var asset = ResourceManager.LoadSync(Path.Combine(AssetPathConst.fishPrefabRootPath + fishBaseData.prefabPath), typeof(GameObject));
        GameObject go = GameObjectUtil.InstantiatePrefab(asset.Asset as GameObject, gameObject);
        SetFishModel(go);
    }
    public void SetFishModel(GameObject goFish)
    {
        if (transModel != null) { Destroy(transModel.gameObject); }
        transModel = goFish.transform;
        animator = goFish.GetComponent<Animator>();
        animator.SetFloat("Speed", 0.5f);
        //GameObject goMouth = GameObject.Find("mouth");
        //Debug.Assert(goMouth != null, "HomeFishControl.SetFishModel()_1");
        //GameObject go = ResourceManager.LoadSync(Path.Combine(AssetPathConst.effectRootPath, "fx_blister_home"), typeof(GameObject)).Asset as GameObject;
        //Instantiate(go, goMouth.transform);
    }
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (!isDrage)
            {
                Ray camRay = camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                int layMask = 1 << LayerMask.NameToLayer("Fish");
                if (Physics.Raycast(camRay, out hit, 5000f, layMask))
                {
                    isDrage = true;
                }
            }

            if (isDrage)
            {
                nowTouchPosX = Input.mousePosition.x;
                if (preTouchPosX != 0f)
                {
                    FishRotAngle += (preTouchPosX - nowTouchPosX) * 0.5f;
                    transModel.rotation = Quaternion.AngleAxis(FishRotAngle, Vector3.up);
                }
                preTouchPosX = nowTouchPosX;
            }
        }
        else 
        {
            isDrage = false;
            preTouchPosX = 0f;
        }


        




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
