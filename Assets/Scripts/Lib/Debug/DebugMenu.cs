using UnityEngine;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;

public class DebugMenu :MonoBehaviour
{

    /// <summary>
    /// The size of the log.
    /// </summary>
    const int LogSize = 200;
    
    const float PressThreshold = 0.1f;
    enum DebugMenuStatus
    {
        Hidden,
        FPSMode,
        FrameTimeMode,
        MonoMemoryMode,
        LogMode,
    }
    enum ButtonStatus
    {
        Free,
        Released,
        Pressed,
        Hold
    }
    float defaultWidth;
    Material material;
    float lowMemory;

    float highMemory;

    float defaultHeight;

    float lastFPSUpdated;
    const int GraphLogSize = 64;
    ButtonStatus buttonStatus;
    float startPressTime;

    float pixelPerCentimeter;
    float[] memoryArray;
    public static DebugMenu instance = null;
    DebugMenuStatus debugMenuStatus = DebugMenuStatus.FPSMode;
    UnityEngine.EventSystems.EventSystem currentEventSystem;
    Texture2D texture;

    float[] deltaTimeArray;
    int averageFPS = 0;
    const float updateFPSinterval = 1f;
    public int MaxGraphFPS = 60;
    float[] fpsRateArray;

    /// <summary>
    /// The window rect.
    /// </summary>
    Rect windowRect;
    int toolbarInt;
    Vector2 pos;

    string tmpVersion;
    string[] toolbarStrings = new string[] { "Endpoint", "Log" };

    /// <summary>
    /// The reports.
    /// </summary>
    Queue<LoggerReport> reports = new Queue<LoggerReport>();


    private int currentServerIdx;
    private int selectedServerIdx;

    public static DebugMenu Instance
    {
        get
        {
            // まだ生成されておらず、シーン上にも設置されていない場合は生成.
            if (instance == null)
            {
                GameObject gameObject = new GameObject(typeof(DebugMenu).ToString());
                instance = gameObject.AddComponent<DebugMenu>();
                DontDestroyOnLoad(gameObject);
            }

            return instance;
        }
    }

    public void StartDebug()
    {

    }

    void Awake()
    {

        // ハンドルされていない例外を受け取ったらLogReportを追加する
        Application.logMessageReceived += (condition, stackTrace, type) =>
        {
            switch (type)
            {
                case LogType.Exception:
                    HandleLog(new LoggerReport(
                        LogLevel.All,
                        "Exception",
                        string.Concat(condition, "\n", stackTrace)
                    ));
                    break;
                case LogType.Warning:
                    HandleLog(new LoggerReport(
                        LogLevel.Warn,
                        "Warning",
                        string.Concat(condition, "\n", stackTrace)
                    ));
                    break;
                case LogType.Log:
                    HandleLog(new LoggerReport(
                        LogLevel.Debug,
                        "Debug",
                        string.Concat(condition, "\n", stackTrace)
                    ));
                    break;
            }
        };


        // 画面の初期pixelを保持
        defaultHeight = Screen.height;
        defaultWidth = Screen.width;

        // １センチあたりのピクセル数
        pixelPerCentimeter = Screen.dpi / 2.54f * ((float) Screen.height / defaultHeight);

        windowRect = new Rect();

        // グラフ用テクスチャ生成 サイズ要検討
        texture = Texture2D.whiteTexture;
        fpsRateArray = new float[GraphLogSize];
        deltaTimeArray = new float[GraphLogSize];
        memoryArray = new float[GraphLogSize];
        texture.wrapMode = TextureWrapMode.Clamp;

        material = new Material(Shader.Find("Custom/Debug"));

        string[] list = Enum.GetNames(typeof(ESeverType));
        currentServerIdx = Array.IndexOf(list, AppConst.ServerType.ToString());
        selectedServerIdx = currentServerIdx;
    }
    

