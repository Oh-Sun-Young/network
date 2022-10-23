using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{
    public static ButtonManager instance 
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<ButtonManager>();
            }
            return m_instance;
        }
    }
    private static ButtonManager m_instance;

    private bool jump;
    private bool attack;

    private void Awake()
    {
        if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void JumpButtonUp()
    {
        jump = false;
    }
    public void JumpButtonDown()
    {
        jump = true;
    }

    public bool CheckJump()
    {
        return jump;
    }
    public void AttackButtonUp()
    {
        attack = false;
    }
    public void AttackButtonDown()
    {
        attack = true;
    }

    public bool CheckAttack()
    {
        return attack;
    }

    public void Disconnect()
    {
        NetworkManager theNetworkManager = FindObjectOfType<NetworkManager>();
        theNetworkManager.DisconnectScreen();
    }

    public void ObjectEnable(GameObject obj)
    {
        obj.SetActive(true);
    }
    public void ObjectDisable(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
