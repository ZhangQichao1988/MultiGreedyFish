using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public static class UIBehaviourUtil
{
	//===UIBehaviour===

	

	//===Graphic.rectTransform===

	/// <summary>
	/// 
	/// </summary>
	/// <param name="graphic"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void SetRectTransformOffsetMax(Graphic graphic, float x, float y)
	{
		graphic.rectTransform.offsetMax = new Vector2(x, y);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="graphic"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void SetRectTransformOffsetMin(Graphic graphic, float x, float y)
	{
		graphic.rectTransform.offsetMin = new Vector2(x, y);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="graphic"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void SetRectTransformPivot(Graphic graphic, float x, float y)
	{
		graphic.rectTransform.pivot = new Vector2(x, y);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="graphic"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void SetRectTransformSizeDelta(Graphic graphic, float x, float y)
	{
		graphic.rectTransform.sizeDelta = new Vector2(x, y);
	}

    public static int GetRectTransformSizeDeltaWidth(Graphic graphic)
    {
        return (int)graphic.rectTransform.sizeDelta.x;
    }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="graphic"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void SetRectTransformAnchoredPosition(Graphic graphic, float x, float y)
	{
		graphic.rectTransform.anchoredPosition = new Vector2(x, y);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="graphic"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="z"></param>
	public static void SetRectTransformAnchoredPosition3D(Graphic graphic, float x, float y, float z)
	{
		graphic.rectTransform.anchoredPosition3D = new Vector3(x, y, z);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="graphic"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void SetRectTransformAnchorMax(Graphic graphic, float x, float y)
	{
		graphic.rectTransform.anchorMax = new Vector2(x, y);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="graphic"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public static void SetRectTransformAnchorMin(Graphic graphic, float x, float y)
	{
		graphic.rectTransform.anchorMin = new Vector2(x, y);
	}


	//===Graphic===

	/// <summary>
	/// 设置半透明
	/// </summary>
	/// <param name="graphic"></param>
	/// <param name="alpha"></param>
	public static void SetGraphicAlpha(Graphic graphic, float alpha)
	{
		Color color = graphic.color;
		color.a = alpha;
		graphic.color = color;
	}

    //===CvansGroup===
    /// <summary>
	/// 设置组半透明
	/// </summary>
	/// <param name="canvasGroup"></param>
	/// <param name="alpha"></param>
    public static void SetCanvasGroupAlpha(CanvasGroup canvasGroup, float alpha)
    {
        canvasGroup.alpha = alpha;
    }
 
	/// <summary>
	/// 设置颜色
	/// </summary>
	/// <param name="graphic"></param>
	/// <param name="r"></param>
	/// <param name="g"></param>
	/// <param name="b"></param>
	/// <param name="a"></param>
	public static void SetGraphicColorRGBA(Graphic graphic, float r, float g, float b, float a)
	{
		graphic.color = new Color(r, g, b, a);
	}

	/// <summary>
	/// 设置颜色
	/// </summary>
	/// <param name="graphic"></param>
	/// <param name="r"></param>
	/// <param name="g"></param>
	/// <param name="b"></param>
	public static void SetGraphicColorRGB(Graphic graphic, float r, float g, float b)
	{
		graphic.color = new Color(r, g, b);
	}

	//===Text===

	/// <summary>
	/// 设置Text组件文字
	/// </summary>
	/// <param name="text"></param>
	/// <param name="content"></param>
	public static void SetTextString(Text text, string content)
	{
		text.text = content;
	}

	/// <summary>
	/// 设置text
	/// </summary>
	/// <param name="text"></param>
	/// <param name="anchor"></param>
	public static void SetTextAlignment(Text text, int anchor)
	{
		text.alignment = (TextAnchor)anchor;
	}

	//===Image===

	/// <summary>
	/// 设置一个Sprite到Image上
	/// </summary>
	/// <param name="image"></param>
	/// <param name="sprite"></param>
	public static void SetImageSprite(Image image, Sprite sprite)
	{
		if(image != null)
		{
			image.sprite = sprite;
		}
	}

    public static void SetImageOverrideSprite(Image image, Sprite sprite)
    {
        if (image != null)
        {
            image.overrideSprite = sprite;
        }
    }

    /// <summary>
    /// 设置一个Sprite到Image上
    /// </summary>
    /// <param name="image"></param>
    /// <param name="spritePath"></param>
    public static void SetImageSprite(Image image, string spritePath)
	{
		Sprite sprite = ResourceManager.LoadSync<Sprite>(spritePath);

		if(sprite != null)
		{
			SetImageSprite(image, sprite);
		}
	}

    public static void SetImageOverrideSprite(Image image, string spritePath)
    {
        Sprite sprite = ResourceManager.LoadSync<Sprite>(spritePath);

        if (sprite != null)
        {
            SetImageOverrideSprite(image, sprite);
        }
    }

    /// <summary>
    /// 设置一个Sprite到Image上
    /// </summary>
    /// <param name="image"></param>
    /// <param name="sprite"></param>
    /// <param name="bNativeSize"></param>
    public static void SetImageSprite(Image image, Sprite sprite, bool bNativeSize)
	{
		image.sprite = sprite;

		if(bNativeSize)
		{
			image.SetNativeSize();
		}	
	}

    public static void SetImageOverrideSprite(Image image, Sprite sprite, bool bNativeSize)
    {
        image.overrideSprite = sprite;

        if (bNativeSize)
        {
            image.SetNativeSize();
        }
    }

    public static void SetImageSprite(Image image, string spritePath, bool bNativeSize)
	{
		SetImageSprite(image, spritePath);

		if (bNativeSize)
		{
			image.SetNativeSize();
		}
	}

    public static void SetImageOverrideSprite(Image image, string spritePath, bool bNativeSize)
    {
        SetImageOverrideSprite(image, spritePath);

        if (bNativeSize)
        {
            image.SetNativeSize();
        }
    }

    public static void SetImageType(Image image, int type)
    {
        image.type = (Image.Type)type;
    }
    
    /// <summary>
    /// 设置Image的材质
    /// </summary>
    /// <param name="image"></param>
    /// <param name="material"></param>
    public static void SetImageMaterial(Image image, string materialPath)
    {
        Material material = ResourceManager.LoadSync<Material>(materialPath);
        if (material != null)
        {
            image.material = material;
        }
        else
        {
            image.material = null;
        }
    }

    public static void SetMaterialGray(Material material, int isGray)
    {
        if (isGray == 1)
        {
            material.SetFloat("_GrayScale", 0);
        }
        else
        {
            material.SetFloat("_GrayScale", 1);
        }
    }

    public static void SetImageGray(Image image, int isGray)
    {
        if (isGray == 1)
        {
            Material mat = ResourceManager.LoadSync<Material>("UI/Materials/GrayMat");
            image.material = mat;
        }
        else
        {
            image.material = null;
        }
    }

    //===RawImage===

    /// <summary>
    /// 设置RawImage的Texture；
    /// </summary>
    /// <param name="rawImage"></param>
    /// <param name="texutre"></param>
    /// <param name="texturePath"></param>
    /// <param name="bNativeSize"></param>

    public static void SetRawImageTexture(RawImage rawImage, Texture texture)
	{
		rawImage.texture = texture;
	}

    public static void SetRawImageTexture(RawImage rawImage, string texturePath)
    {
        Texture texture = ResourceManager.LoadSync<Texture>(texturePath);
        if (texture != null)
        {
            SetRawImageTexture(rawImage, texture);
        }
    }

    public static void SetRawImageTexture(RawImage rawImage, Texture texture, bool bNativeSize)
    {
        rawImage.texture = texture;
        if (bNativeSize)
        {
            rawImage.SetNativeSize();
        }
    }

    public static void SetRawImageTexture(RawImage rawImage, string texturePath, bool bNativeSize)
    {
        SetRawImageTexture(rawImage, texturePath);
        if (bNativeSize)
        {
            rawImage.SetNativeSize();
        }
    }

    public static void SetRawImageMaterial(RawImage rawImage, string materialPath)
    {
        Material material = ResourceManager.LoadSync<Material>(materialPath);
        if (material != null)
        {
            rawImage.material = material;
        }
    }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="rawImage"></param>
	/// <param name="texutrePath"></param>
	//public static void SetRawImageTextureAsync(RawImage rawImage, string texutrePath)
	//{
	//	ResourceManager.LoadAsync<Texture>(texutrePath, (handle, callbackObject, sprite) =>
	//	{
	//		SetRawImageTexture(rawImage, sprite);
	//	});
	//}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="rawImage"></param>
	/// <param name="texutrePath"></param>
	/// <param name="bNativeSize"></param>
	//public static void SetRawImageTextureAsync(RawImage rawImage, string texutrePath, bool bNativeSize)
	//{
 //       ResourceManager.LoadAsync<Texture>(texutrePath, (handle, callbackObject, sprite) =>
	//	{
	//		SetRawImageTexture(rawImage, sprite, bNativeSize);
	//	});
	//}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="rawImage"></param>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	public static void SetRawImageUVRect(RawImage rawImage, float x, float y, float width, float height)
	{
		rawImage.uvRect = new Rect(x, y, width, height);
	}

	//===Selectable===

	/// <summary>
	/// 
	/// </summary>
	/// <param name="selectable"></param>
	/// <param name="interactable"></param>
	public static void SetInteractable(Selectable selectable, bool interactable)
	{
		selectable.interactable = interactable;
	}

	//===Button===

	/// <summary>
	/// 设置按钮点击事件
	/// </summary>
	/// <param name="button"></param>
	/// <param name="onClick"></param>
	public static void SetButtonOnClickedEvent(Button button, Button.ButtonClickedEvent onClick)
	{
		button.onClick = onClick;
	}

	/// <summary>
	/// 为按钮点击事件添加监听器
	/// </summary>
	/// <param name="button"></param>
	/// <param name="action"></param>
	public static void AddButtonOnClickListener(Button button, UnityAction action)
	{
		button.onClick.AddListener(action);
	}

	/// <summary>
	/// 为按钮点击事件移除监听器
	/// </summary>
	/// <param name="button"></param>
	/// <param name="action"></param>
	public static void RemoveButtonOnClickListener(Button button, UnityAction action)
	{
		button.onClick.RemoveListener(action);
	}

	/// <summary>
	/// 为按钮点击事件移除所有监听器
	/// </summary>
	/// <param name="button"></param>
	public static void RemoveAllButtonOnClickListener(Button button)
	{
		button.onClick.RemoveAllListeners();
	}

	//===Toggle===

	
	public static void SetToggle(Toggle toggle)
	{

	}

	//===Slider===

	
	public static void SetSlider(Slider slider)
	{

	}

	//===Scrollbar===

	
	public static void SetScrollbar(Scrollbar scrollbar)
	{

	}

	//===Dropdown===

	
	public static void SetDropdown(Dropdown dropdown)
	{

	}

	//===InputField===

	
	public static void SetInputFieldText(InputField inputField, string content)
	{
		inputField.text = content;
	}

	//===ScrollRect===

    //设置button亮度；
    public static void SetButtonBright(GameObject obj, float num)
    {
        Button btn = obj.GetComponent<Button>();
        btn.transition = Selectable.Transition.ColorTint;

        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(num/255,num/255,num/255,1);
        cb.highlightedColor = cb.normalColor;
        cb.pressedColor = cb.pressedColor;
        cb.disabledColor = cb.disabledColor;

        btn.colors = cb;

    }

    public static Vector2 ScreenPointToLocalPoint(GameObject obj, Vector2 screenPoint)
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((obj.transform as RectTransform), screenPoint, BlUIManager.UICamera, out pos);
        return pos;
    }

    /// <summary>
    /// 获取Tips的自适应坐标，保证Tips不会超出屏幕，且自动进行全局到本地的坐标转换
    /// </summary>
    /// <param name="tipsLayer">Tips所在的层级对象</param>
    /// <param name="screenClickPoint">屏幕空间的触摸点</param>
    /// <param name="tips">Tips对象</param>
    /// <param name="offset">距离触摸点的偏移量(正数往上偏移，负数往下偏移)</param>
    /// <returns>返回一个位于 tipsLayer 空间的坐标点</returns>
    public static Vector2 GetTipsPos(GameObject tipsLayer, Vector2 screenClickPoint, GameObject tips, float offset = 200)
    {
        //首先更新需要动态缩放的Tips的尺寸
        ContentSizeFitter csf = tips.GetComponent<ContentSizeFitter>();
        if(csf != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(tips.transform as RectTransform);
        }

        Vector2 newScreenPoint = screenClickPoint;
        RectTransform layerRt = tipsLayer.transform as RectTransform;
        RectTransform tipsRt = tips.transform as RectTransform;

        newScreenPoint += Vector2.up * offset;

        float tipsWidth = tipsRt.sizeDelta.x;
        float tipsHeight = tipsRt.sizeDelta.y;
        float leftX = newScreenPoint.x - tipsRt.pivot.x * tipsWidth;
        float rightX = leftX + tipsWidth;
        float upY = newScreenPoint.y + (1 - tipsRt.pivot.y) * tipsHeight;
        float downY = upY - tipsHeight;

        if (leftX < 0)
        {
            newScreenPoint.x = tipsWidth * tipsRt.pivot.x;
        }
        else if(rightX > BlUIManager.RootCanvas.pixelRect.width)
        {
            newScreenPoint.x = (BlUIManager.RootCanvas.pixelRect.width - tipsWidth) + tipsWidth * tipsRt.pivot.x;
        }

        if (downY < 0)
        {
            newScreenPoint.y = tipsHeight * tipsRt.pivot.y;
        }
        else if(upY > BlUIManager.RootCanvas.pixelRect.height)
        {
            newScreenPoint.y = (BlUIManager.RootCanvas.pixelRect.height - tipsHeight) + tipsHeight * tipsRt.pivot.y;
        }

        Vector2 tipsPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(layerRt, newScreenPoint, BlUIManager.UICamera, out tipsPos);
        return tipsPos;
    }

    public static void SetLongPressTime(GameObject go, float pTime)
    {
        PointerLongPressTrigger pointerLongPress = go.GetComponent<PointerLongPressTrigger>();
        if (pointerLongPress != null)
        {
            pointerLongPress.StartAfter = pTime;
        }
    }
}
