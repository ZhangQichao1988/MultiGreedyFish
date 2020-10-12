/**
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
using UnityEditor;
using Jackpot;
using System.Reflection;

namespace Jackpot
{
    [CustomEditor(typeof(Scheduler))]
    public class SchedulerEditor : Editor
    {
        public static bool defaultFoldout = false;

        /// <summary>
        /// Workerの種類を示します
        /// </summary>
        enum WorkerType
        {
            Timeout,
            Frame,
            Interval
        }

        /// <summary>
        /// リフレクションを利用して<c>Scheduler</c>インスタンスから取得するプライベートフィールドの名前です
        /// </summary>
        static readonly string workersName = "workers";

        /// <summary>
        /// 前フレームで操作されたインスペクタの畳み込みを記憶している辞書
        /// </summary>
        Dictionary<IScheduler, bool> foldoutsByLastUpdate = new Dictionary<IScheduler, bool>();

        /// <summary>
        /// インスペクタの畳み込みを foldoutsByLastUpdate に引き継ぐ為に、一時的に記憶する辞書
        /// </summary>
        Dictionary<IScheduler, bool> foldoutsForNextUpdate = new Dictionary<IScheduler, bool>();

        /// <summary>
        /// インスペクタのスクロールビューの位置を保存します
        /// </summary>
        Vector2 scrollPosition;

        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            var workers = GetWorkers(target);
            var labelText = string.Format("Workers ({0})", workers == null ? 0 : workers.Count);

            EditorGUILayout.LabelField(labelText);

            if (workers != null)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                EditorGUI.indentLevel++;

                workers.ForEach(ViewWorker);

                foldoutsByLastUpdate.Clear();
                foldoutsByLastUpdate = new Dictionary<IScheduler, bool>(foldoutsForNextUpdate);
                foldoutsForNextUpdate.Clear();

                EditorGUI.indentLevel--;
                EditorGUILayout.EndScrollView();
            }

        }

        /// <summary>
        /// Schedulerから非公開フィールドである、稼働中のISchedulerの一覧をリフレクションで取得します
        /// </summary>
        /// <returns>The workers.</returns>
        /// <param name="obj">Object.</param>
        static List<IScheduler> GetWorkers(Object obj)
        {
            var scheduler = obj as Scheduler;
            var fieldInfo = typeof(Scheduler).GetField(workersName, BindingFlags.NonPublic | BindingFlags.Instance);
            return fieldInfo.GetValue(scheduler) as List<IScheduler>;
        }

        /// <summary>
        /// インスペクタのスケジューラインスタンスの項目の畳み込みをします
        /// </summary>
        /// <param name="worker">Worker.</param>
        bool Foldout(IScheduler worker)
        {
            var foldout = foldoutsByLastUpdate.ContainsKey(worker) ? foldoutsByLastUpdate[worker] : defaultFoldout;

            foldout = EditorGUILayout.Foldout(foldout, string.Format("[{1}] {0}", worker.Name, worker.State));
            foldoutsForNextUpdate.Add(worker, foldout);

            return foldout;
        }

        /// <summary>
        /// ISchedulerインスタンスをインスペクタ上の一項目として表示します
        /// </summary>
        /// <param name="worker">Worker.</param>
        void ViewWorker(IScheduler worker)
        {
            if (!Foldout(worker))
            {
                return;
            }
            EditorGUI.indentLevel++;

            EditorGUILayout.LabelField("Name", worker.Name);
            EditorGUILayout.EnumPopup("State", worker.State);

            if ((worker as DelayFrameScheduler) != null)
            {
                ViewDelayFrameScheduler(worker as DelayFrameScheduler);
            }
            else if ((worker as TimeoutScheduler) != null)
            {
                ViewTimeoutScheduler(worker as TimeoutScheduler);
            }
            else if ((worker as IntervalScheduler) != null)
            {
                ViewIntervalScheduler(worker as IntervalScheduler);
            }

            EditorGUI.indentLevel--;
        }

        static void ViewTimeoutScheduler(TimeoutScheduler scheduler)
        {
            EditorGUILayout.EnumPopup("Type", WorkerType.Timeout);
            EditorGUILayout.Toggle("Follow TimeScale", scheduler.IsFollowTimeScale);
            EditorGUILayout.FloatField("Timer (s)", scheduler.Delay);
            EditorGUILayout.FloatField("Remain (s)", scheduler.Delay - scheduler.TotalDeltaTime);
        }

        static void ViewDelayFrameScheduler(DelayFrameScheduler scheduler)
        {
            EditorGUILayout.EnumPopup("Type", WorkerType.Frame);
            EditorGUILayout.IntField("Delay (frame)", scheduler.Delay);
            EditorGUILayout.IntField("Remain (frame)", scheduler.Delay - scheduler.FrameCount);
        }

        static void ViewIntervalScheduler(IntervalScheduler scheduler)
        {
            EditorGUILayout.EnumPopup("Type", WorkerType.Interval);
            EditorGUILayout.Toggle("Follow TimeScale", scheduler.IsFollowTimeScale);
            EditorGUILayout.FloatField("Interval (s)", scheduler.Seconds);
            EditorGUILayout.FloatField("Next (s)", scheduler.Seconds - scheduler.StackTime);
        }
    }
}
