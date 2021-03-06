using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellManager : MonoBehaviour
{
	public List<ShellControl> listShell = new List<ShellControl>();
	private uint updateCnt = 0;
	private void Awake()
	{
		Transform transTmp = null;
		for (int i = 0; i < transform.childCount; ++i)
		{
			transTmp = transform.GetChild(i);
			if (transTmp.gameObject.activeSelf)
			{
				listShell.Add(transTmp.GetComponent<ShellControl>());
			}
		}
	}
	public void ShowTargetIcon(bool enable)
	{
		listShell.ForEach((a) => a.ShowTargetIcon(enable));
	}
    private void Update()
    {

		++updateCnt;
		if (updateCnt >= int.MaxValue) { updateCnt = 0; }
		if (updateCnt % 10 != 1) { return; }

		bool isActive = false;
		for (int i = 0; i < listShell.Count; ++i)
        {
			// 不在视野范围内就不显示
			isActive = BattleConst.instance.RobotVisionRange > Vector3.SqrMagnitude(BattleManagerGroup.GetInstance().cameraFollow.targetPlayerPos - listShell[i].transform.position);
			GameObjectUtil.SetActive(listShell[i].gameObject, isActive);
        }
    }
    public void EatPearl(PlayerBase fish)
	{
		for (int i = 0; i < listShell.Count; ++i)
		{
			float distance = Vector3.Distance(listShell[i].transform.position, fish.transform.position);
			if (distance <= BattleConst.instance.EatPearlRange)
			{
				// 根据开合状态来判定吃了珍珠还是被夹
				if (listShell[i].PearlEatenCheck(fish))
				{
					return;
				}
			}
		}
	}

	public ShellControl GetPearlWithRange(Vector3 pos, Vector2 range)
	{
		List<ShellControl> _listShell = new List<ShellControl>(listShell);
		_listShell.Sort((a, b) => { return (int)(Vector3.Distance(a.transform.position, pos) - Vector3.Distance(b.transform.position, pos)); });
		for (int i = 0; i < _listShell.Count; ++i)
		{
			if (!_listShell[i].CanEatPearl()) { continue; }
			// 剔除毒圈以外的珍珠
			if (_listShell[i].transform.position.sqrMagnitude > BattleManagerGroup.GetInstance().poisonRing.GetPoisonRangePow() - 50)
			{ continue; }
			if (pos.x + range.x > _listShell[i].transform.position.x &&
				pos.x - range.x < _listShell[i].transform.position.x &&
				pos.z + range.y > _listShell[i].transform.position.z &&
				pos.z - range.y < _listShell[i].transform.position.z)
			{
				return _listShell[i];
			}
		}
		return null;
	}
}