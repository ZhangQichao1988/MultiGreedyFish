using Oz.Framework;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Gameobject Helper
/// </summary>
public static class GameObjectUtil
{
    /// <summary>
    /// 获取游戏对象上的组件
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="componentType"></param>
    /// <returns></returns>
    public static Component GetComponent(GameObject gameObject, string componentType)
    {
        return gameObject.GetComponent(componentType);
    }


    public static Component GetComponentInChildren(GameObject gameObject, string componentType)
    {
        System.Type type = BlTypeUtil.GetType(componentType);
        Component comp = gameObject.GetComponentInChildren(type);

        return comp;
    }

    /// <summary>
    /// 为游戏对象添加组件
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="componentType"></param>
    /// <returns></returns>
    public static Component AddComponent(GameObject gameObject, string componentType)
    {
        System.Type type = BlTypeUtil.GetType(componentType);
        return gameObject.AddComponent(type);
    }

    /// <summary>
    /// 获取游戏对象组件如果不存在则添加该组件
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="componentType"></param>
    /// <returns></returns>
    public static Component AddMissingComponent(GameObject gameObject, string componentType)
    {
        Component component = GetComponent(gameObject, componentType);

        if (!component)
        {
            component = AddComponent(gameObject, componentType);
        }
        return component;
    }

    /// <summary>
    /// 实例化预制
    /// </summary>
    /// <param name="prefab">预制</param>
    /// <param name="parent">父节点对象</param>
    /// <returns>返回实例化出的游戏对象</returns>
    public static GameObject InstantiatePrefab(GameObject prefab, GameObject parent, bool resetTransform = true)
    {
        GameObject gameObject = Object.Instantiate(prefab);
        AttachGameObject(gameObject, parent, resetTransform);
         
        return gameObject;
    }

    public static GameObject InstantiatePrefabByCenter(GameObject prefab, GameObject parent)
    {
        GameObject gameObject = Object.Instantiate(prefab);
        AttachGameObjectByCenter(gameObject, parent);

        return gameObject;
    }

    /// <summary>
    /// 实例化界面预制
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject InstantiateUIPrefab(GameObject prefab, GameObject parent)
    {
        GameObject gameObject = Object.Instantiate(prefab);
        gameObject.transform.SetParent(parent != null ? parent.transform : null, false);

        return gameObject;
    }

    public static void ResetTransform(GameObject go, bool ignoreScale = false)
    {
        go.transform.localPosition = Vector3.zero;
        go.transform.localRotation = Quaternion.identity;
        if(!ignoreScale)
            go.transform.localScale = Vector3.one;
    }

    public static void AttachUIGameObject(GameObject gameObject, GameObject parent)
    {
        gameObject.transform.SetParent(parent != null ? parent.transform : null, false);
    }

    /// <summary>
    /// 设置父节点
    /// </summary>
    /// <param name="gameObject">游戏对象</param>
    /// <param name="parent">父节点对象</param>
    public static void AttachGameObject(GameObject gameObject, GameObject parent, bool resetTransform = false)
    {
        gameObject.transform.SetParent(parent != null ? parent.transform : null, false);

        if(resetTransform)
        {
            ResetTransform(gameObject);
        }     
    }

    public static void AttachGameObjectByCenter(GameObject go, GameObject parent)
    {
        //GameObject go = Instantiate(prefab);
        go.transform.SetParent(parent.transform, false);
        Renderer[] rdrs = go.GetComponentsInChildren<Renderer>();
        Vector3 offset = Vector3.zero;
        foreach (Renderer rdr in rdrs)
        {
            offset += rdr.bounds.center;
        }
        offset /= rdrs.Length;
        go.transform.position -= offset;
    }

    /// <summary>
    /// 新建一个空游戏对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns>返回新建的游戏对象</returns>
    public static GameObject NewGameObject(string name, GameObject parent)
    {
        GameObject gameObject = new GameObject(name);
        AttachGameObject(gameObject, parent, true);

        return gameObject;
    }

