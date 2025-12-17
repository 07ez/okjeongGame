using System.Collections.Generic;
using UnityEngine;
using TMPro; // TMP 사용 시 필수

public class LeaderboardManager : MonoBehaviour
{
    public GameObject rankItemPrefab; // 아까 만든 RankItem 프리팹
    public Transform contentParent;   // Scroll View의 Content 위치

    public static LeaderboardManager instance;

    public void Awake()
    {
        instance = this;
    }
    public void OpenLeaderboard()
    {
        // 기존에 생성되어 있던 리스트들 삭제 (새로고침 효과)
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Firebase에서 데이터 가져오기 (상위 10명)
        FirebaseManager.instance.GetTopRankings(10, (rankList) => {
            DisplayRanking(rankList);
        });
    }

    void DisplayRanking(List<PlayerData> rankList)
    {
        int rank = 1;
        foreach (var data in rankList)
        {
            // 프리팹 생성
            GameObject item = Instantiate(rankItemPrefab, contentParent);

            // 프리팹 안에 있는 텍스트들 찾아서 값 넣어주기
            // (GetChild 번호는 본인이 만든 순서에 맞게 조절하세요)
            item.transform.GetChild(0).GetComponent<TMP_Text>().text = rank.ToString() + "등"; // 순위
            item.transform.GetChild(1).GetComponent<TMP_Text>().text = data.name;       // 이름
            item.transform.GetChild(2).GetComponent<TMP_Text>().text = data.highScore.ToString() + "점"; // 점수

            rank++;
        }
    }
}