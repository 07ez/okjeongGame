using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Vector3 startPos;
    public float endPos;
    public bool islive = false;
    float speed;

    void OnEnable()
    {
        // 초기 설정
        transform.position = startPos;
        islive = true;  // 살아남
        transform.rotation = Quaternion.Euler(0, 90, 0); // 바라보는 방향

        speed = Random.Range(5f, 7f); // 속도 조정
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
            if (transform.position.x > endPos)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
