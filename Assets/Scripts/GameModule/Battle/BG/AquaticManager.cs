using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AquaticManager : MonoBehaviour
{
	public List<Transform> listTransAquatic = new List<Transform>();
	public List<GameObject> listGoTargetIcon = new List<GameObject>();
	private uint updateCnt = 0;

	private void Awake()
	{
		Transform transTmp = null;
		bool isTurorial = TutorialControl.IsCompleted();
		var go = ResourceManager.LoadSync(AssetPathConst.targetIconAquaticPath, typeof(GameObject)).Asset as GameObject;
		for (int i = 0; i < transform.childCount; ++i)
		{
			transTmp = transform.GetChild(i);
			if (transTmp.gameObject.activeSelf)
			{
				listTransAquatic.Add(transTmp);
                if (!isTurorial)
                {
                    listGoTargetIcon.Add(GameObjectUtil.InstantiatePrefab(go, transTmp.gameObject, false));
                }
            }
		}
	}
	public void ShowTargetIcon(bool enable)
	{
		foreach (var note in listGoTargetIcon)
		{
			note.SetActive(enable);
		}
	}
    private void Update()
    {
		++updateCnt;
		if (updateCnt >= int.MaxValue) { updateCnt = 0; }
		if (updateCnt % 10 != 1) { return; }

#if !UNITY_EDITOR
		// 不在视野范围内就不显示
		bool isActive = false;
		for (int i = 0; i < listTransAquatic.Count; ++i)
        {
			isActive = BattleConst.instance.RobotVisionRange > Vector3.SqrMagnitude(BattleManagerGroup.GetInstance().cameraFollow.targetPlayerPos - listTransAquatic[i].position);
			GameObjectUtil.SetActive(listTransAquatic[i].gameObject, isActive);
        }
#endif
    }
 //   public bool IsInAquatic(FishBase fish)
	//{
	//	Vector3 myPos = fish.transform.position;
	//	for (int i = 0; i < listTransAquatic.Count; ++i)
	//	{
	//		//if (!listTransAquatic[i].gameObject.activeSelf) { continue; }

	//		float distance = Vector3.Distance(listTransAquatic[i].position, myPos);
	//		if (distance <= BattleConst.instance.AquaticRange)
	//		{
	//			return true;
	//		}
	//	}
	//	return false;
	//}
}
