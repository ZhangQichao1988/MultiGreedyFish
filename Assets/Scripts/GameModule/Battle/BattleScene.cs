using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BattleScene : BaseScene
{
    /// <summary>
    /// 添加资源需要加载的资源
    /// </summary>
    /// <param name="parms">外部传入的参数</param>
    public override void Init(object parms)
    {
        // m_sceneData.Add(new SceneData(){
        //     Resource = "UI/ABC/bbb",
        //     ResType = typeof(GameObject)
        // });
    }

    //场景加载完毕
    public override void Create()
    {
        //初始化资源等
        Debug.Log("OnSceneLoaded Do Something");

        //GameObject.Instantiate(cachedObject["UI/ABC/BBB"]);
    }

    public override void Destory()
    {
        base.Destory();
    }
}