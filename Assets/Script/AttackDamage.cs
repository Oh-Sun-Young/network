using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDamage : MonoBehaviour
{
    private PlayerScript thePlayerScript;
    private PhotonView PV;
    public bool attack;
    void Awake()
    {
        thePlayerScript = GetComponentInParent<PlayerScript>();
        PV = thePlayerScript.PV;
        attack = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!PV.IsMine && col.tag == "PlayerHP" && col.GetComponentInParent<PhotonView>().IsMine && attack) // 느린 쪽에 맞춰서 Hit 판정
        {
            attack = false;
            col.GetComponentInParent<PlayerScript>().Hit();
        }
    }
}