    /// <summary>
    /// レジュームしたら見えるようになる
    /// </summary>
    /// <param name="pause">If set to <c>true</c> pause.</param>
    void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            debugMenuStatus = DebugMenuStatus.Hidden;
        }
        else
        {
            windowRect.x = windowRect.y = 0;
            windowRect.width = windowRect.height = pixelPerCentimeter;
            debugMenuStatus = DebugMenuStatus.FPSMode;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && (Input.GetKey(KeyCode.LeftWindows) || Input.GetKey(KeyCode.RightWindows) || 
            Input.GetKey(KeyCode.LeftApple) || Input.GetKey(KeyCode.RightApple)))
        {
            Intro.Instance.Restart();
            return;
        }

        // 完全に隠す場合は何もしない
        if (debugMenuStatus == DebugMenuStatus.Hidden)
        {
            return;
        }

        // FPS取得
        Array.Copy(fpsRateArray, 1, fpsRateArray, 0, fpsRateArray.Length - 1);

        var fFPS = (float) MaxGraphFPS;
        var x = Mathf.Max(0f, fFPS - (1f / Time.deltaTime)) / fFPS;
        fpsRateArray[fpsRateArray.Length - 1] = x * x;
         
        // 差分時間取得
        Array.Copy(deltaTimeArray, 1, deltaTimeArray, 0, deltaTimeArray.Length - 1);
        deltaTimeArray[deltaTimeArray.Length - 1] = Time.deltaTime;

        // Monoメモリ使用量取得
        Array.Copy(memoryArray, 1, memoryArray, 0, memoryArray.Length - 1);
        memoryArray[memoryArray.Length - 1] = System.GC.GetTotalMemory(false) / (1024f * 1024f);

        // 低メモリ保持
        if (lowMemory == 0f)
        {
            lowMemory = memoryArray[memoryArray.Length - 1];
        }

        // 下限上限更新
        lowMemory = Mathf.Min(lowMemory, memoryArray[memoryArray.Length - 1]);
        highMemory = Mathf.Max(highMemory, memoryArray[memoryArray.Length - 1]);
    }

    void OnGUI()
    {
        // 完全に隠す場合は何もしない
        if (debugMenuStatus != DebugMenuStatus.Hidden)
        {
            // デバッグウィンドウ表示
            windowRect = GUI.Window(0, windowRect, Window, string.Empty, GUI.skin.box);
        }
        else
        {
            if (UnityEngine.EventSystems.EventSystem.current != null)
            {
                currentEventSystem = UnityEngine.EventSystems.EventSystem.current;
            }

            if (currentEventSystem != null)
            {
                currentEventSystem.enabled = true;
            }
        }

    }

    void Window(int windowID)
    {
        // 最新の１センチあたりのピクセル数を取得
        // 縦横で基準切り替え
        pixelPerCentimeter = Screen.dpi / 2.54f * ((float) Screen.height / (Screen.width > Screen.height ? defaultHeight : defaultWidth));

        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            currentEventSystem = UnityEngine.EventSystems.EventSystem.current;
        }

        // タップ判定更新
        var isTouch = (Event.current.type == EventType.Used
                      || Event.current.type == EventType.MouseDown
                      || Event.current.type == EventType.MouseDrag
                      || Event.current.type == EventType.MouseUp);

        // 状態遷移
        switch (debugMenuStatus)
        {
            case DebugMenuStatus.FPSMode:
            case DebugMenuStatus.FrameTimeMode:
            case DebugMenuStatus.MonoMemoryMode:
                // 簡易表示モード
                ShowLiteWindow();
                break;
            default:
                // 詳細表示モード
                ShowMainWindow();
                break;
        }

        // 判定無効チェック
        var onMouse = new Rect(0, 0, windowRect.width, windowRect.height).Contains(Event.current.mousePosition);
        if (isTouch && onMouse && debugMenuStatus != DebugMenuStatus.Hidden)
        {
            if (currentEventSystem != null)
            {
                currentEventSystem.enabled = false;
            }
        }
        else if(!onMouse)
        {
            if (currentEventSystem != null)
            {
                currentEventSystem.enabled = true;
            }
        }

        // ドラッグ範囲全指定
        GUI.DragWindow();
    }

    void ShowLiteWindow()
    {
        // windowサイズを１センチに指定
        windowRect.width = pixelPerCentimeter * 2f;
        windowRect.height = pixelPerCentimeter;

        if (new Rect(0, 0, windowRect.width, windowRect.height).Contains(Event.current.mousePosition))
        {
            if (Event.current.type == EventType.MouseDown)
            {
                startPressTime = Time.time;
                buttonStatus = ButtonStatus.Pressed;
            }
        }

        if (Time.time - startPressTime > PressThreshold && buttonStatus >= ButtonStatus.Pressed && Event.current.type == EventType.MouseDrag)
        {
            windowRect.x += Event.current.delta.x;

#if UNITY_EDITOR
            windowRect.y += Event.current.delta.y;

#else
            windowRect.y -= Event.current.delta.y;
#endif
            buttonStatus = ButtonStatus.Hold;
        }

        // フレームレート表示
        var labelStyle = GUI.skin.label;
        labelStyle.fontSize = Mathf.FloorToInt(pixelPerCentimeter * 0.25f);

        // モード切り替え
        switch (debugMenuStatus)
        {
            case DebugMenuStatus.FPSMode:
                DrawFPS();
                break;
            case DebugMenuStatus.FrameTimeMode:
                DrawRenderFrameTime();
                break;
            case DebugMenuStatus.MonoMemoryMode:
                DrawMonoMemory();
                break;
        }

        // クリック判定
        var style = new GUIStyle();
        style.fixedWidth = windowRect.width;
        style.fixedHeight = windowRect.height;
        if (GUI.Button(new Rect(0, 0, windowRect.width, windowRect.height), string.Empty, style))
        {
            if (buttonStatus != ButtonStatus.Hold)
            {
                debugMenuStatus++;
            }

            buttonStatus = ButtonStatus.Free;
        }

        if (Event.current.type == EventType.MouseUp)
        {
            buttonStatus = ButtonStatus.Free;
        }
    }
    void DrawFPS()
    {
        var now = Time.time;

        if (now > lastFPSUpdated + updateFPSinterval)
        {
            var sumTime = 0f;
            for (var i = 0; i < deltaTimeArray.Length; i++)
            {
                sumTime += deltaTimeArray[i];
            }
            
            averageFPS = Mathf.FloorToInt(deltaTimeArray.Length / sumTime);
            lastFPSUpdated = now;
        }

        var lowfps = Mathf.FloorToInt(1f / deltaTimeArray.Max());
        var highfps = Mathf.FloorToInt(1f / deltaTimeArray.Min());
        

        // グラフ用パラメータ
        if (Event.current.type.Equals(EventType.Repaint))
        {
            material.SetColor("_Color", Color.green);
            material.SetInt("_Length", fpsRateArray.Length);
            material.SetFloatArray("_Param", fpsRateArray);

            Graphics.DrawTexture(new Rect(1, 1, windowRect.width - 2, windowRect.height - 2), texture, material);
        }

        GUILayout.Label(string.Format("{0:000} FPS ({1}-{2})", averageFPS, lowfps, highfps));
    }

    /// <summary>
    /// １フレームにかかった時間を表示します
    /// </summary>
    void DrawRenderFrameTime()
    {
        var ms = Mathf.FloorToInt(deltaTimeArray[deltaTimeArray.Length - 1] * 1000f);
        var lowms = Mathf.FloorToInt(deltaTimeArray.Min() * 1000f);
        var highms = Mathf.FloorToInt(deltaTimeArray.Max() * 1000f);

        // グラフ用パラメータ
        if (Event.current.type.Equals(EventType.Repaint))
        {
            material.SetColor("_Color", Color.blue);
            material.SetInt("_Length", deltaTimeArray.Length);
            material.SetFloatArray("_Param", deltaTimeArray);

            Graphics.DrawTexture(new Rect(1, 1, windowRect.width - 2, windowRect.height - 2), texture, material);
        }

        GUILayout.Label(string.Format("{0:00} MS ({1}-{2})", ms, lowms, highms));
    }

    /// <summary>
    /// MonoMemoryの使用状況を表示します
    /// </summary>
    void DrawMonoMemory()
    {
        var mb = System.GC.GetTotalMemory(false) / (1024f * 1024f);

        // グラフ用パラメータ
        if (Event.current.type.Equals(EventType.Repaint))
        {
            material.SetColor("_Color", Color.red);
            material.SetInt("_Length", memoryArray.Length);
            material.SetFloatArray("_Param", memoryArray.Select(x => (x - lowMemory) / ((highMemory - lowMemory) * 2f)).ToArray());

            Graphics.DrawTexture(new Rect(1, 1, windowRect.width - 2, windowRect.height - 2), texture, material);
        }

        GUILayout.Label(string.Format("{0} MB ({1}-{2})", mb.ToString("0.00"), lowMemory.ToString("0.00"), highMemory.ToString("0.00")));
    }

    void ShowMainWindow()
    {
        GUILayout.BeginVertical();
        {
            var buttonStyle = GUI.skin.button;

            GUILayout.BeginHorizontal();
            {
                // タイトルとFPS表示
                var labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.fontSize = Mathf.FloorToInt(pixelPerCentimeter * 0.5f);

                windowRect.width = Screen.width * 0.96f;
                windowRect.height = Screen.height * 0.96f;

                // FPSモードに切り替え
                buttonStyle.fontSize = Mathf.FloorToInt(pixelPerCentimeter * 0.3f);
                buttonStyle.fixedWidth = buttonStyle.fixedHeight = Mathf.FloorToInt(pixelPerCentimeter * 0.6f);
                if (GUILayout.Button("×", buttonStyle))
                {
                    debugMenuStatus = DebugMenuStatus.FPSMode;
                }

                GUILayout.Label(" DEBUG MENU", labelStyle);
            }

            GUILayout.EndHorizontal();

            buttonStyle.fixedWidth = 0;
            buttonStyle.fixedHeight = Mathf.FloorToInt(pixelPerCentimeter * 0.6f);
            buttonStyle.fontSize = Mathf.FloorToInt(pixelPerCentimeter * 0.3f);
            toolbarInt = GUILayout.Toolbar(toolbarInt, toolbarStrings);

            GUILayout.Space(10f);

            GUI.skin.verticalScrollbar.fixedWidth = GUI.skin.verticalScrollbarThumb.fixedWidth = pixelPerCentimeter * 0.3f;
            pos = GUILayout.BeginScrollView(pos);
            {
                var style = GUI.skin.label;
                style.fontSize = Mathf.FloorToInt(pixelPerCentimeter * 0.3f);

                switch (toolbarInt)
                {
                    case 0: // ツールウインドウ表示
                        ShowToolWindow();
                        break;
                    case 1: // ログ表示
                        ShowLogWindow();
                        break;
                }
            }

            GUILayout.EndScrollView();
            if (toolbarInt == 1)
            {
                if (GUILayout.Button("ClearLog", GUI.skin.button))
                {
                    reports.Clear();
                }
            }
        }

        GUILayout.EndVertical();
    }



    /// <summary>
    /// LoggerReportを受け取った時の処理を記述します
    /// </summary>
    /// <param name="report">Report.</param>
    public void HandleLog(LoggerReport report)
    {
        reports.Enqueue(report);

        // 最大数を超えたら古い順に削除
        while (reports.Count > LogSize)
        {
            reports.Dequeue();
        }
    }

    void ShowToolWindow()
    {
        string[] names = Enum.GetNames(typeof(ESeverType));
        GUILayout.Label("服务器选择:");
        currentServerIdx = GUILayout.SelectionGrid(currentServerIdx, names, names.Length, GUILayout.Width(windowRect.width));
        if (selectedServerIdx != currentServerIdx)
        {
            selectedServerIdx = currentServerIdx;
            PlayerPrefs.SetString("SERVER_TYPE", names[selectedServerIdx]);
            PlayerPrefs.Save();
            Debug.LogWarning("CHANGE SERVER MODE TO:" + names[selectedServerIdx]);
        }

        if (GUILayout.Button("清除本地缓存账号(谨慎)", GUI.skin.button))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        if (GUILayout.Button("重启app", GUI.skin.button))
        {
            Intro.Instance.Restart();
        }


        if (GUILayout.Button("清理当前未完成的氪金项", GUI.skin.button))
        {
            BillingManager.FinishPendingTransactions();
        }
    }
    

    /// <summary>
    /// ログウインドウを表示
    /// </summary>
    void ShowLogWindow()
    {
        var stringBuilder = new System.Text.StringBuilder();
        reports.Reverse().ToList().ForEach(r =>
        {
            stringBuilder.Append(r.ToString());
            stringBuilder.Append("\n");
        });
        GUILayout.Label(stringBuilder.ToString());
    }
    
    void OnApplicationQuit()
    {
        Destroy(gameObject);
    }
}