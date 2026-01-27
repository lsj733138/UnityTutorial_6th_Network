using Photon.Pun;
using UnityEngine;

public class Tail : MonoBehaviour
{
    private SnakeController mySnake;
    private PhotonView photonView;
    
    private void OnTriggerEnter(Collider other)
    {
        // 꼬리에 닿은 대상이 Snake일 때, 그 대상의 꼬리를 가져오는 기능
        SnakeController otherSnake = other.GetComponent<SnakeController>();

        if (otherSnake != null && mySnake != null) 
        {
            if (otherSnake != mySnake && photonView.IsMine)
            {
                int otherTailCount = otherSnake.GetTailCount();

                PhotonView otherPV = otherSnake.GetComponent<PhotonView>();

                otherPV.RPC("Death", RpcTarget.AllBufferedViaServer);
            
                for (int i = 0; i < otherTailCount; i++)
                    photonView.RPC("AddTail", RpcTarget.AllBufferedViaServer);
                
                Debug.Log($"뱀을 잡았습니다ㅣ. 획득 꼬리 개수 {otherTailCount}");
            }
        }
    }

    public void SetSnake(SnakeController snake, PhotonView photonView)
    {
        mySnake = snake;
        this.photonView = photonView;
    }
}
