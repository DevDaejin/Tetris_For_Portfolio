using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    //Title, Game, Result
    [SerializeField] private AudioClip[] bgmClip;

    //Move, Rotate, SoftDrop, HardDrop, Hold, Line1, Line2, Line3, Line4
    [SerializeField] private AudioClip[] sfxClip;
    
    private AudioSource bgmSource;
    private AudioSource sfxSource;

    public float bgmVolume = 1;
    public float sfxVolume = 1;

    private readonly string bgmName = "BGM";
    private readonly string sfxName = "SFX";

    private void Start()
    {
        bgmSource = CreateAudioSource(bgmName);
        bgmSource.playOnAwake = false;

        sfxSource = CreateAudioSource(sfxName);
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
    }

    public void PlaySFX(SFX sfx, float pitch = 1)
    {
        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(sfxClip[(int)sfx], sfxVolume);
    }

    public void SetBGMVolume(float volume)
    {
        bgmVolume = volume;
        bgmSource.volume = bgmVolume;
    }
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = sfxVolume;
    }

    public void PlayBGM(BGM bgm, float pitch = 1, bool isLoop = false)
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();

        bgmSource.volume = bgmVolume;
        bgmSource.pitch = pitch;
        bgmSource.clip = bgmClip[(int)bgm];
        bgmSource.loop = isLoop;
        bgmSource.Play();
    }

    public void PlayBGM(BGM bgm, float volume, float pitch, bool isLoop)
    {
        bgmVolume = volume;
        PlayBGM(bgm, volume, pitch, isLoop);
    }

    public void StopBGM() => bgmSource.Stop();

    public void SetBGMPitch(float pitch) => bgmSource.pitch = pitch;

    private AudioSource CreateAudioSource(string name)
    {
        GameObject o = new GameObject(name);
        o.transform.SetParent(transform);
        return o.AddComponent<AudioSource>();
    }
}
