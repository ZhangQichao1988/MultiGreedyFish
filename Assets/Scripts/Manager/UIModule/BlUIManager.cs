using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 界面管理器
/// </summary>
public class BlUIManager : MonoBehaviour
{
    public static BlUIManager Instance { get; set; }
    
    public static RectTransform RootCanvasTransform
    {
        get
        {
            if(Instance != null)
            {
                return Instance._rootCanvasTransform;
            }
            return null;
        }
    }
    private RectTransform _rootCanvasTransform;

    public static Canvas RootCanvas
    {
        get
        {
            if (Instance != null)
            {
                return Instance._rootCanvas;
            }
            return null;
        }
    }
    private Canvas _rootCanvas;

    public static CanvasScaler RootCanvasScaler
    {
        get
        {
            if (Instance != null)
            {
                return Instance._rootCanvasScaler;
            }
            return null;
        }
    }
    private CanvasScaler _rootCanvasScaler;

    public static GraphicRaycaster RootGraphicRaycaster
    {
        get
        {
            if (Instance != null)
            {
                return Instance._rootGraphicRaycaster;
            }
            return null;
        }
    }
    private GraphicRaycaster _rootGraphicRaycaster;

    public static EventSystem UIEvetSystem
    {
        get
        {
            if (Instance != null)
            {
                return Instance._uiEvetSystem;
            }
            return null;
        }
    }
    private EventSystem _uiEvetSystem;

    public static StandaloneInputModule InputModule
    {
        get
        {
            if (Instance != null)
            {
                return Instance._inputModule;
            }
            return null;
        }
    }
    private StandaloneInputModule _inputModule;

    public static Camera UICamera
    {
        get
        {
            if (Instance != null)
            {
                return Instance._uiCamera;
            }
            return null;
        }
    }
    private Camera _uiCamera;

    private UISettings _uiSettings;

    private Dictionary<string, RectTransform> _layeredNodes;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        _layeredNodes = new Dictionary<string, RectTransform>();
        _uiSettings = Resources.Load<UISettings>("UISettings");

