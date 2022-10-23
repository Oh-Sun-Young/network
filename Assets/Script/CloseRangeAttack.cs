using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseRangeAttack : MonoBehaviourPunCallbacks, IPunObservable
{
    private PlayerScript thePlayerScript;
    private AttackDamage theAttackDamage;
    private Animator AN;
    private PhotonView PV;

    void Awake()
    {
        thePlayerScript = GetComponent<PlayerScript>();
        theAttackDamage = GetComponentInChildren<AttackDamage>();
        AN = thePlayerScript.AN;
        PV = thePlayerScript.PV;
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            // 공격
            if (ButtonManager.instance.CheckAttack())
            {
                AN.SetTrigger("attack");
            }
        }
    }
    

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
