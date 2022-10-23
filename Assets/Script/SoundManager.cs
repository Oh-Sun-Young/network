using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
 * 참고
 * - ( Unity쨩 - 15일차 ) 일회성 사운드, 반복 사운드 만들어보기 : https://m.blog.naver.com/PostView.naver?isHttpsRedirect=true&blogId=lovelybarry&logNo=220852443991
 */

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { 
        get {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<SoundManager>();
            }
            return m_Instance;
        }
    }
    private static SoundManager m_Instance;

    [SerializeField] AudioSource BgmAudioSource;

    [SerializeField] AudioClip IntroClip;
    [SerializeField] AudioClip PlayClip;
    [SerializeField] AudioClip GameOverClip;

    private void Awake()
    {
        if(Instance != this)
        {
            Destroy(gameObject);
        }
    }
    public void SoundEnable(bool active)
    {
        BgmAudioSource.mute = !active;
    }
    public void OnIntro()
    {
        if(BgmAudioSource.clip != IntroClip)
        {
            BgmAudioSource.Stop();
            StartCoroutine("IntroSoundLoop");
        }
    }
    IEnumerator IntroSoundLoop()
    {
        while (true)
        {
            if (BgmAudioSource.isPlaying == false)
            {
                BgmAudioSource.clip = IntroClip;
                BgmAudioSource.Play();
            }
            yield return null;
        }
    }
    public void OnPlay()
    {
        StopCoroutine("IntroSoundLoop");
        BgmAudioSource.Stop();
        StartCoroutine("PlaySoundLoop");
    }
    IEnumerator PlaySoundLoop()
    {
        while (true)
        {
            if (BgmAudioSource.isPlaying == false)
            {
                BgmAudioSource.clip = PlayClip;
                BgmAudioSource.Play();
            }
            yield return null;
        }
    }
    public void OnGameOver()
    {
        StopCoroutine("PlaySoundLoop");
        BgmAudioSource.Stop();
        BgmAudioSource.clip = GameOverClip;
        BgmAudioSource.Play();
    }
}
