using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChattingManager : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] GameObject Blur;
    [SerializeField] GameObject ControllerPanel;
    [SerializeField] GameObject ChattingPanel;

    public void ChattingEnable(bool active)
    {
        Blur.SetActive(active);
        ControllerPanel.GetComponent<CanvasGroup>().alpha = (active ? 0 : 1);
        ChattingPanel.SetActive(active);
    }
}
