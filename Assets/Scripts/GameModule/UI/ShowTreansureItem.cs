using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowTreansureItem : MonoBehaviour
{
    public Image imageItem;
    public Text textCount;

    Animator animator;
    public void Init(string imgPath, int count)
    {
        animator = GetComponent<Animator>();
        animator.enabled = false;

        imageItem.sprite = ResourceManager.LoadSync<Sprite>(AssetPathConst.itemIconPath + imgPath).Asset;
        textCount.text = "x" + count;
    }
    public void Show()
    {
        animator.enabled = true;
    }
}
