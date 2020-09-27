using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySE : MonoBehaviour
{
    public void Play(int soundId)
    {
        SoundManager.PlaySE(soundId);
    }
}
