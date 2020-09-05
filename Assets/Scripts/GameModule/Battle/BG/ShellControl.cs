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
    public ShellStatus shellStatus = ShellStatus.Closed;
    float openReminingTime;
    float openReminingTimedefault;
    List<FishBase> listDamagedFish = new List<FishBase>();

    void Awake()
    {
        openReminingTime = Wrapper.GetRandom( 1f, 5f);
        openReminingTimedefault = BattleConst.instance.OpenShellRemainingTime;

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
                    openReminingTime = BattleConst.instance.ShellOpenningTime;
                    if (Wrapper.GetRandom(0f, 1f) < BattleConst.instance.ShellPearlResetRate)
                    {
                        goPearl.SetActive(true);
                    }
                    break;
                case ShellStatus.Open:
                    animator.SetTrigger("close");
                    openReminingTime = openReminingTimedefault;
                    break;
            }
            
        }
    }

    void Closed()
    {
        
        listDamagedFish.Clear();
        shellStatus = ShellStatus.Closed;
    }
    void Closing()
    {
        shellStatus = ShellStatus.Closing;
    }
    void Openning()
    {
        shellStatus = ShellStatus.Open;
    }
    public bool PearlEatenCheck(PlayerBase fish)
    {
        if (CanEatPearl())
        {
            goPearl.SetActive(false);
            fish.Heal(fish.lifeMax);
            //fish.Eat(ConfigTableProxy.Instance.GetDataById(23).floatValue);
            return true;
        }
        else if(shellStatus == ShellStatus.Closing && !listDamagedFish.Contains(fish))
        {
            fish.Damage( BattleConst.instance.ShellCloseDmg, null );
            listDamagedFish.Add(fish);
            return false;
        }
        return false;
    }

    public bool CanEatPearl()
    {
        return shellStatus == ShellStatus.Open && goPearl.activeSelf;
    }
}
