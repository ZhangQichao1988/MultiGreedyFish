﻿/**
 * Jackpot
 * Copyright(c) 2014 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 * 
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */
using UnityEngine;
using System.Collections.Generic;

namespace Jackpot
{
    /// <summary>
    /// アプリ画面上にログ出力するLoggerHandlerです
    /// DevelopmentBuild時のみ動作します
    /// </summary>
    /// <remarks>
    /// 使用する場合、以下の初期化コードをプログラム中に記載します
    /// <code>
    /// using Jackpot;
    /// public class EntryPoint : MonoBehaviour
    /// {
    ///     void Awake()
    ///     {
    ///         GuiLogHandler.Instance.StartLog();
    ///         // あまりないと思いますが、ReleaseBuildでも使用したい場合は以下
    ///         // GuiLogHandler.Instance.StartLog(true);
    ///     }
    /// }
    /// </code>
    /// アプリ画面上では、3本タップで表示/非表示を切り替えます
    /// </remarks>
    public class GuiLogHandler : MonoBehaviour, ILoggerHandler
    {
        #region Constants

        const int DefaultLogSize = 1000;

        #endregion

        #region Properties

        /// <summary>
        /// GuiLogHandlerのSingletonインスタンスを返却します
        /// </summary>
        /// <value>The instance.</value>
        public static GuiLogHandler Instance
        {
            get
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new GameObject("GuiLogHandler").AddComponent<GuiLogHandler>();
                    }
                }
                return instance;
            }
        }

        static GuiLogHandler instance;
        static System.Object lockObject = new System.Object();
        Queue<LoggerReport> reports;
        int logSize;
        bool isDisplay;
        bool logHasBeenStarted;
        Vector2 scrollPosition;
        private string searchString = "";

        #endregion

        #region public methods


        /// <summary>
        /// GuiLogHandlerのログ集計を開始します
        /// </summary>
        /// <param name="ignoreBuildConstrants">DevelopmentBuild, ReleaseBuildに関わらず使用する場合は<c></c>true</c>を指定します。defaultはfalseです</param>
        public void StartLog(bool ignoreBuildConfiguration = false)
        {
            if (!ignoreBuildConfiguration && !Debug.isDebugBuild)
            {
                return;
            }
            if (logHasBeenStarted)
            {
                return;
            }
            Logger.AddHandler(this);
            logHasBeenStarted = true;
        }

        /// <summary>
        /// GuiLogHandler上に保持するログの量を設定します。
        /// </summary>
        /// <remarks>
        /// あまりにも多い数値を設定すると表示時いとても重くなるのでほどほどにお願いします
        /// </remarks>
        /// <param name="size">Size.</param>
        public void SetLogSize(int size)
        {
            logSize = size;
            Dequeue();
        }

        /// <summary>
        /// GuiLogHandlerをゲーム画面上に表示します
        /// </summary>
        public void Show()
        {
            isDisplay = true;
        }

        /// <summary>
        /// GuiLogHandlerを非表示にします
        /// </summary>
        public void Hide()
        {
            isDisplay = false;
        }

        public void HandleLog(LoggerReport report)
        {
            reports.Enqueue(report);
            Dequeue();
        }

        #endregion

        #region Unity methods

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            reports = new Queue<LoggerReport>();
            logSize = DefaultLogSize;
            isDisplay = false;
            scrollPosition = Vector2.zero;
        }

        void Update()
        {
            ProcessTouch();
        }

        void OnGUI()
        {
            if (!isDisplay)
            {
                return;
            }

            if (GUI.Button(new Rect(10, 10, 100, 50), "Clear"))
            {
                reports.Clear();
            }
            searchString = GUI.TextField(new Rect(120, 10, 300, 50), searchString);
            int scrollTop = 80;

            float labelWidth = 10000;
            float labelHeight = GUI.skin.GetStyle("label").fontSize + 10f;
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), string.Empty);
            Rect scroller = new Rect(0f, scrollTop, Screen.width, Screen.height - scrollTop);
            Rect scrollee = new Rect(0f, scrollTop, labelWidth, labelHeight * reports.Count - scrollTop);
            scrollPosition = GUI.BeginScrollView(scroller, scrollPosition, scrollee);
            List<LoggerReport> copied = new List<LoggerReport>(reports);
            for (int i = copied.Count - 1, ii = 0; i >= 0; i--)
            {
                var logString = copied[i].ToString();
                if (searchString.Length != 0 && logString.IndexOf(searchString) == -1)
                {
                    continue;
                }
                GUI.Label(new Rect(0, scrollTop + labelHeight * ii, labelWidth, labelHeight), logString);
                ii++;
            }
            GUI.EndScrollView();
        }

        #endregion

        #region Private Methods

        void Dequeue()
        {
            while (true)
            {
                if (reports.Count <= logSize)
                {
                    break;
                }
                reports.Dequeue();
            }
        }

        void ProcessTouch()
        {
            if (isDisplay)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.touches[0];
                    if (touch.phase == TouchPhase.Moved)
                    {
                        scrollPosition.x -= touch.deltaPosition.x;
                        scrollPosition.y += touch.deltaPosition.y;
                    }
                }
                if (Input.touchCount > 2)
                {
                    Touch touch = Input.touches[2];
                    if (touch.phase == TouchPhase.Ended)
                    {
                        Hide();
                    }
                    return;
                }
            }
            if (Input.touchCount > 2)
            {
                Touch touch = Input.touches[2];
                if (touch.phase == TouchPhase.Ended)
                {
                    Show();
                }
                return;
            }
        }

        #endregion

    }
}
