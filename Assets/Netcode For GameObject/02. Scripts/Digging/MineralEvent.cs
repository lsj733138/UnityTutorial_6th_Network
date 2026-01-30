using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MineralEvent : NetworkBehaviour
{
    private ScoreManager scoreManager;
    private bool isDrop = false;

    // 광물 드랍 후 1초가 지나야 획득 가능
    IEnumerator Start()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();
        
        isDrop = false;
        yield return new WaitForSeconds(1f);

        isDrop = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isDrop)
        {
            Debug.Log("광물 획득");
            GetMineralServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void GetMineralServerRpc()
    {
        scoreManager.AddScore();
        gameObject.SetActive(false);
        GetComponent<NetworkObject>().Despawn(); // 네트워트 상에서 아예 없어짐
    }
}
