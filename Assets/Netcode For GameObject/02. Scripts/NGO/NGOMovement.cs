using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NGOMovement : NetworkBehaviour
{
    private Vector3 moveInput;

    public float moveSpeed = 5f;
    public float turnSpeed = 10f;

    void Update()
    {
        if (IsOwner)
        {
            transform.position += moveInput * moveSpeed * Time.deltaTime;

            if (moveInput.magnitude > 0.1f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(moveInput);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, turnSpeed * Time.deltaTime);
            }
        }
    }

    void OnMove(InputValue value)
    {
        var input = value.Get<Vector2>();
        moveInput = new Vector3(input.x, 0, input.y);
    }
}
