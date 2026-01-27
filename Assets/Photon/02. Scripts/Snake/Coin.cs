using UnityEngine;

public class Coin : MonoBehaviour
{
    public float turnSpeed = 180f;
    
    private void Update()
    {
        transform.Rotate(turnSpeed * Time.deltaTime * Vector3.up);
    }
}
