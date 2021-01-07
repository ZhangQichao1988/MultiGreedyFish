using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleTime : MonoBehaviour
{
    public bool isRun = false;
    float time = 0f;
    Text textTime;
    private void Awake()
    {
        textTime = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRun)
        {
            time += Time.deltaTime;
            textTime.text = string.Format("{0}:{1}", ((int)(time / 60)).ToString().PadLeft(2, '0'), ((int)(time % 60)).ToString().PadLeft(2, '0'));
        }
    }
}
