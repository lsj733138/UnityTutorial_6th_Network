using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectManager : MonoBehaviour
{
    [Header("Buttons")] [SerializeField] private Button hostButton;
    [SerializeField] private Button serverButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private Button closeButton;

    private void Start()
    {
        hostButton.onClick.AddListener(() => NetworkManager.Singleton.StartHost());
        serverButton.onClick.AddListener(() => NetworkManager.Singleton.StartServer());
        clientButton.onClick.AddListener(() => NetworkManager.Singleton.StartClient());
        closeButton.onClick.AddListener(() => NetworkManager.Singleton.Shutdown());

        NetworkManager.Singleton.OnServerStarted += ServerStarted;
        NetworkManager.Singleton.OnServerStopped += ServerStopped;
        NetworkManager.Singleton.OnClientStarted += ClientStarted;
    }

    private void ServerStarted()
    {
        Debug.Log("서버 시작");
    }

    private void ServerStopped(bool isBool)
    {
        Debug.Log("서버 종료");
    }

    private void ClientStarted()
    {
        Debug.Log("클라이언트 접속");
    }
}
