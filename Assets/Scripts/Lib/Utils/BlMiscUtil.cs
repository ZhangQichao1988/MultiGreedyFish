using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public static class BlMiscUtil
{
    public static bool isIdle = false;
    
    /// <summary>
    /// 指定相机截图
    /// </summary>
    /// <returns>The by camera.</returns>
    /// <param name="mCamera">M camera.要被截屏的相机</param>
    /// <param name="mRect">M rect. 截屏的区域</param>
    /// <param name="mFileName">M file name.</param>
    public static Texture2D CaptureByCamera(Camera mCamera, Rect mRect, GameObject goImage)
    {
        //Oz.Framework.UIModule.UIManager.RootCanvasTransform.sizeDelta = new Vector2(1920, 1080);
        //Camera mCamera = goCamera.GetComponent<Camera>();
        RawImage rawImage = goImage != null ? goImage.GetComponent<RawImage>() : null;  
        //等待渲染线程结束
        //yield return new WaitForEndOfFrame();
        //初始化RenderTexture   深度只能是【0、16、24】截不全图请修改
        RenderTexture mRender = new RenderTexture(Screen.width, Screen.height, 16);
        //设置相机的渲染目标
        mCamera.targetTexture = mRender;
        //开始渲染
        mCamera.Render();
        //激活渲染贴图读取信息
        RenderTexture.active = mRender;
        Texture2D mTexture = new Texture2D((int)mRect.width, (int)mRect.height, TextureFormat.RGB24, false);
        //读取屏幕像素信息并存储为纹理数据
        mTexture.ReadPixels(mRect, 0, 0);
        //应用
        mTexture.Apply();
        //释放相机，销毁渲染贴图
        mCamera.targetTexture = null;
        RenderTexture.active = null;
        GameObject.Destroy(mRender);
        //将图片信息编码为字节信息
        //byte[] bytes = mTexture.EncodeToPNG();
        //保存
        //System.IO.File.WriteAllBytes(mFileName, bytes);
        //需要展示次截图，可以返回截图
        //return mTexture;
        if (rawImage != null)
        {
            rawImage.texture = mTexture;
        }
        return mTexture;
    }

    /// <summary>
    /// 屏幕宽高比
    /// </summary>
    /// <returns></returns>
    public static float GetScreenAspect()
    {
        return (float)Screen.width / Screen.height;
    }


    /// <summary>
    /// enum转换
    /// </summary>
    /// <param name="number">lua中定义的number</param>
    public static UnityEngine.Playables.DirectorUpdateMode EnumDirectorUpdateMode(int number)
    {
        return (UnityEngine.Playables.DirectorUpdateMode)number;
    }

    public static BlRNGUtil GenRNG(uint seed)
    {
        BlRNGUtil r = new BlRNGUtil(seed);
        return r;
    }
}
