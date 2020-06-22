using UnityEngine;

public class TitleScene : BaseScene
{
    public override void Init(System.Object parms)
    {
        m_sceneData.Add(new SceneData(){
            Resource = "UI/Prefabs/Title",
            ResType = typeof(GameObject)
        });
    }

    public override void Create()
    {
        Debug.Log("OnCreate");
        // do ui logic
        BlUIManager.Instance.AddPanel(new TitlePanel("UI/Prefabs/Title"));
    }

    public override void Destory()
    {
        Debug.Log("OnCreate");
        // do ui logic
    }
}