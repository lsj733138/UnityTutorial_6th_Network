using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class SnakeController : MonoBehaviourPun
{
    [SerializeField] private GameObject tailPrefab;
    [SerializeField] private List<Transform> tails = new List<Transform>();

    [SerializeField] private Transform coinTransform;
    private MeshRenderer headRenderer;

    private Vector3 moveInput;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float turnSpeed = 180f;
    [SerializeField] private float lerpSpeed = 5f;
    
    private bool isCoin = false;
    
    private void Start()
    {
        if (coinTransform == null)
            coinTransform = GameObject.FindGameObjectWithTag("Coin").transform;

        headRenderer = GetComponent<MeshRenderer>();

        if (photonView.IsMine)
        {
            headRenderer.material.color = Color.green;
        }
        else
        {
            headRenderer.material.color = Color.red;
        }
    }

    private void Update()
    {
        if (photonView.IsMine)
            MoveHead();
        
        MoveTail();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin") && !isCoin)
        {
            if (photonView.IsMine) // 먹은 본인만 실행
            {
                isCoin = true;
                photonView.RPC("MoveCoin", RpcTarget.MasterClient);
            }
        }
    }

    private void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    private void MoveHead()
    {
        transform.Translate(moveSpeed * Time.deltaTime * Vector3.up); // 머리가 향하는 방향으로 이동
        transform.Rotate(Vector3.forward, -turnSpeed * Time.deltaTime * moveInput.x); // A, D 누를 때 회전
    }

    private void MoveTail()
    {
        Transform target = transform; // 처음에는 Target을 snake로
        
        foreach (Transform tail in tails)
        {
            Vector3 pos = target.position;
            Quaternion rot = target.rotation;

            tail.position = Vector3.Lerp(tail.position, pos, lerpSpeed * Time.deltaTime);
            tail.rotation = Quaternion.Lerp(tail.rotation, rot, lerpSpeed * Time.deltaTime);

            target = tail; // 현재 꼬리를 Target 설정
        }
    }
    
    [PunRPC]
    public void AddTail()
    { 
        Vector3 spawnPosition = transform.position;
        if (tails.Count > 0)
            spawnPosition = tails[tails.Count - 1].position;
        
        GameObject newTail = Instantiate(tailPrefab, spawnPosition, Quaternion.identity);
        tails.Add(newTail.transform);

        newTail.GetComponent<Tail>().SetSnake(this, photonView); // 생성한 꼬리의 내 정보 저장

        // 내 꼬리와 상대 꼬리 색 다르게 지정
        MeshRenderer tailRenderer = newTail.GetComponent<MeshRenderer>();
        if (photonView.IsMine)
            tailRenderer.material.color = Color.green;
        else 
            tailRenderer.material.color = Color.red;
    }

    [PunRPC]
    private void MoveCoin()
    {
        float randX = Random.Range(-13f, 13f);
        float randY = Random.Range(-4f, 4f);
        Vector3 pos = new Vector3(randX, randY, 0);
        
        photonView.RPC("SetCoinPosition", RpcTarget.AllBufferedViaServer, pos);
    }

    [PunRPC]
    private void SetCoinPosition(Vector3 newPos)
    {
        if (coinTransform == null)
            coinTransform = GameObject.FindGameObjectWithTag("Coin").transform;
        
        coinTransform.position = newPos;
        
        if (photonView.IsMine)
            isCoin = false;

        AddTail();
    }

    [PunRPC]
    public void Death()
    {
        GetComponent<Collider>().enabled = false;
        headRenderer.material.color = Color.gray;
        
        foreach (var tail in tails)
            tail.gameObject.SetActive(false);
        
        this.enabled = false;
    }

    public int GetTailCount()
    {
        return tails.Count;
    }
}
