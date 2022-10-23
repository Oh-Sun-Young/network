using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
/*
 * 참고
 * [Unity] EventSystem.current - 현재 클릭된 UI 가져와서 구별하기 : https://timeboxstory.tistory.com/m/85
 */

public class SoundButtonAction : MonoBehaviour
{
    [SerializeField] GameObject ButtonActive;
    [SerializeField] bool active;

    public void SoundEnable()
    {
        GameObject obj = EventSystem.current.currentSelectedGameObject;

        ButtonActive.SetActive(true);
        SoundManager.Instance.SoundEnable(active);
        obj.SetActive(false);
    }
}
