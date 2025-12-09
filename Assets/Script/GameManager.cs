using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

    // Á¡¼ö
    public int score;
    public int maxScore;
    public TMP_Text scoreUI;

    private void Awake()
    {
        instance = this;
    }

    public void AddScore(int addScore)
    {
        score += addScore;
        scoreUI.text = $"{score}";
    }

    public void SetScore()
    {
        if (score > maxScore) 
            maxScore = score;
        score = 0;
        scoreUI.text = "0";
    }
}
