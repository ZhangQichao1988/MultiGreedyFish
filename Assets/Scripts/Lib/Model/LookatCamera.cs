using UnityEngine;

public class LookatCamera : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.forward = transform.position - Camera.main.transform.position;
    }
}