using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [SerializeField] private AudioSource p1ShotSource;
    [SerializeField] private AudioSource p2ShotSource;
    [SerializeField] private AudioSource musicSource;
    
    [SerializeField] private AudioClip menuMusicClip;
    [SerializeField] private AudioClip gameMusicClip;
    [SerializeField] private AudioClip[] shotClips;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayP1Shot()
    {
        p1ShotSource.clip = shotClips[UnityEngine.Random.Range(0, shotClips.Length)];
        p1ShotSource.Play();
    }
    
    public void PlayP2Shot()
    {
        p2ShotSource.clip = shotClips[UnityEngine.Random.Range(0, shotClips.Length)];
        p2ShotSource.Play();
    }

    public void PlayMenuMusic()
    {
        musicSource.clip = menuMusicClip;
        musicSource.Play();
    }

    public void PlayGameMusic()
    {
        musicSource.clip = gameMusicClip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
    
    public void SetVolume(float volume)
    {
        p1ShotSource.volume = volume * 0.01f;
        p2ShotSource.volume = volume * 0.01f;
        musicSource.volume = volume * 0.05f;
    }


    public void IncreasePitch()
    {
        p1ShotSource.pitch += 0.25f;
        p2ShotSource.pitch += 0.25f;
        musicSource.pitch += 0.25f;
    }

    public void DecreasePitch()
    {
        p1ShotSource.pitch -= 0.25f;
        p2ShotSource.pitch -= 0.25f;
        musicSource.pitch -= 0.25f;
    }
}
