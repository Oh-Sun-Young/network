using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundButtonCheck : MonoBehaviour
{
    [SerializeField] GameObject SoundOffButton;
    [SerializeField] GameObject SoundOnButton;

    private AudioSource BgmAudioSource;

    private void Awake()
    {
        BgmAudioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
    }

    private void OnEnable()
    {

        if (BgmAudioSource.mute == true)
        {
            SoundOffButton.SetActive(false);
            SoundOnButton.SetActive(true);
        }
        else
        {
            SoundOffButton.SetActive(true);
            SoundOnButton.SetActive(false);
        }
    }
}
