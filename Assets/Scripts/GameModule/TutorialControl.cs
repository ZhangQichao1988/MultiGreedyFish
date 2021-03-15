using System.Text;
using System.IO;
using UnityEngine;
using System;


public class TutorialControl
{
    public enum Step
    {
        None,
        GotoTutorialBattle,

        GotoTestBattle,
        Completed
    };
    static public Step currStep = Step.None;

    static public void NextStep()
    {
        ++currStep;
        PlayerPrefs.SetInt(AppConst.PlayerPrefabsTutorialStep, (int)currStep);
    }

    static public bool IsCompleted() { return currStep == Step.Completed; }
    static public bool IsStep(Step step, bool toNext = false) 
    {
        bool ret = step == currStep;
        if (ret && toNext)
        {
            NextStep();
        }
        return ret;
    }
}
