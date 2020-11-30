using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

[DisallowMultipleComponent]
public class BlLogger : MonoBehaviour
{
    struct Log
    {
        public string message;
        public string stackTrace;
        public LogType type;
    }

    /// <summary>  
    /// Whether to only keep a certain number of logs.  
    ///  
    /// Setting this can be helpful if memory usage is a concern.  
    /// </summary>  
    public bool restrictLogCount = true;

    /// <summary>  
    /// Number of logs to keep before removing old ones.  
    /// </summary>  
    public int maxLogs = 100;

    readonly List<Log> logs = new List<Log>();
    Vector2 scrollPosition;
    bool collapse;

    /// <summary>
    /// 
    /// </summary>

    public string ip = "127.0.0.1";
    public int port = 6000;
    private IPEndPoint ipEndPoint;
    private UdpClient udpClient;

    readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
{
    { LogType.Assert, Color.white },
    { LogType.Error, Color.red },
    { LogType.Exception, Color.red },
    { LogType.Log, Color.white },
    { LogType.Warning, Color.yellow },
};

    private void Awake()
    {
        ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        udpClient = new UdpClient();

        Application.logMessageReceived += OnLogMessage;
    }

    void OnGUI()
    {
        windowRect = GUILayout.Window(123456, windowRect, DrawConsoleWindow, windowTitle);
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= OnLogMessage;

        udpClient.Close();
    }

    /// <summary>  
    /// Records a log from the log callback.  
    /// </summary>  
    /// <param name="message">Message.</param>  
    /// <param name="stackTrace">Trace of where the message came from.</param>  
    /// <param name="type">Type of message (error, exception, warning, assert).</param>  
    void OnLogMessage(string message, string stackTrace, LogType type)
    {
        logs.Insert(0, new Log
        {
            message = message,
            stackTrace = stackTrace,
            type = type,
        });
        TrimExcessLogs();

        try
        {
            byte[] bytes;
            bytes = Encoding.UTF8.GetBytes(message + "\n" + stackTrace);
            udpClient.Send(bytes, bytes.Length, ipEndPoint);

        }
        catch (Exception e)
        {

        }
    }

    const string windowTitle = "Console";
    const int margin = 20;
    static readonly GUIContent closeLabel = new GUIContent("Close", "Close the contents of the console.");
    static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
    static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

    readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);
    Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));

    /// <summary>  
    /// Displays a window that lists the recorded logs.  
    /// </summary>  
    /// <param name="windowID">Window ID.</param>  
    void DrawConsoleWindow(int windowID)
    {
        DrawLogsList();
        DrawToolbar();

        // Allow the window to be dragged by its title bar.  
        GUI.DragWindow(titleBarRect);
    }

    /// <summary>  
    /// Displays a scrollable list of logs.  
    /// </summary>  
    void DrawLogsList()
    {
        GUI.skin.verticalScrollbarThumb.fixedWidth = 50;
        GUI.skin.verticalScrollbarThumb.fixedHeight = 50;
        GUI.skin.verticalScrollbar.fixedWidth = 50;

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        // Iterate through the recorded logs.  
        for (var i = 0; i < logs.Count; i++)
        {
            var log = logs[i];

            // Combine identical messages if collapse option is chosen.  
            if (collapse && i > 0)
            {
                var previousMessage = logs[i - 1].message;

                if (log.message == previousMessage)
                {
                    continue;
                }
            }

            GUI.contentColor = logTypeColors[log.type];
            GUILayout.Label(log.message);
            GUILayout.Label(log.stackTrace);
        }

        GUILayout.EndScrollView();

        // Ensure GUI colour is reset before drawing other components.  
        GUI.contentColor = Color.white;
    }

    /// <summary>  
    /// Displays options for filtering and changing the logs list.  
    /// </summary>  
    void DrawToolbar()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button(clearLabel, GUILayout.Width(50), GUILayout.Height(50)))
        {
            logs.Clear();
        }

        if (GUILayout.Button(closeLabel, GUILayout.Width(50), GUILayout.Height(50)))
        {
            enabled = false;
        }

        collapse = GUILayout.Toggle(collapse, collapseLabel,GUILayout.ExpandWidth(false));

        GUILayout.EndHorizontal();
    }


    /// <summary>  
    /// Removes old logs that exceed the maximum number allowed.  
    /// </summary>  
    void TrimExcessLogs()
    {
        if (!restrictLogCount)
        {
            return;
        }

        var amountToRemove = Mathf.Max(logs.Count - maxLogs, 0);

        if (amountToRemove == 0)
        {
            return;
        }

        logs.RemoveRange(maxLogs, amountToRemove);
    }
}