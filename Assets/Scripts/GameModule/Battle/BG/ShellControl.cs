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

    AudioSource audioSource;
    Animator animator;
    GameObject goTargetIcon;
    bool isTargeting = false;
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
        audioSource = GetComponent<AudioSource>();

        if (!TutorialControl.IsStep(TutorialControl.Step.Completed))
        {
            var go = ResourceManager.LoadSync(AssetPathConst.targetIconShellPath, typeof(GameObject)).Asset as GameObject;
            goTargetIcon = GameObjectUtil.InstantiatePrefab(go, gameObject, false);
        }
    }
    public void ShowTargetIcon(bool enable)
    {
        isTargeting = enable;
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
                    SoundManager.PlaySE(6, audioSource);
                    openReminingTime = BattleConst.instance.ShellOpenningTime;
                    if (Wrapper.GetRandom(0f, 1f) < BattleConst.instance.ShellPearlResetRate || BattleManagerGroup.GetInstance().IsTutorialStep(BattleManagerGroup.TutorialStep.ShellMissionChecking))
                    {
                        goPearl.SetActive(true);
                    }
                    break;
                case ShellStatus.Open:
                    animator.SetTrigger("close");
                    SoundManager.PlaySE(7, audioSource);
                    openReminingTime = openReminingTimedefault;
                    break;
            }
            
        }
        GameObjectUtil.SetActive(goTargetIcon, isTargeting && CanEatPearl());
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
        if (shellStatus == ShellStatus.Closing && !listDamagedFish.Contains(fish))
        {
            fish.Damage((int)(fish.life * BattleConst.instance.ShellCloseDmg), null, FishBase.AttackerType.Shell);
            listDamagedFish.Add(fish);
            return false;
        }
        else if (CanEatPearl())
        {
            goPearl.SetActive(false);
            fish.Eat(ConfigTableProxy.Instance.GetDataById(22).floatValue);
            if (fish.fishType == FishBase.FishType.Player)
            {
                PlayerModel.Instance.MissionActionTriggerAdd(10, 1);
                BattleManagerGroup.GetInstance().AddTutorialCnt(BattleManagerGroup.TutorialStep.ShellMissionChecking);
            }
            return true;
        }
        return false;
    }

    public bool CanEatPearl()
    {
        return shellStatus == ShellStatus.Open && goPearl.activeSelf;
    }
}
