using UnityEngine;
using UnityEngine.UI;

public class MsgBox : UIBase
{
    public Text title;
    public Text content;
    
    public static void Open(string title, string content)
    {
        var msgBox = UIBase.Open<MsgBox>("ArtResources/UI/Prefabs/MsgBox", UILayers.POPUP);
        msgBox.SetContent(title, content);
    }

    public void SetContent(string txt_title, string txt_content)
    {
        title.text = txt_title;
        content.text = txt_content;
    }
}