    /// <summary>
    /// 新建一个RectTransform空游戏对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static GameObject NewGameObjectUI(string name, GameObject parent)
    {
        GameObject gameObject = new GameObject(name, new System.Type[] { typeof(RectTransform) });
        AttachGameObject(gameObject, parent, true);

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.offsetMin = Vector2.zero;

        return gameObject;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObjet"></param>
    /// <param name="avtive"></param>
    public static void SetActive(GameObject gameObject, bool avtive)
    {
        if (gameObject.activeSelf != avtive)
        {
            gameObject.SetActive(avtive);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="component"></param>
    /// <param name="avtive"></param>
    public static void SetActive(Component component, bool avtive)
    {
        SetActive(component.gameObject, avtive);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static bool IsActiveSelf(GameObject gameObject)
    {
        return gameObject.activeSelf;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static bool IsActiveInHierarchy(GameObject gameObject)
    {
        return gameObject.activeInHierarchy;
    }

    /// <summary>
    /// 获取游戏对象的哈希码
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static string GetGameObjectHashCode(GameObject gameObject)
    {
        if (gameObject != null)
        {
            return gameObject.GetHashCode().ToString();
        }
        return string.Empty;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static string GetGameObjectPath(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return string.Empty;
        }

        Transform curt = gameObject.transform;

        string ret = curt.name + "/";
        while (curt.parent != null)
        {
            ret = curt.parent.name + "/" + ret;
            curt = curt.parent;
        }

        return ret;
    }

    /// <summary>
    /// 查找指定路径下的节点的组件
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="childPath"></param>
    /// <param name="componentType"></param>
    /// <returns></returns>
    public static Component FindChildComponent(GameObject gameObject, string childPath, string componentType)
    {
        Transform transform = gameObject.transform.Find(childPath);

        if (transform != null)
        {
            return transform.GetComponent(componentType);
        }
        return null;
    }

    /// <summary>
    /// 查找指定路径下的节点
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="childPath"></param>
    /// <returns></returns>
    public static GameObject FindChildGameObject(GameObject gameObject, string childPath)
    {
        Transform transform = gameObject.transform.Find(childPath);

        if (transform != null)
        {
            return transform.gameObject;
        }
        return null;
    }

    /// <summary>
    /// 查找指定名字的子节点对象
    /// </summary>
    /// <param name="name">对象名</param>
    /// <param name="parent">查找节点</param>
    /// <returns>返回第一个名字符合的对象，如果找不到返回null</returns>
    public static GameObject FindGameObjectByName(string name, GameObject parent)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();

        for (int i = 0; i < children.Length; i++)
        {
            Transform t = children[i];

            if (name.Equals(t.name))

                return t.gameObject;
        }
        return null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    public static void DestroyAllChildren(GameObject gameObject)
    {
        if (gameObject != null)
        {
            Transform t = gameObject.transform;

            int childCount = t.childCount;

            for (int i = 0; i < childCount; ++i)
            {
                Object.Destroy(t.GetChild(i).gameObject);
            }
        }
    }

    /// <summary>
    /// 设置游戏对象以及子对象层
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="layer"></param>
    public static void SetGameObjectLayer(GameObject gameObject, int layer)
    {
        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();

        for (int i = 0; i < transforms.Length; i++)
        {
            Transform t = transforms[i];

            t.gameObject.layer = layer;
        }
    }

    /// <summary>
    /// 设置游戏对象世界坐标
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetTransformPosition(GameObject gameObject, float x, float y, float z)
    {
        gameObject.transform.position = new Vector3(x, y, z);
    }

    /// <summary>
    /// 获取游戏对象世界坐标
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void GetTransformPosition(GameObject gameObject, out float x, out float y, out float z)
    {
        Vector3 position = gameObject.transform.position;
        x = position.x;
        y = position.y;
        z = position.z;
    }

    /// <summary>
    /// 设置游戏对象自身坐标
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetTransformLocalPosition(GameObject gameObject, float x, float y, float z)
    {
        gameObject.transform.localPosition = new Vector3(x, y, z);
    }

    public static Vector2 GetUIPosForCameraMode(Camera sceneCamera, Canvas canvas, Vector3 worldPos)
    {
        Vector2 uiPos = Vector3.zero;

        if (sceneCamera == null || canvas == null)
        {
            return uiPos;
        }

        Vector3 screenPos = sceneCamera.WorldToScreenPoint(worldPos);

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out uiPos);
        
        return uiPos;
    }

    public static int GetScreenPosX(GameObject g)
    {
        //if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //  cvs.transform as RectTransform, Camera.main.WorldToScreenPoint(transform.position),
        //  cvs.worldCamera, out local))
        //{
        //    Debug.Log(local);
        //}
        
        int x = (int)BlUIManager.UICamera.WorldToScreenPoint(g.transform.position).x;
        return x;
    }

    public static int GetScreenPosY(GameObject g)
    {
        int y = (int)BlUIManager.UICamera.WorldToScreenPoint(g.transform.position).y;
        return y;
    }

    /// <summary>
    /// 获取游戏对象自身坐标
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void GetTransformLocalPosition(GameObject gameObject, out float x, out float y, out float z)
    {
        Vector3 position = gameObject.transform.localPosition;
        x = position.x;
        y = position.y;
        z = position.z;
    }

    /// <summary>
    /// 设置游戏对象旋转
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetTransformRotation(GameObject gameObject, float x, float y, float z)
    {
        gameObject.transform.rotation = Quaternion.Euler(x, y, z);
    }

    /// <summary>
    /// 获取游戏对象旋转欧拉角
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void GetTransformRotation(GameObject gameObject, out float x, out float y, out float z)
    {
        Vector3 euler = gameObject.transform.rotation.eulerAngles;
        x = euler.x;
        y = euler.y;
        z = euler.z;
    }

    /// <summary>
    /// 设置游戏对象自身旋转
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetTransformLocalRotation(GameObject gameObject, float x, float y, float z)
    {
        gameObject.transform.localRotation = Quaternion.Euler(x, y, z);
    }


    /// <summary>
    /// 获取游戏对象自身旋转欧拉角
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void GetTransformLocalRotation(GameObject gameObject, out float x, out float y, out float z)
    {
        Vector3 euler = gameObject.transform.localRotation.eulerAngles;
        x = euler.x;
        y = euler.y;
        z = euler.z;
    }

    /// <summary>
    /// 设置游戏对象自身缩放
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="scale"></param>
    public static void SetTransformLocalScale(GameObject gameObject, float scale)
    {
        gameObject.transform.localScale = Vector3.one * scale;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void GetTransformLossyScale(GameObject gameObject, out float x, out float y, out float z)
    {
        Vector3 scale = gameObject.transform.lossyScale;
        x = scale.x;
        y = scale.y;
        z = scale.z;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void GetTransformLocalScale(GameObject gameObject, out float x, out float y, out float z)
    {
        Vector3 scale = gameObject.transform.localScale;
        x = scale.x;
        y = scale.y;
        z = scale.z;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SetRectTransformOffsetMax(GameObject gameObject, float x, float y)
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;

        if (rectTransform != null)
        {
            rectTransform.offsetMax = new Vector2(x, y);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SetRectTransformOffsetMin(GameObject gameObject, float x, float y)
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;

        if (rectTransform != null)
        {
            rectTransform.offsetMin = new Vector2(x, y);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SetRectTransformPivot(GameObject gameObject, float x, float y)
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;

        if (rectTransform != null)
        {
            rectTransform.pivot = new Vector2(x, y);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SetRectTransformSizeDelta(GameObject gameObject, float x, float y)
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;

        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(x, y);
        }
    }

    public static int GetRectTransformSizeDeltaWidth(GameObject gameObject)
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;
        return (int)rectTransform.sizeDelta.x;
    }

    public static int GetRectTransformSizeDeltaHeight(GameObject gameObject)
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;
        return (int)rectTransform.sizeDelta.y;
    }

    public static int GetRectTransformWidth(GameObject go)
    {
        RectTransform rectTransform = go.transform as RectTransform;
        return (int)rectTransform.rect.width;
    }

    public static int GetRectTransformHeight(GameObject go)
    {
        RectTransform rectTransform = go.transform as RectTransform;
        return (int)rectTransform.rect.height;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SetRectTransformAnchoredPosition(GameObject gameObject, float x, float y)
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetRectTransformAnchoredPosition3D(GameObject gameObject, float x, float y, float z)
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition3D = new Vector3(x, y, z);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SetRectTransformAnchorMax(GameObject gameObject, float x, float y)
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;

        if (rectTransform != null)
        {
            rectTransform.anchorMax = new Vector2(x, y);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SetRectTransformAnchorMin(GameObject gameObject, float x, float y)
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;

        if (rectTransform != null)
        {
            rectTransform.anchorMin = new Vector2(x, y);
        }
    }

    public static void SetRectTransformRectHeight(GameObject gameObject, float h)
    {
        RectTransform rectTransform = gameObject.transform as RectTransform;

        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, h);
        }
    }

    public static void SetScrollPosInt(GameObject go)
    {
        ScrollRect rect = go.GetComponent<ScrollRect>();
        rect.verticalNormalizedPosition = 1;

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="sprite"></param>
    public static void SetSpriteRendererSprite(GameObject gameObject, Sprite sprite)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    public static void SetLightColor(GameObject gameObject, float r, float g, float b)
    {
        Light light = gameObject.GetComponent<Light>();

        if (light != null)
        {
            light.color = new Color(r, g, b);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="intensity"></param>
    public static void SetLightIntensity(GameObject gameObject, float intensity)
    {
        Light light = gameObject.GetComponent<Light>();

        if (light != null)
        {
            light.intensity = intensity;
        }
    }
}
