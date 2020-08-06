using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BattleScene : BaseScene
{
    /// <summary>
    /// 添加资源需要加载的资源
    /// </summary>
    /// <param name="parms">外部传入的参数</param>
    public override void Init(object parms)
    {
        // Other
        m_sceneData.Add(new SceneData(){ Resource = Path.Combine(AssetPathConst.lifeGaugePath), ResType = typeof(GameObject) });
        m_sceneData.Add(new SceneData() { Resource = Path.Combine(AssetPathConst.playerNameplatePrefabPath), ResType = typeof(GameObject) });
        m_sceneData.Add(new SceneData() { Resource = Path.Combine(AssetPathConst.robotNameplatePrefabPath), ResType = typeof(GameObject) });

        // 鱼
        List<int> listFishIds = new List<int>();

        // 玩家鱼ID
        listFishIds.Add(1); // TODO:从Response获取玩家鱼ID

        // 杂鱼ID,一般都是固定的
        listFishIds.AddRange(new int[] { 0 });

        // AI鱼
        var aryRobot = DataBank.stageInfo.AryRobotDataInfo;
        for (int i = 0; i < aryRobot.Count; ++i)
        {
            listFishIds.Add(aryRobot[i].FishId);
        }
        
        FishDataInfo fishBaseData;
        FishSkillDataInfo fishSkillBaseData;
        EffectDataInfo effectBaseData;
        foreach (int fishId in listFishIds)
        {
            fishBaseData = FishDataTableProxy.Instance.GetDataById(fishId);
            // 鱼本体
            m_sceneData.Add(new SceneData() { Resource = Path.Combine(AssetPathConst.fishPrefabRootPath + fishBaseData.prefabPath), ResType = typeof(GameObject) });

            if (fishBaseData.skillId > 0)
            {
                // 技能特效
                fishSkillBaseData = FishSkillDataTableProxy.Instance.GetDataById(fishBaseData.skillId);
                if (fishSkillBaseData.effectId > 0)
                {
                    effectBaseData = EffectDataTableProxy.Instance.GetDataById(fishSkillBaseData.effectId);
                    m_sceneData.Add(new SceneData() { Resource = Path.Combine(AssetPathConst.effectRootPath + effectBaseData.prefabPath), ResType = typeof(GameObject) });
                }
            }
        }

    }

    //场景加载完毕
    public override void Create()
    {
        //初始化资源等
        Debug.Log("OnSceneLoaded Do Something");

        //GameObject.Instantiate(cachedObject[Path.Combine(UIPathRoot, "BattleControl")]);
    }

    public override void Destory()
    {
        base.Destory();
    }
}