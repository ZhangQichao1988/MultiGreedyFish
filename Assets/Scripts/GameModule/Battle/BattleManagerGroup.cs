using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManagerGroup : MonoBehaviour
{

    enum GameStep 
    {
        None,
        Title,
        Battle,
        Result,
    }
    static BattleManagerGroup instance = null;

    public InGameUIPanel inGameUIPanel = null;
    public FishManager fishManager = null;
    public AquaticManager aquaticManager = null;
    public ShellManager shellManager = null;
    public PoisonRing poisonRing = null;
    public CameraFollow cameraFollow = null;

    public Text resultText = null;

    Animator animator = null;
    private bool isPause = false;
    private void Awake()
	{
        instance = this;
        animator = GetComponent<Animator>();

        GotoBattle();
    }
    static public BattleManagerGroup GetInstance()
    { return instance; }

    

    public void GotoBattle()
    {
        //resultRoot.SetActive(false);
        //titleBtn.SetActive(false);
        //battleControl.SetActive(true) ;
        animator.SetTrigger("ReadyStart");
        poisonRing.gameObject.SetActive(true);
        poisonRing.Init();
        cameraFollow.Init();

        fishManager.Clean();
        inGameUIPanel.Init();
        fishManager.CreateEnemy();
        this.isPause = true;
    }

    public void GotoHome()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Intro");
    }
        public void SetPlayPoint()
    {
        animator.SetTrigger("PlayPoint");
    }
    public void GotoResult(int rank)
    {
        if (rank == 1)
        {
            //this.isPause = true;
            animator.SetTrigger("Win");
            animator.SetTrigger("BattleEnd");
        }
        else
        {
            animator.SetTrigger("Lose");
        }
        //resultRoot.SetActive(true);
        //battleControl.SetActive(false);
        resultText.text = string.Format( LanguageDataTableProxy.GetText(1), rank.ToString() );
    }

    public void BattleEnd()
    {
        animator.SetTrigger("BattleEnd");
    }

	public void SetBattleStart()
    {
        this.isPause = false;
    }

	public void SetBattlePause()
	{
		this.isPause = true;
	}

	private void Update()
	{
        if (isPause) { return; }
        fishManager.CustomUpdate();
        poisonRing.CustomUpdate();
    }
}
