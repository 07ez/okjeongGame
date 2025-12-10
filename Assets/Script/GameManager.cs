using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

    // 점수
    public int score;
    public int maxScore;
    public TMP_Text scoreUI;

    // 따라오는 적
    public GameObject[] FollowEnemy;

    private void Awake()
    {
        instance = this;
    }

    public void AddScore(int addScore)
    {
        score += addScore;
        scoreUI.text = $"{score}";
    }

    public void Reset()
    {
        SetScore();
        MabSpawnManager.instance.AllTileReset();
        for (int i = 0; i < FollowEnemy.Length; i++)
        {
            FollowEnemy[i].GetComponent<BossMove>().ResetPos();
        }
    }

    private void SetScore()
    {
        if (score > maxScore) 
            maxScore = score;
        score = 0;
        scoreUI.text = "0";
    }
}
