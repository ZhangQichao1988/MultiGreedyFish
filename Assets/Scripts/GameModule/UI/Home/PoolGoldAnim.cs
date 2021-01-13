using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolGoldAnim : MonoBehaviour
{
    Vector3 resGoldPosition;

    Vector3 targetPos;
    float remainingTime;
    int step = 0;
    public void Init(Vector3 resGoldPosition)
    {
        this.resGoldPosition = resGoldPosition;
    }
    private void Start()
    {
        float moveAngle = Wrapper.GetRandom(0f, 180f);
        remainingTime = 1f;
        var movePos = Quaternion.AngleAxis(moveAngle, Vector3.forward) * Vector3.right * Wrapper.GetRandom(5f, 2f);
        targetPos = transform.position + movePos;
        
    }

    // Update is called once per frame
    void Update()
    {
        remainingTime = Mathf.Max(0f, remainingTime - Time.deltaTime);
        switch (step)
        {
            case 0:
                transform.position = Vector3.Lerp(transform.position, targetPos, 1f - remainingTime);
                if (remainingTime <= 0.5f)
                {
                    remainingTime = Wrapper.GetRandom(0.1f, 0.8f);
                    step = 1;
                }
                break;
            case 1:
                if (remainingTime <= 0f)
                {
                    remainingTime = 1f;
                    targetPos = resGoldPosition;
                    step = 2;
                }
                    
                break;
            case 2:
                transform.position = Vector3.Lerp(transform.position, targetPos, 1f - remainingTime);
                if (remainingTime <= 0.5f)
                {
                    UIHomeResource.Instance.StartGoldAddCalc();
                    Destroy(gameObject);
                    //step = 3;
                }
                break;
        }
    }
}
