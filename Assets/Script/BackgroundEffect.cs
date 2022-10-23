using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundEffect : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine("ImageLoop");
    }

    private void OnDisable()
    {
        StopCoroutine("ImageLoop");
    }

    IEnumerator ImageLoop()
    {
        int cnt = 0;
        int all = transform.childCount;
        while (true)
        {
            for(int i = 0; i < all; i++)
            {
                transform.GetChild(i).gameObject.SetActive((i == cnt) ? true : false);
            }
            if (++cnt == all) cnt = 0;
            yield return new WaitForSeconds(0.25f);
        }
    }
}
