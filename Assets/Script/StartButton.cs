using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public List<MoveUI> moveUIList = new List<MoveUI>();
    public float duration = 0.3f;
    public bool useEaseOut = true;
    public TMP_InputField studentIdInput;
    public TMP_InputField nameInput;

    public void OnClick()
    {
        TriggerMoveOut(gameObject.name);
    }

    public void TriggerMoveOut(string ObjName = null)
    {
        StopAllCoroutines();
        StartCoroutine(MoveOutCoroutine(ObjName));
    }

    private IEnumerator MoveOutCoroutine(string ObjName)
    {
        if (!string.IsNullOrEmpty(ObjName) && ObjName == "RealStart")
        {
            string studentId = studentIdInput.text.Trim();
            string name = nameInput.text.Trim();

            if (studentId == "학번" || name == "이름")
            {
                Debug.Log("이름 없음");
                yield break;
            }

        }

        float t = 0f;

        Vector2[] starts = new Vector2[moveUIList.Count];
        Vector2[] ends = new Vector2[moveUIList.Count];

        // Start → End 계산
        for (int i = 0; i < moveUIList.Count; i++)
        {
            var ui = moveUIList[i];

            starts[i] = ui.target.anchoredPosition;

            // Direction: 1 = Out, -1 = In
            ends[i] = starts[i] + ui.offset;
        }

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            t = Mathf.Clamp01(t);

            float tt = useEaseOut ? (1f - Mathf.Pow(1f - t, 2f)) : t;

            for (int i = 0; i < moveUIList.Count; i++)
                moveUIList[i].target.anchoredPosition = Vector2.Lerp(starts[i], ends[i], tt);

            yield return null;
        }

        if (ObjName == "RealStart")
        {
            string studentId = studentIdInput.text.Trim();
            string name = nameInput.text.Trim();
            Debug.Log("학번: " + studentId + ", 이름: " + name);
            FirebaseManager.instance.SaveOrLoadPlayerData(name, studentId);
            yield return new WaitForSecondsRealtime(0.5f);
            SceneManager.LoadScene("InGameScene");
        }
        else if (ObjName == "LeaderBoard")
        {
            LeaderboardManager.instance.OpenLeaderboard();
        }
    }
}