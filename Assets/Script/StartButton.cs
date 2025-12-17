using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public List<MoveUI> moveUIList = new List<MoveUI>();
    public float duration = 0.3f;
    public bool useEaseOut = true;

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
        float t = 0f;

        Vector2[] starts = new Vector2[moveUIList.Count];
        Vector2[] ends = new Vector2[moveUIList.Count];

        // Start ¡æ End °è»ê
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

        if (!string.IsNullOrEmpty(ObjName) && ObjName == "Start")
        {
            yield return new WaitForSecondsRealtime(0.5f);
            SceneManager.LoadScene("InGameScene");
        }
    }
}