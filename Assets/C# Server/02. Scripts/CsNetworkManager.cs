using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CsNetworkManager : MonoBehaviour
{
    #region UniTask
    private TcpClient client;
    private NetworkStream stream;
    
    public TMP_InputField inputField;
    public Button sendButton;
    public TextMeshProUGUI chatLog;
    
    private bool isConnected = false;
    private CancellationTokenSource cts;
    
    private void Awake()
    {
        sendButton.onClick.AddListener(() => SendPacketAsync(inputField.text).Forget());
        cts = new CancellationTokenSource();
    }
    
    async void Start()
    {
        await ConnectToServerAsync();
    }

    private async UniTask ConnectToServerAsync()
    {
        try
        {
            client = new TcpClient();
            await client.ConnectAsync("127.0.0.1", 7979).AsUniTask().AttachExternalCancellation(cts.Token);
            stream = client.GetStream();
            isConnected = true;
            Debug.Log("[Step 1] 접속 성공");

            ReceiveLoopAsync(cts.Token).Forget();
        }
        catch (Exception e)
        {
            Debug.LogError($"접속 실패: {e.Message}");
        }
    }

    private async UniTaskVoid SendPacketAsync(string message)
    {
        if (!isConnected || string.IsNullOrEmpty(message)) return;

        try
        {
            await UniTask.SwitchToMainThread();
            if (chatLog != null)
                chatLog.text += $"\n[Client]: {message}";
            
            
            byte[] bodyData = Encoding.UTF8.GetBytes(message);
            short bodyLength = (short)bodyData.Length;
            byte[] headerData = BitConverter.GetBytes(bodyLength);

            byte[] fullPacket = new byte[headerData.Length + bodyData.Length];
            Array.Copy(headerData, 0, fullPacket, 0, headerData.Length);
            Array.Copy(bodyData, 0, fullPacket, headerData.Length, bodyData.Length);

            await stream.WriteAsync(fullPacket, 0, fullPacket.Length, cts.Token);
            inputField.text = "";
        }
        catch (Exception e)
        {
            Debug.LogError($"송신 에러: {e.Message}");
        }
    }

    private async UniTaskVoid ReceiveLoopAsync(CancellationToken token)
    {
        byte[] headerBuffer = new byte[2];

        try
        {
            while (isConnected && client.Connected)
            {
                int headerRead = await stream.ReadAsync(headerBuffer, 0, 2, token); // 스트림에 데이터가 올 때까지 대기
                if (headerRead <= 0) // 아무것도 안옴
                    break;

                short bodySize = BitConverter.ToInt16(headerBuffer, 0);

                byte[] bodyBuffer = new byte[bodySize]; // 헤더에서 받아온 bodySize만큼 buffer생성
                int totalRead = 0; // 지금까지 읽은 데이터 위치

                while (totalRead < bodySize)
                {
                    int read = await stream.ReadAsync(bodyBuffer, totalRead, bodySize - totalRead, token);
                    if (read <= 0) break;
                    totalRead += read; // 읽은 곳 까지 추가
                }

                string receivedText = Encoding.UTF8.GetString(bodyBuffer);
                
                chatLog.text += $"\n[Server]: {receivedText}";
                Debug.Log($"[Step 3] 수신 완료: {receivedText}");
            }
        }
        catch (Exception e) when (!(e is OperationCanceledException))
        {
            Debug.LogError($"수신 에러: {e.Message}");
        }
        finally
        {
            Disconnect();
        }
    }

    
    private async UniTaskVoid SendMessageToServer(string message)
    {
        try
        {
            byte[] body = Encoding.UTF8.GetBytes(message); // 전송하려는 메시지
            byte[] header = BitConverter.GetBytes((short)body.Length); // 메시지의 크기를 저장하는 헤더
    
            byte[] fullPacket = new byte[header.Length + body.Length];
            Array.Copy(header, 0, fullPacket, 0, header.Length); // 헤더 정보 복사
            Array.Copy(body, 0, fullPacket, header.Length, body.Length); // 바디 정보 복사
    
            stream.Write(fullPacket, 0, fullPacket.Length); // fullPacket을 0부터 패킷 끝까지 
            Debug.Log($"전송 성공 : {message} ({body.Length} bytes");
        }
        catch (Exception e)
        {
            Debug.Log($"전송 오류 : {e.Message}");
        }
    }
    
    private void Disconnect()
    {
        isConnected = false;
        stream?.Close();
        client?.Close();
        Debug.Log("연결 종료");
    }
    
    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
    }
    #endregion
    
    #region UniTask Test
    // private TcpClient client;
    // private bool isConnected = false;
    //
    // private CancellationTokenSource cts;
    //
    // private void Awake()
    // {
    //     cts = new CancellationTokenSource();
    // }
    //
    // async void Start()
    // {
    //     await ConnectToServerAsync();
    // }
    //
    // private async UniTask ConnectToServerAsync()
    // {
    //     try
    //     {
    //         client = new TcpClient();
    //         
    //         Debug.Log("[UniTask] 서버에 접속을 시도합니다...");
    //
    //         await client.ConnectAsync("127.0.0.1", 7979).AsUniTask().AttachExternalCancellation(cts.Token);
    //
    //         isConnected = true;
    //         Debug.Log($"[UniTask] 서버 접속 성공! (상태: {client.Connected})");
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError($"[UniTask] 서버 접속 실패: {e.Message}");
    //     }
    // }
    //
    // private void Disconnect()
    // {
    //     if (client != null)
    //     {
    //         client.Close();
    //         client = null;
    //         isConnected = false;
    //         Debug.Log("[UniTask] 서버와 연결 해제됨.");
    //     }
    // }
    //
    // private void OnDestroy()
    // {
    //     cts?.Cancel();
    //     cts?.Dispose();
    //     Disconnect();
    // }
    //
    // private void OnApplicationQuit()
    // {
    //     Disconnect();
    // }
    #endregion
}
