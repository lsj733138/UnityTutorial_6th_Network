using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public TextMeshProUGUI scoreText;

    private NetworkVariable<int> score = new NetworkVariable<int>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        scoreText.text = $"현재 획득한 광물의 수 : {score.Value}";
        //score.OnValueChanged += (prevValue, newValue) => scoreText.text = $"현재 획득한 광물의 수 : {newValue}";
        score.OnValueChanged += SetScore;
    }

    public void AddScore()
    {
        score.Value++;
    }
    
    private void SetScore(int prevValue, int newValue)
    {
        scoreText.text = $"현재 획득한 광물의 수 : {newValue}";
    }
    
}
