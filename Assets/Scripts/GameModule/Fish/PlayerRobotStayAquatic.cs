using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRobotStayAquatic : PlayerRobotBase
{
	protected override void CalcMoveAction()
	{
		List<Transform> listTransAquatic = ManagerGroup.GetInstance().aquaticManager.listTransAquatic;
		if (listTransAquatic.Count <= 0) { return; }
		listTransAquatic.Sort((a,b)=> { return (int)(Vector3.Distance(transform.position, a.position) - Vector3.Distance(transform.position, b.position)); });
		MoveToTarget(listTransAquatic[0].transform.position);
	}
}
