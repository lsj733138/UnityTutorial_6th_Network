using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPController : MonoBehaviourPun
{
    private Animator anim;
    private IHitbox hitbox;

    public TextMeshPro nickNameText;
    public Image hpBar;
    
    public float maxHp = 100f;
    public float currentHp = 100f;
    
    private void Start()
    {
        anim = GetComponent<Animator>();

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

        hpBar.fillAmount = currentHp / maxHp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentHp <= 0)
            return;
        
        hitbox = other.GetComponent<IHitbox>();

        if (hitbox != null)
        {
            photonView.RPC("OnGetDamage", RpcTarget.All, hitbox.Damage);
        }
    }

    [PunRPC]
    private void OnGetDamage(float damage)
    {
        currentHp -= damage;
        hpBar.fillAmount = currentHp / maxHp;
        
        if (currentHp <= 0)
            Death();
    }

    private void Death()
    {
        //Debug.Log($"{photonView.Owner.NickName} Die");
        
        anim.SetTrigger("Death");
        
        if (photonView.IsMine)
            Fade.onFadeAction?.Invoke(2f, Color.black, true);
        
        GetComponent<FightController>().SetDeath();
    }
}
