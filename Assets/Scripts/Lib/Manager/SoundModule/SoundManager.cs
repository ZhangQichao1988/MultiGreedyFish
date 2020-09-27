using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;

/// <summary>
/// 声音管理器
/// </summary>
public class SoundManager : MonoBehaviour
{
    //public GameObject goBgm;
    public GameObject goSe;
    //private AudioSource bgmSource;
    private AudioSource[] seSources;
    private Dictionary<int, AssetRef<AudioClip>> dicAudio = new Dictionary<int, AssetRef<AudioClip>>();
    public static SoundManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        //bgmSource = goBgm.GetComponent<AudioSource>();
        seSources = goSe.GetComponents<AudioSource>();

    }
    static public AudioClip GetAudioClip(int soundId)
    {
        AudioClip ac = null;
        if (Instance.dicAudio.ContainsKey(soundId))
        {
            ac = Instance.dicAudio[soundId].Asset;
            Debug.Assert(ac != null, "SoundManager.PlaySE()_1");
        }
        else
        {
            var asset = ResourceManager.LoadSync<AudioClip>(string.Format(AssetPathConst.soundRootPath, soundId));
            Instance.dicAudio.Add(soundId, asset);
            ac = asset.Asset;
            Debug.Assert(ac != null, "SoundManager.PlaySE()_2" + soundId);
        }
        return ac;
    }
    static public AudioSource CreateAudioSource(GameObject gameObject)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = 50;
        audioSource.playOnAwake = false;
        return audioSource;
    }
    static public void PlaySE(int soundId, AudioSource audioSource)
    {
        audioSource.clip = SoundManager.GetAudioClip(soundId);
        audioSource.Play();
    }
    static public void PlaySE(int soundId)
    {
        if (!Instance) { return; }
        AudioClip ac = GetAudioClip(soundId);

        foreach (var ss in Instance.seSources)
        {
            if (!ss.isPlaying) 
            {
                ss.clip = ac;
                ss.Play();
                return;
            }
        }
        // 没有空闲Source的话，就强制播放第一个Source
        Instance.seSources[0].clip = ac;
        Instance.seSources[0].Play();
    }

    public void UnloadBattleSE()
    {
        Dictionary<int, AssetRef<AudioClip>> tmp = new Dictionary<int, AssetRef<AudioClip>>(dicAudio);
        foreach (var se in tmp.Keys)
        {
            if (SoundDataTableProxy.Instance.GetDataById(se).scene == (int)SoundData.SceneType.Battle)
            {
                ResourceManager.Unload(dicAudio[se]);
                dicAudio.Remove(se);
            }
        }
    }

    public void UnloadSimSE()
    {
        Dictionary<int, AssetRef<AudioClip>> tmp = new Dictionary<int, AssetRef<AudioClip>>(dicAudio);
        foreach (var se in tmp.Keys)
        {
            if (SoundDataTableProxy.Instance.GetDataById(se).scene == (int)SoundData.SceneType.Sim)
            {
                ResourceManager.Unload(dicAudio[se]);
                dicAudio.Remove(se);
            }
        }
    }
}

