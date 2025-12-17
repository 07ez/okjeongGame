using UnityEngine;
using System.Collections;

public class CameraMove : MonoBehaviour
{
    public Transform playerPos;
    Coroutine camCoroutine;

    private void Start()
    {
        playerPos = GameManager.instance.player.transform;
    }

    void Update()
    {
        Vector3 targetPos = new Vector3 (playerPos.position.x + 1, transform.position.y, playerPos.position.z -2);
        // 플래이어가 움직이고 있을때 실행
        if (GameManager.instance.player.GetComponent<PlayerMove>().isMoving)
        {
            // 카메라가 움직이고있으면 중지
            if (camCoroutine != null)
            { 
                StopCoroutine(camCoroutine);
            }
            camCoroutine = StartCoroutine(Move(targetPos));
        }

    }
    IEnumerator Move(Vector3 targetPos)
    {
        Vector3 start = transform.position;
        Vector3 end = targetPos;

        float t = 0f;
        while (t < 1f) {
            t += Time.deltaTime / 0.25f;

            float camSpeed = easeOutQuart(t);

            Vector3 pos = Vector3.Lerp(start, end, camSpeed);
            transform.position = pos;

            yield return null;
        }

        transform.position = end;
    }

    // 이동 속도 계산
    float easeOutQuart(float x)
    {
        return 1 - Mathf.Pow(1 - x, 4);
    }
}
