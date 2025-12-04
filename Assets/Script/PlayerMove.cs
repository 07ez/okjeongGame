using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    [Header("이동 및 회전")]
    public float moveCooldown = 0.05f;
    public bool isMoving = false;
    public float moveTime = 0.05f;   // 한 칸을 이동하는 시간 (짧을수록 빠름)
    public float jumpHeight = 0.7f;  // 얼마나 위로 튀어오를지

    Quaternion targetRotation;
    public float rotateSpeed = 50f;

    void Update()
    {
        if (isMoving) return;

        // 이동
        Vector3 move = Vector3.zero;

        // 오른쪽 (D 또는 →)
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) && moveCooldown < 0f)
        {
            move = Vector3.right;
            targetRotation = Quaternion.Euler(0, 90, 0); // 회전
            moveCooldown = 0.1f;
        }

        // 왼쪽 (A 또는 ←)
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow) && moveCooldown < 0f)
        {
            move = Vector3.left;
            targetRotation = Quaternion.Euler(0, -90, 0); // 회전
            moveCooldown = 0.1f;
        }

        // 위쪽 (W 또는 ↑)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) && moveCooldown < 0f)
        {
            move = Vector3.forward;
            targetRotation = Quaternion.Euler(0, 0, 0); // 회전
            moveCooldown = 0.1f;

            // 길 생성
            MabSpawnManager.instance.SpawnLoad();
        }

        // 아래쪽 (S 또는 ↓)
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) && moveCooldown < 0f)
        {
            move = Vector3.back;
            targetRotation = Quaternion.Euler(0, -180, 0); // 회전
            moveCooldown = 0.1f;
        }

        // 한칸 이동
        if (move != Vector3.zero)
        {
            StartCoroutine(Move(move));
        }

        // 회전을 자연스럽게 보간
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotateSpeed
        );

        // 이동 쿨타임
        if (moveCooldown > 0f)
        {
            moveCooldown -= Time.deltaTime;
        }
    }
    IEnumerator Move(Vector3 move)
    {
        isMoving = true;

        Vector3 start = transform.position;
        Vector3 end = start + move;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / moveTime;

            // 기본 위치 보간
            Vector3 pos = Vector3.Lerp(start, end, t);

            // 위로 살짝 점프하는 곡선 : (sin πt) 형태
            pos.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;

            transform.position = pos;

            yield return null;
        }

        transform.position = end; // 마지막 위치 보강
        isMoving = false;
    }
}