        CreateCanvas();
        CreateEventSystem();
    }

    private void CreateCanvas()
    {
        GameObject canvasGameObject = new GameObject("Canvas");
        canvasGameObject.transform.SetParent(transform, false);
        canvasGameObject.layer = LayerMask.NameToLayer("UI");

        _rootCanvasTransform = canvasGameObject.AddComponent<RectTransform>();

        _rootCanvas = canvasGameObject.AddComponent<Canvas>();
        RootCanvas.renderMode = _uiSettings.renderMode;
        RootCanvas.additionalShaderChannels |= AdditionalCanvasShaderChannels.TexCoord1;

        if (_uiSettings.renderMode == RenderMode.ScreenSpaceCamera)
        {
            CreateScreenSpaceCamera(_uiSettings.orthographic);
        }

        _rootCanvasScaler = canvasGameObject.AddComponent<CanvasScaler>();
        RootCanvasScaler.referenceResolution = _uiSettings.referenceResolution;
        RootCanvasScaler.uiScaleMode = _uiSettings.uiScaleMode;
        RootCanvasScaler.screenMatchMode = _uiSettings.screenMathMode;
        RootCanvasScaler.matchWidthOrHeight = _uiSettings.matchWidthOrHeight;

        _rootGraphicRaycaster = canvasGameObject.AddComponent<GraphicRaycaster>();
    }

    private void CreateScreenSpaceCamera(bool orthographic)
    {
        GameObject uiCamera = new GameObject("UICamera");
        uiCamera.transform.SetParent(transform, false);

        _uiCamera = uiCamera.AddComponent<Camera>();
        _uiCamera.cullingMask = 1 << LayerMask.NameToLayer("UI");
        _uiCamera.depth = 1000;
        _uiCamera.allowHDR = false;
        _uiCamera.allowMSAA = false;
        _uiCamera.clearFlags = CameraClearFlags.Depth;
        _uiCamera.orthographic = orthographic;

        RootCanvas.worldCamera = _uiCamera;
    }

    private void CreateEventSystem()
    {
        GameObject eventSystemGameObject = new GameObject("EventSystem");
        eventSystemGameObject.transform.SetParent(transform, false);

        _uiEvetSystem = eventSystemGameObject.AddComponent<EventSystem>();
        _inputModule = eventSystemGameObject.AddComponent<StandaloneInputModule>();
    }

    private void Start()
    {
        RectTransform layeredNodeHide = AddLayer("");
        layeredNodeHide.gameObject.SetActive(false);

        if (_uiSettings != null)
        {
            if (_uiSettings.layers != null)
            {
                for (int i = 0; i < _uiSettings.layers.Count; i++)
                {
                    AddLayer(_uiSettings.layers[i]);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (_uiSettings != null)
        {
            if (_uiSettings.layers != null)
            {
                for (int i = _uiSettings.layers.Count - 1; i >= 0; i--)
                {
                    RemoveLayer(_uiSettings.layers[i]);
                }
            }
        }
        RemoveLayer("");

        if (Instance != null)
        {
            Instance = null;
        }
    }

    public static int GetResoulutionX()
    {
        return (int)RootCanvasScaler.referenceResolution.x;
    }

    public static int GetResoulutionY()
    {
        return (int)RootCanvasScaler.referenceResolution.y;
    }

    public static RectTransform AddLayer(string layerName)
    {
        if(Instance != null)
        {
            RectTransform layeredNode = CreateLayer(layerName);

            layeredNode.SetParent(RootCanvasTransform, false);

            if (!Instance._layeredNodes.ContainsKey(layerName))
            {
                Instance._layeredNodes.Add(layerName, layeredNode);
            }
            return layeredNode;
        }
        return null;
    }

    private static RectTransform CreateLayer(string layerName)
    {
        GameObject layer = new GameObject(layerName);

        RectTransform layerTransform = layer.AddComponent<RectTransform>();
        layer.layer = LayerMask.NameToLayer("UI");

        layerTransform.anchorMin = Vector2.zero;
        layerTransform.anchorMax = Vector2.one;
        layerTransform.sizeDelta = Vector2.zero;

        return layerTransform;
    }

    public static void RemoveLayer(string layerName)
    {
        if (Instance != null)
        {
            RectTransform layeredNode;

            if (Instance._layeredNodes.TryGetValue(layerName, out layeredNode))
            {
                if (layeredNode != null)
                {
                    Destroy(layeredNode.gameObject);
                }
            }
        }
    }

    public static GameObject GetLayerNode(string layerName)
    {
        if (Instance != null)
        {
            RectTransform layeredNode;

            if (Instance._layeredNodes.TryGetValue(layerName, out layeredNode))
            {
                if (layeredNode != null)
                {
                    return layeredNode.gameObject;
                }
            }
        }
        return null;
    }

    public static bool AttachToLayer(GameObject uiGameObject, string layerName)
    {
        if (Instance != null)
        {
            RectTransform layeredNode;

            if (Instance._layeredNodes.TryGetValue(layerName, out layeredNode))
            {
                uiGameObject.transform.SetParent(layeredNode, false);

                return true;
            }
        }
        return false;
    }

    public static void DetachFromLayer(GameObject uiGameObject)
    {
        AttachToLayer(uiGameObject, "");
    }


    // ==========================  UIManager Lua 相关功能  ===========================
    Stack<UIPanel> listPanel = new Stack<UIPanel>();
    
    public GameObject UIRootObject;
    public GameObject UIPopupObject;
    public GameObject UILoadingObject;

    public void Init()
    {
        LoadingMgr.Init();

        UIRootObject = BlUIManager.GetLayerNode("DEFAULT");
        UIPopupObject = BlUIManager.GetLayerNode("POPUP");
        UILoadingObject = BlUIManager.GetLayerNode("LOADING");
    }

    public void PopPanel(System.Object parms = null)
    {
        var panel = listPanel.Pop();
        panel.Close();
        var currentPanel = listPanel.Peek();
        currentPanel.Show();
    }

    public void SwitchPanel(UIPanel panel, System.Object parms = null)
    {
        if (listPanel.Count > 0)
        {
            listPanel.Pop().Close();
        }
        panel.Create(UIRootObject, parms);
        listPanel.Push(panel);
    }

    public void AddPanel(UIPanel panel, System.Object parms = null, GameObject root = null)
    {
        if (root == null)
        {
            root = UIRootObject;
        }
        panel.Create(root, parms);
    }
}