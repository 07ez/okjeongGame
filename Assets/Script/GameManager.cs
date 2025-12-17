using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;

    // ¡°ºˆ
    public int score;
    public int maxScore;
    public TMP_Text scoreUI;
    public GameObject maxScoreText;

    // µ˚∂Ûø¿¥¬ ¿˚
    public GameObject[] FollowEnemy;

    // ƒ∑
    public GameObject mainCam;

    bool die = false;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (die && Input.anyKey)
        {
            die = false;
            UIManager.instance.ResetMap();
        }
    }

    public void AddScore(int addScore)
    {
        score += addScore;
        scoreUI.text = $"{score}";
    }

    public void ShowScore()
    {
        if (score > maxScore)
            maxScore = score;
        scoreUI.fontSize = 150;
        maxScoreText.GetComponent<TMP_Text>().text = $"≈È: {maxScore}";
        maxScoreText.SetActive(true);
        die = true;
    }

    public void ResetMap()
    {
        SetScore();
        maxScoreText.SetActive(false);
        MabSpawnManager.instance.AllTileReset();
        player.GetComponent<PlayerMove>().ResetMap();
        mainCam.transform.position = new Vector3(1, 7, -2);
        scoreUI.fontSize = 100;
        for (int i = 0; i < FollowEnemy.Length; i++)
        {
            FollowEnemy[i].GetComponent<BossMove>().ResetPos();
        }
    }

    private void SetScore()
    {
        score = 0;
        scoreUI.text = "0";
    }
}
