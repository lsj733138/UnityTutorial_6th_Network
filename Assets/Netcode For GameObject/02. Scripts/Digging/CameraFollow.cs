using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;

    public Vector3 offset;

    private void LateUpdate()
    {
        if (target != null)
            transform.position = target.position + offset;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
