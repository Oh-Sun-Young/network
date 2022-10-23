using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;
/*
* 참고
* - 오브젝트 텍스트 TextMeshPro, Color 변수 스크립트에서 선언하고 사용하기(Color, Color32 의 차이) : https://dlemrcnd.tistory.com/m/6
*/

public class PlayerScript : MonoBehaviourPunCallbacks, IPunObservable
{
    public Rigidbody2D RB;
    public Animator AN;
    public SpriteRenderer SR;
    public PhotonView PV;
    public TextMeshProUGUI NickNameText;
    public Slider HealthImage;

    private FixedJoystick joy;
    [SerializeField] float JumpCheckY;

    bool isGround; // 점프 계산
    bool checkDeath; // 죽었는 지 확인
    Vector3 curPos;

    void Awake()
    {
        // 닉네임
        NickNameText.text = PV.IsMine ? PhotonNetwork.NickName : PV.Owner.NickName;
        NickNameText.color = PV.IsMine ? new Color32(66, 144, 88, 255) : new Color32(156, 50, 70, 255);

        joy = FindObjectOfType<FixedJoystick>();

        if (PV.IsMine)
        {
            // 2D 카메라
            var CM = GameObject.Find("CMCamera").GetComponent<CinemachineVirtualCamera>();
            CM.Follow = transform;
            CM.LookAt = transform;

            checkDeath = false;
        }
    }
    void Update()
    {
        if (PV.IsMine)
        {
            if(0 < HealthImage.value)
            {
                // 이동
                float axis = joy.Horizontal;
                RB.velocity = new Vector2(4 * axis, RB.velocity.y); // Transform 으로 이동할 경우 막다른 벽을 뚫고 지나갈려고 할 때 떨리는 이슈 사항 발생

                if (axis != 0)
                {
                    PV.RPC("WalkRPC", RpcTarget.All, true);
                    PV.RPC("FlipXRPC", RpcTarget.AllBuffered, axis); // 재접속시 filpX를 동기화해주기 위해서 AllBuffered
                }
                else
                {
                    PV.RPC("WalkRPC", RpcTarget.All, false);
                }

                // 점프, 바닥 체크
                isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(0, -0.5f), JumpCheckY, 1 << LayerMask.NameToLayer("Ground"));
                AN.SetBool("jump", !isGround);
                if (ButtonManager.instance.CheckJump() && isGround)
                {
                    PV.RPC("JumpRPC", RpcTarget.All);
                }
            }
        }
        // IsMine이 아닌 것들을 부드럽게 위치 동기화
        else if ((transform.position - curPos).sqrMagnitude >= 100)
        {
            transform.position = curPos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, curPos, Time.deltaTime * 10);
        }
    }

    [PunRPC]
    void WalkRPC(bool active) => AN.SetBool("walk", active);

    [PunRPC]
    void FlipXRPC(float axis) => SR.flipX = axis == -1; // X축 반전

    [PunRPC]
    void JumpRPC()
    {
        RB.velocity = Vector2.zero;
        RB.AddForce(Vector2.up * 700);
    }

    public void Hit()
    {
        HealthImage.value -= 0.1f;
        if(HealthImage.value <= 0 && !checkDeath)
        {
            checkDeath = true;
            StartCoroutine(PlayerDeath());
        }
    }

    IEnumerator PlayerDeath()
    {
        PV.RPC("DieRPC", RpcTarget.All);
        yield return new WaitForSeconds(1.5f);
        GameObject.Find("PostProcessCanvas").transform.Find("Blur").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("RespawnPanel").gameObject.SetActive(true);
        GameObject.Find("Canvas").transform.Find("ControllerPanel").gameObject.SetActive(false);
        GameObject.Find("Canvas").transform.Find("ChattingPanel").gameObject.SetActive(false);
        PV.RPC("DestroyRPC", RpcTarget.AllBuffered); // AllBuffered로 해야 제대로 사라져 복제 버그가 안 생긴다.
    }

    [PunRPC]
    void DieRPC() => AN.SetTrigger("die");

    [PunRPC]
    void DestroyRPC() => Destroy(gameObject);

    // 위치, 체력 변수 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(HealthImage.value);
        }
        else
        {
            curPos = (Vector3)stream.ReceiveNext();
            HealthImage.value = (float)stream.ReceiveNext();
        }
    }
}