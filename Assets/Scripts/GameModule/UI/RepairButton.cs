using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class RepairButton : UIBase
{
    public override void OnEnter(System.Object obj)
	{

	}

    public void OnClickReapir()
    {
        MsgBox.OpenConfirm("repair", "repair the game", ()=>{
            GameHelper.Repair();
        });
    }
}