using System.Collections;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class FightController : MonoBehaviour
{
    private ThirdPersonController controller;
    private Animator anim;

    public GameObject punchHitbox, kickHitbox;
    
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        controller = GetComponent<ThirdPersonController>();
    }

    private void OnPunch(InputValue value)
    {
        StartCoroutine(PunchRoutine());
    }

    IEnumerator PunchRoutine()
    {
        controller.enabled = false;
        
        anim.SetTrigger("Punch");
        yield return new WaitForSeconds(0.5f);
        punchHitbox.SetActive(true);
        
        yield return new WaitForSeconds(0.3f);
        punchHitbox.SetActive(false);
        
        controller.enabled = true;
    }

    private void OnKick(InputValue value)
    {
        StartCoroutine(KickRoutine());
    }

    IEnumerator KickRoutine()
    {
        controller.enabled = false;
        
        anim.SetTrigger("Kick");
        yield return new WaitForSeconds(0.6f);
        kickHitbox.SetActive(true);
        
        yield return new WaitForSeconds(0.2f);
        kickHitbox.SetActive(false);
        
        controller.enabled = true;
    }

    public void SetDeath()
    {
        controller.enabled = false;
        this.enabled = false;
    }
}
