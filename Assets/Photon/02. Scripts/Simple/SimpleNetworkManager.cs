using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField nickNameInput;
    [SerializeField] private Button connectButton;
    
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.SendRate = 60; // 내 컴퓨터 게임 정보에 대한 전송률
        PhotonNetwork.SerializationRate = 30; // PhotonView에서 관측 중인 대상의 전송률

        PhotonNetwork.GameVersion = "1";
    }

    private void Start()
    {
        connectButton.onClick.AddListener(Connect);
    }

    private void Connect()
    {
        PhotonNetwork.NickName = nickNameInput.text;
        
        PhotonNetwork.ConnectUsingSettings();
        
        Debug.Log("서버 접속");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 20 }, null);
        Debug.Log("서버 접속 완료");
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel(1);   
    }
}
