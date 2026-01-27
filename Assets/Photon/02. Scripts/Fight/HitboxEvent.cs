using UnityEngine;

public class HitboxEvent : MonoBehaviour, IHitbox
{
    [field:SerializeField]
    public float Damage { get; set; }
    
    
}
