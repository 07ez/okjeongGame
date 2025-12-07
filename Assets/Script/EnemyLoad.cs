using System.Threading;
using UnityEngine;

public class EnemyLoad : MonoBehaviour
{
    public GameObject[] barriers;      // 장애물들
    public Transform poolParent;

    // 길이 활송화 되었을때
    private void OnEnable()
    {
        // 1. 소환할 적 갯수 (1 ~ 3개)
        int spawnNum = Random.Range(1, 4);

        for (int i = 0; i < spawnNum; i++)
        {
            // 1. 스폰할 프리팹 선택
            GameObject prefab = barriers[Random.Range(1, barriers.Length)];

            // 2. Instantiate
            GameObject enemy = Instantiate(prefab, poolParent);
            enemy.transform.localPosition = new Vector3(Random.Range(-4, 5), 0.5f, 0);
            enemy.transform.localRotation = Quaternion.Euler(0, 180, 0);

            // 3. 소환된 객체의 Move 비활성화
            enemy.GetComponent<EnemyMove>().enabled = false;
        }
    }

    private void OnDisable()
    {
        
    }
}
