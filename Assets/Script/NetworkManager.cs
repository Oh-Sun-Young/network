using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Windows;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region 변수
    [Header("Singleton")]
    public static NetworkManager instance;

    [Header("Panel")]
    [SerializeField] GameObject IntroBackground;
    [SerializeField] GameObject IntroPanel;
    [SerializeField] GameObject DisconnectPanel;
    [SerializeField] GameObject RespawnPanel;
    [SerializeField] GameObject ControllerPanel;
    [SerializeField] GameObject ChattingPanel;
    [SerializeField] GameObject PostProcessingCanvas;

    [Header("Connect")]
    [SerializeField] TMP_InputField NickNameInput;
    [SerializeField] Button ConnectButton;
    [SerializeField] TextMeshProUGUI ConnectInfoText;
    public bool ConnectCheck;

    [Header("Play")]
    [SerializeField] GameObject[] Player;

    [Header("Chatting")]
    [SerializeField] TextMeshProUGUI[] ChattingText;
    [SerializeField] TMP_InputField ChattingInput;

    [Header("ETC")]
    [SerializeField] PhotonView PV;
    #endregion
    // Awake : 프로그램 실행시 최초로 한 번 실행
    private void Awake()
    {
        Screen.SetResolution(960, 540, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30; // 동기화가 더 빨리 될 수 있는 가능성
    }

    private void Start()
    {
        // 화면
        IntroBackground.SetActive(true);
        IntroPanel.SetActive(true);
        PostProcessingCanvas.SetActive(false);
        DisconnectPanel.SetActive(false);
        RespawnPanel.SetActive(false);
        ControllerPanel.SetActive(false);
        ChattingPanel.SetActive(false);

        // 소리
        SoundManager.Instance.OnIntro();

        ConnectCheck = true;
    }
    public void Update()
    {
        /*
        if (Input.GetKeyUp(KeyCode.Escape) && PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        */
    }

    #region 접속
    // PhotonNetwork.ConnectUsingSettings() : 바로 서버에 접속
    public void Connect()
    {
        // 중복 접속 시도를 막기 위해 접속 버튼 잠시 비활성화
        ConnectButton.interactable = false;
        ConnectCheck = false;

        // 마스터 서버에 접속 중이라면
        if (PhotonNetwork.IsConnected)
        {
            // 접속 시도 중임을 텍스트로 표시 (룸에 접속 중 / Connecting to room)
            ConnectInfoText.text = "룸에 접속 중";
            // 룸 접속 실행
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // 접속 시도 중임을 텍스트로 표시 (마스터 서버에 접속 중 / Connecting to master server)
            ConnectInfoText.text = "마스터 서버에 접속 중";
            // 마스터 서버로의 접속 시도
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    // 마스터 서버 접속 성공 시 자동 실행
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.LocalPlayer.NickName = NickNameInput.text;
        // 접속 정보 표시 (온라인 : 마스터 서버와 연결됨 / Online : Connected to master server)
        ConnectInfoText.text = "온라인 : 마스터 서버와 연결됨";
        // 룸 접속 실행
        PhotonNetwork.JoinRandomRoom();
    }

    // (빈 방이 없어) 랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        // 접속 정보 표시 (빈 방 없음, 새로운 방 생성 / Online : No room available, creating a new room)
        ConnectInfoText.text = "빈 방 없음, 새로운 방 생성";
        // 최대 6명을 수용 가능한 빈 방 생성
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 6 }, null);
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom()
    {
        // 접속 정보 표시 (방 참가 성공 / Successfully joined the room)
        ConnectInfoText.text = "방 참가 성공";

        // 화면
        PostProcessingCanvas.SetActive(false);
        IntroBackground.SetActive(false);
        DisconnectPanel.SetActive(false);
        RespawnPanel.SetActive(false);
        ControllerPanel.SetActive(true);
        ChattingPanel.SetActive(false);

        // 소리
        SoundManager.Instance.OnPlay();

        // 캐릭터 생성
        Spawn();

        // 채팅 초기화
        for (int i = 0; i < ChattingText.Length; i++)
        {
            ChattingText[i].gameObject.SetActive(false);
            ChattingText[i].text = string.Empty;
        }
    }

    public void Spawn()
    {
        int index = Random.Range(0, Player.Length) + 1;
        GameObject.Find("PostProcessCanvas").transform.Find("Blur").gameObject.SetActive(false);
        RespawnPanel.SetActive(false);
        ControllerPanel.SetActive(true);
        ControllerPanel.GetComponent<CanvasGroup>().alpha = 1;
        // 순서가 생성 후 ControllerPanel을 활성화시키면 캐릭터 컨트롤이 안 되는 이슈 있음
        PhotonNetwork.Instantiate("Player " + index, new Vector3(Random.Range(-25f, 25f), 4, 0), Quaternion.identity);

        // 채팅 초기화
        for (int i = 0; i < ChattingText.Length; i++)
        {
            ChattingText[i].gameObject.SetActive(false);
            ChattingText[i].text = string.Empty;
        }
    }

    // 마스터 서버 접속 실패 시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        DisconnectScreen();
        // 접속 정보 표시 (오프라인 : 마스터 서버와 연결되지 않음 / Offline : Not connected to master server)
        ConnectInfoText.text = "오프라인 : 마스터 서버와 연결되지 않음";
    }

    // 룸 접속 실패
    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }

    }

    public void DisconnectScreen()
    {
        // 화면
        PostProcessingCanvas.SetActive(true);
        IntroBackground.SetActive(true);
        IntroPanel.SetActive(false);
        DisconnectPanel.SetActive(true);
        RespawnPanel.SetActive(false);
        ControllerPanel.SetActive(false);
        ChattingPanel.SetActive(false);

        // 소리
        SoundManager.Instance.OnIntro();

        // 버튼
        ConnectButton.interactable = true;
        ConnectCheck = true;
    }
    #endregion
    #region 방
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        PV.RPC("ChattingRPC", RpcTarget.All, "<color=#4ec9b0>[system] " + newPlayer.NickName + "님이 참가하셨습니다.</color>");
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        PV.RPC("ChattingRPC", RpcTarget.All, "<color=#4ec9b0>[system] " + otherPlayer.NickName + "님이 퇴장하셨습니다.</color>");
    }
    #endregion
    #region 채팅
    public void Send()
    {
        string msg = PhotonNetwork.NickName + " : " + ChattingInput.text;
        PV.RPC("ChattingRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChattingInput.text);
        ChattingInput.text = string.Empty;
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다.
    void ChattingRPC(string msg)
    {
        bool isInput = false;
        for(int i = 0; i < ChattingText.Length; i++)
        {
            if (ChattingText[i].text == string.Empty)
            {
                isInput = true;
                ChattingText[i].gameObject.SetActive(true);
                ChattingText[i].text = msg;
                break;
            }
        }
        if(!isInput) // 꽉 차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChattingText.Length; i++)
            {
                ChattingText[i - 1].text = ChattingText[i].text;
            }
            ChattingText[ChattingText.Length - 1].text = msg;
        }
    }
    #endregion
}