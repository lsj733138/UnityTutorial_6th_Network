using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : NetworkBehaviour
{
   public enum ActionType { Idle, Walk, Attack }
   public ActionType actionType = new ActionType();
   
   [SerializeField] private GameObject[] animObjs;

   // 네트워크에 동기화가 되는 위임자
   private NetworkVariable<ActionType> currentAnimStates = new NetworkVariable<ActionType>();
   
   private Rigidbody2D rb;

   private Vector3 moveInput;

   private float moveSpeed = 3f;
   private float jumpPower = 7f;

   public override void OnNetworkSpawn()
   {
      base.OnNetworkSpawn();
      
      rb = GetComponent<Rigidbody2D>();
      currentAnimStates.OnValueChanged += SetAnimObject;
      
      if (IsOwner)
      {
         CameraFollow cameraFollow = FindFirstObjectByType<CameraFollow>();
         cameraFollow.SetTarget(transform);
      }
   }

   public override void OnNetworkDespawn()
   {
      base.OnNetworkDespawn();
      
      currentAnimStates.OnValueChanged -= SetAnimObject;
   }

   private void FixedUpdate()
   {
      if (IsOwner)
         MovementServerRpc(moveInput);
   }

   // moveInput은 서버와 동기화되지 않았으니 변수로 전달
   [ServerRpc]
   private void MovementServerRpc(Vector3 moveInput)
   {
      if (currentAnimStates.Value == ActionType.Attack)
         return;
      
      // 좌우 움직임에 따라 스프라이트 좌우 전환
      if (moveInput.x != 0)
      {
         currentAnimStates.Value = ActionType.Walk;
         int scaleX = moveInput.x < 0 ? 1 : -1;
         transform.localScale = new Vector3(scaleX, 1, 1);
      
         rb.linearVelocityX = moveInput.x * moveSpeed; // 이동 기능
      }
      else
      {
         currentAnimStates.Value = ActionType.Idle;
         rb.linearVelocityX = 0f;
      }
   }
   
   private void OnMove(InputValue value)
   {
      var input = value.Get<Vector2>();
      moveInput = new Vector3(input.x, 0, 0);
   }

   
   private void OnJump(InputValue value)
   {
      if (IsOwner)
         JumpServerRpc();
   }

   [ServerRpc]
   private void JumpServerRpc()
   {
      rb.AddForceY(jumpPower, ForceMode2D.Impulse);
   }

   private void OnAttack(InputValue value)
   {
      if (IsOwner)
         AttackServerRpc();
   }

   [ServerRpc]
   private void AttackServerRpc()
   {
      if (currentAnimStates.Value == ActionType.Attack)
         return;
      
      StartCoroutine(AttackRoutine());
   }

   IEnumerator AttackRoutine()
   {
      currentAnimStates.Value = ActionType.Attack;
      yield return new WaitForSeconds(1f);
      
      currentAnimStates.Value = ActionType.Idle;
   }

   private void SetAnimObject(ActionType preType, ActionType newType)
   {
      for (int i = 0; i < animObjs.Length; i++)
         animObjs[i].SetActive((i == (int)newType));
   }
}
