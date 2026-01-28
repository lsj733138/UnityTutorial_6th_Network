using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private int score;

    private void Start()
    {
        scoreText.text = $"현재 획득한 광물의 수 : {score}";
    }

    public void AddScore()
    {
        score++;
        scoreText.text = $"현재 획득한 광물의 수 : {score}";
    }
}
