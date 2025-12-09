using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("이동 및 회전")]
    public bool isMoving = false;
    public float moveTime = 0.05f;   // 한 칸을 이동하는 시간 (짧을수록 빠름)
    public float jumpHeight = 0.7f;  // 얼마나 위로 튀어오를지

    Quaternion targetRotation;
    public float rotateSpeed = 50f;

    // 아이템
    public GameObject shieldActive;
    public bool isGetShield = false;
    float itemTimer;

    private Coroutine moveCoroutine;

    private void Start()
    {
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (isMoving) return;

        // 이동
        Vector3 move = Vector3.zero;

        // 맵 밖으로 나가지 못하게
        bool canRightMove = true;
        bool canLeftMove = true;
        if (gameObject.transform.position.x > 3f)
            canRightMove = false;
        if (gameObject.transform.position.x < -3f)
            canLeftMove = false;

            // 오른쪽 (D 또는 →)
            if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && canRightMove)
        {
            move = Vector3.right;
            targetRotation = Quaternion.Euler(0, 90, 0); // 회전
        }

        // 왼쪽 (A 또는 ←)
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && canLeftMove)
        {
            move = Vector3.left;
            targetRotation = Quaternion.Euler(0, -90, 0); // 회전
        }

        // 위쪽 (W 또는 ↑)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            move = Vector3.forward;
            targetRotation = Quaternion.Euler(0, 0, 0); // 회전

            // 길 생성
            MabSpawnManager.instance.SpawnLoad();

            // 점수
            if (GameManager.instance.score < transform.position.z +1)
            {
                GameManager.instance.AddScore(1);
            }
        }

        // 아래쪽 (S 또는 ↓)
        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            move = Vector3.back;
            targetRotation = Quaternion.Euler(0, -180, 0); // 회전
        }

        // 한칸 이동
        if (move != Vector3.zero)
        {
            moveCoroutine = StartCoroutine(Move(move));
        }

        // 회전을 자연스럽게 보간
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotateSpeed
        );

    }

    private void FixedUpdate()
    {
        if (itemTimer > 0) {
            itemTimer -= Time.fixedDeltaTime;
        }
        else {
            shieldActive.SetActive(false);
            isGetShield = false;
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

    // 적과 닿았을시
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (isGetShield)
            {
                other.gameObject.SetActive(false);
                isGetShield = false;
                itemTimer = 0f;
            }
            else
            {
                Debug.Log("사망");
                // 스테이지 재시작
                StopCoroutine(moveCoroutine);
                isMoving = false;
                transform.position = new Vector3(0, -0.2f, -0.2f);
                GameManager.instance.SetScore(); 
            }
        }
        if (other.gameObject.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();

            switch (item.type)
            {
                case ItemType.Shield:
                    isGetShield = true;
                    shieldActive.SetActive(true);
                    itemTimer = 10f;
                    Destroy(other.gameObject);
                    break;
                case ItemType.Bomb:
                    other.GetComponent<Bomb>().Fire(transform.position);
                    break;
            }
        }
    }
}
