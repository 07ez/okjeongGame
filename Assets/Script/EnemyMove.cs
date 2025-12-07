using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float startPos;
    public float endPos;
    public bool islive = false;
    float speed;
    bool goingRight;

    void OnEnable()
    {
        // 초기 설정
        islive = true;  // 살아남

        // 방향 결정 (시작점 > 끝점)
        goingRight = endPos > startPos;
        if (goingRight)
            transform.rotation = Quaternion.Euler(0, 90, 0);
        else
            transform.rotation = Quaternion.Euler(0, -90, 0);

        // 속도 조정
        speed = Random.Range(7f, 9f); 
    }

    private void OnDisable()
    {
        // 비활성화
        islive = false;
    }

    void Update()
    {
        if (islive)
        {
            // 앞으로 쭉 이동
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            // 끝지점 넘으면 비활성화
            if (goingRight)
            {
                if (transform.position.x > endPos)
                    gameObject.SetActive(false);
            }
            else
            {
                if (transform.position.x < endPos)
                    gameObject.SetActive(false);
            }
        }
    }
}
