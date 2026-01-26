using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleMovement : MonoBehaviourPun
{
    public TextMeshPro nickNameText;
    public GameObject hat;
    
    private Vector3 moveInput;

    public float moveSpeed = 5f;
    public float turnSpeed = 10f;

    private void Start()
    {
        if (photonView.IsMine)
        {
            nickNameText.text = PhotonNetwork.NickName;
            nickNameText.color = Color.green;
        }
        else
        {
            nickNameText.text = photonView.Owner.NickName;
            nickNameText.color = Color.red;
        }
    }

    void OnMove(InputValue value)
    {
        var input = value.Get<Vector2>();
        moveInput = new Vector3(input.x, 0, input.y);
    }

    void OnHatOn(InputValue value)
    {
        if (photonView.IsMine)
            photonView.RPC("SetHatRPC", RpcTarget.All, true);
    }

    void OnHatOff(InputValue value)
    {
        if (photonView.IsMine)
            photonView.RPC("SetHatRPC", RpcTarget.All, false);
    }

    [PunRPC]
    private void SetHatRPC(bool isActive)
    {
        hat.SetActive(isActive);
    }
    
    private void Update()
    {
        if (!photonView.IsMine) // 자신의 오브젝트가 아니면 리턴
            return;
        
        transform.position += moveSpeed * Time.deltaTime * moveInput;

        if (moveInput.magnitude > 0.1f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(moveInput);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
