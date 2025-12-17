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
    public GameObject maxScoreText;

    // 따라오는 적
    public GameObject[] FollowEnemy;

    // 캠
    public GameObject mainCam;

    bool die = false;

    private void Awake()
    {
        instance = this;
        this.maxScore = FirebaseManager.instance.userHighScore;
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
        {
            maxScore = score;

            // [추가] Firebase 서버에도 새로운 최고 점수 저장 요청
            // FirebaseManager에 보관해둔 이름과 학번 정보를 사용합니다.
            string uName = FirebaseManager.instance.userStudentName; // FirebaseManager에 userName 변수도 추가되어 있어야 합니다.
            string uId = FirebaseManager.instance.userStudentId;

            if (!string.IsNullOrEmpty(uId)) // 학번 정보가 있을 때만 저장
            {
                FirebaseManager.instance.SaveGameData(uName, uId, maxScore);
                // 다음 게임을 위해 FirebaseManager 안의 점수도 최신화
                FirebaseManager.instance.userHighScore = maxScore;
            }
        }
        scoreUI.fontSize = 150;
        maxScoreText.GetComponent<TMP_Text>().text = $"Top: {maxScore}";
        maxScoreText.SetActive(true);
        die = true;
    }

    public void ResetMap()
    {
        score = 0;
        scoreUI.text = "0";
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
}
