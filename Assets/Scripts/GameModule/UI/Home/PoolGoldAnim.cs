using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolGoldAnim : MonoBehaviour
{
    static readonly Vector3 goldTargetPos = new Vector3(264.6f, 184.7f, 352.9f);
    Vector3 targetPos;
    float remainingTime;
    int step = 0;
    private void Start()
    {
        float moveAngle = Wrapper.GetRandom(0f, 180f);
        remainingTime = 1f;
        var movePos = Quaternion.AngleAxis(moveAngle, Vector3.forward) * Vector3.right * Wrapper.GetRandom(50f, 250f);
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
                    remainingTime = 2f;
                    targetPos = goldTargetPos;
                    step = 2;
                }
                    
                break;
            case 2:
                transform.position = Vector3.Lerp(transform.position, targetPos, 1f - remainingTime);
                if (remainingTime <= 0.2f)
                {
                    UIHomeResource.Instance.StartGoldAddCalc();
                    Destroy(gameObject);
                    step = 3;
                }
                break;
        }
    }
}
