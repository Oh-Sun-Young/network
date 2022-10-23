using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectButtonEnable : MonoBehaviour
{
    [SerializeField] TMP_InputField NickNameInput;
    private Button ConnectButton;

    private NetworkManager theNetworkManager;
    private void Awake()
    {
        ConnectButton = GetComponent<Button>();
        theNetworkManager = FindObjectOfType<NetworkManager>();
    }
    void Update()
    {
        ConnectButton.interactable = (NickNameInput.text == string.Empty || !theNetworkManager.ConnectCheck) ? false : true;
    }
}
