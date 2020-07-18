using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellControl : MonoBehaviour
{
    public enum ShellStatus{ 
        Closed,
        Closing,
        Open,
    };

    public GameObject goPearl = null;

    Animator animator;
    ShellStatus shellStatus = ShellStatus.Closed;
    float openReminingTime;
    float openReminingTimedefault;

    void Awake()
    {
        openReminingTime = Wrapper.GetRandom( GameConst.OpenShellRemainingTimeRange.x, GameConst.OpenShellRemainingTimeRange.y);
        openReminingTimedefault = openReminingTime;

        animator = GetComponent<Animator>();
        Debug.Assert(animator != null, "ShellControl.Awake()_1");
    }

    void Update()
    {
        openReminingTime -= Time.deltaTime;
        if (openReminingTime <= 0)
        {
            switch (shellStatus)
            {
                case ShellStatus.Closed:
                    animator.SetTrigger("open");
                    shellStatus = ShellStatus.Open;
                    openReminingTime = GameConst.ShellOpenningTime;
                    break;
                case ShellStatus.Open:
                    animator.SetTrigger("close");
                    shellStatus = ShellStatus.Closing;
                    openReminingTime = openReminingTimedefault;
                    break;
            }
            
        }
    }

    void Closed()
    {
        shellStatus = ShellStatus.Closed;
    }

    public bool PearlEatenCheck()
    {
        if (shellStatus == ShellStatus.Open && goPearl.activeSelf)
        {
            goPearl.SetActive(false);
            return true;
        }
        return false;
    }
}
