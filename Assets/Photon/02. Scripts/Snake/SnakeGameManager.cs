using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SnakeGameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject snakePrefab;
    [SerializeField] private GameObject coinPrefab;

    
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 60;
        PhotonNetwork.GameVersion = "1";
    }

    private void Start()
    {
        Connect();
    }

    private void Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 20 }, null);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient) // 호스트라면
            PhotonNetwork.Instantiate("Snake/Coin", RandomPosition(), Quaternion.identity);
            
        PhotonNetwork.Instantiate("Snake/Snake", RandomPosition(), Quaternion.identity);
    }

    private Vector3 RandomPosition()
    {
        float randX = Random.Range(-13f, 13f);
        float randY = Random.Range(-4f, 4f);

        return new Vector3(randX, randY, 0);
    }
}
