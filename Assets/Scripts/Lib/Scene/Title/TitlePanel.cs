using UnityEngine;
using UnityEngine.UI;


public class TitlePanel : UIPanel
{
    public TitlePanel(string path):base(path)
    {

    }
    protected override void OnEnter()
    {
        Debug.Log("OnEnter Title Panel");
    }
}