using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public bool isGetStar = false;
    float itemTimer;

    private Coroutine moveCoroutine;

    bool isDie = false;
    bool getStart = false;

    public GameObject boss; // 원래 여기서 하면 안돼는데 여기가 편함ㅎ

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
        if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && canRightMove && !isDie)
        {
            move = Vector3.right;
            targetRotation = Quaternion.Euler(0, 90, 0); // 회전
        }

        // 왼쪽 (A 또는 ←)
        if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && canLeftMove && !isDie)
        {
            move = Vector3.left;
            targetRotation = Quaternion.Euler(0, -90, 0); // 회전
        }

        // 위쪽 (W 또는 ↑)
        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && !isDie)
        {
            move = Vector3.forward;
            targetRotation = Quaternion.Euler(0, 0, 0); // 회전

            // 점수
            if (GameManager.instance.score < transform.position.z +1)
            {
                GameManager.instance.AddScore(1);
            }
        }

        // 아래쪽 (S 또는 ↓)
        if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !isDie)
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

        if (Input.GetKeyDown(KeyCode.Escape))
            SceneManager.LoadScene("StartZone");
    }

    public void ResetMap()
    {
        // 스테이지 재시작
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
        isMoving = false;
        UIManager.instance.UseItem();
        transform.position = new Vector3(0, -0.2f, -0.2f);
        transform.rotation = Quaternion.identity;
        getStart = false;
        isDie = false;
        boss.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (itemTimer > 0) {
            itemTimer -= Time.fixedDeltaTime;
        }
        else {
            shieldActive.SetActive(false);
            UIManager.instance.UseItem();
            isGetShield = false;
            isGetStar = false;
        }
    }
    IEnumerator Move(Vector3 move)
    {
        if (!getStart)
        {
            getStart = true;
            boss.SetActive(true);
        }

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

        // 길 생성
        MabSpawnManager.instance.SpawnLoad(transform.position.z);
    }
    IEnumerator DieAnim()
    {
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3(1, 6, -2);

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / 0.3f;

            // 기본 위치 보간
            Vector3 pos = Vector3.Lerp(start, end, t);
            Quaternion Rot = Quaternion.Lerp(Quaternion.identity, Quaternion.Euler(-540, -720, 1040), t);

            transform.position = pos;
            transform.rotation = Rot;

            yield return null;
        }
        GameManager.instance.ShowScore();
    }

    // 적과 닿았을시
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (isGetShield)
            {
                other.gameObject.SetActive(false);
                UIManager.instance.UseItem();
                isGetShield = false;
                itemTimer = 0f;
            }
            else if (isGetStar) {
                other.gameObject.SetActive(false);
            }
            else
            {
                isDie = true;
                StartCoroutine(DieAnim());
            }
        }
        if (other.gameObject.CompareTag("Boss"))
        {
            isDie = true;
            StartCoroutine(DieAnim());
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
                    UIManager.instance.GetItem(0);
                    break;
                case ItemType.Bomb:
                    other.GetComponent<Bomb>().Fire(transform.position);
                    itemTimer = 1f;
                    UIManager.instance.GetItem(1);
                    break;
                case ItemType.Clock:
                    itemTimer = 1.4f; // 5배 느려지게 해서 5초임
                    Destroy(other.gameObject);
                    StartCoroutine(DoSlowMotion());
                    UIManager.instance.GetItem(2);
                    break;
                case ItemType.Star:
                    isGetStar = true;
                    itemTimer = 3f;
                    Destroy(other.gameObject);
                    UIManager.instance.GetItem(3);
                    break;
            }
        }
    }
    IEnumerator DoSlowMotion()
    {
        // 느리게 만들기
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        // 물리 엔진도 같이 느려져야 부드러움

        yield return new WaitForSecondsRealtime(7f);
        // Realtime = timeScale 영향을 받지 않고 실제 시간 기준으로 7초 기다림

        // 원래대로
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

}
