using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private void Awake()
    {
        ApplicationChrome.statusBarState = ApplicationChrome.States.Hidden;
        ApplicationChrome.navigationBarState = ApplicationChrome.States.TranslucentOverContent;
    }
}