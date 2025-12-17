using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager instance;

    DatabaseReference dbReference;
    private bool isFirebaseReady = false;

    public string userStudentName;
    public string userStudentId; // 로그인한 학번 기억
    public int userHighScore;    // 서버에서 불러온 최고 점수 기억


    void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Firebase 초기화 및 참조 주소 가져오기
        InitializeFirebase();
    }
    private void InitializeFirebase()
    {
        // Firebase 앱의 종속성을 확인하고 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                // 초기화 성공 시 참조 설정
                dbReference = FirebaseDatabase.GetInstance("https://okjeong-game-default-rtdb.firebaseio.com/").RootReference;
                isFirebaseReady = true;
                Debug.Log("Firebase가 정상적으로 초기화되었습니다.");
            }
            else
            {
                Debug.LogError($"Firebase 종속성 문제: {dependencyStatus}");
            }
        });
    }

    public void SaveOrLoadPlayerData(string name, string sId)
    {
        if (!isFirebaseReady || dbReference == null)
        {
            Debug.LogWarning("Firebase가 준비되지 않았습니다.");
            return;
        }

        // 1. 먼저 해당 학번(sId)의 데이터가 있는지 확인
        dbReference.Child("players").Child(sId).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("데이터베이스 접근 오류");
                return;
            }

            DataSnapshot snapshot = task.Result;

            if (snapshot.Exists)
            {
                // [CASE 1] 데이터가 이미 있는 경우 -> 점수 불러오기
                string json = snapshot.GetRawJsonValue();
                PlayerData existingData = JsonUtility.FromJson<PlayerData>(json);

                Debug.Log($"기존 유저 확인: {existingData.name}님, 최고점수 {existingData.highScore}점을 불러옵니다.");

                // 여기서 변수에 학번과 점수를 저장하거나 다음 씬으로 넘어가면 됩니다.
                userStudentName = existingData.name;
                userStudentId = existingData.studentId;
                userHighScore = existingData.highScore;
            }
            else
            {
                // [CASE 2] 데이터가 없는 경우 -> 새로 만들기
                userStudentName = name;
                userStudentId = sId;
                userHighScore = 0; // 새로 만들었으니 0점부터 시작!

                PlayerData newData = new PlayerData(name, sId, 0); // 새 유저는 점수 0점
                string newJson = JsonUtility.ToJson(newData);

                dbReference.Child("players").Child(sId).SetRawJsonValueAsync(newJson).ContinueWithOnMainThread(saveTask => {
                    if (saveTask.IsCompleted)
                    {
                        Debug.Log($"새 유저 생성 완료: {name}({sId})님 반갑습니다!");
                    }
                });
            }
        });
    }

    public void SaveGameData(string name, string sId, int score = 0)
    {
        if (!isFirebaseReady || dbReference == null)
        {
            Debug.LogWarning("Firebase가 아직 준비되지 않았습니다. 1~2초 후 다시 시도하세요.");
            return;
        }

        PlayerData newData = new PlayerData(name, sId, score);
        string json = JsonUtility.ToJson(newData);

        // players/학번 경로에 저장
        dbReference.Child("players").Child(sId).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                    Debug.Log($"<color=cyan>{name}님 데이터 저장 성공!</color>");
                else
                    Debug.LogError("저장 실패: " + task.Exception);
            });
    }

    public void GetTopRankings(int limit, System.Action<List<PlayerData>> callback)
    {
        dbReference.Child("players").OrderByChild("highScore").LimitToLast(limit)
        .GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsCompleted)
            {
                List<PlayerData> rankList = new List<PlayerData>();
                foreach (var child in task.Result.Children)
                {
                    rankList.Add(JsonUtility.FromJson<PlayerData>(child.GetRawJsonValue()));
                }
                rankList.Reverse(); // 높은 점수부터 나오게 뒤집기
                callback(rankList); // UI 스크립트에게 리스트 전달
            }
        });
    }
}

[System.Serializable]
public class PlayerData
{
    public string name;
    public string studentId;
    public int highScore;

    public PlayerData(string name, string sId, int score)
    {
        this.name = name;
        this.studentId = sId;
        this.highScore = score;
    }
}