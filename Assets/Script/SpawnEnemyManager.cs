using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SpawnEnemyManager : MonoBehaviour
{
    public GameObject[] enemys;
    public List<GameObject> enemyPool = new List<GameObject>();

    public Transform spawnDot;
    public Transform destroyDot;

    void Start()
    {
        StartCoroutine(EnemyLoop());
    }

    GameObject GetEnemy(int choosenEnemyNum)
    {
        // 1. 풀에서 비활성화된 적 찾기
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeSelf)
            {
                enemy.SetActive(true);
                return enemy;
            }
        }

        // 2. 없으면 새로 생성
        GameObject newEnemy = Instantiate(enemys[choosenEnemyNum]);
        enemyPool.Add(newEnemy);

        // 적 기본 세팅
        // 1) 생성 위치로
        newEnemy.transform.position = spawnDot.position;

        // 2) 처음, 끝 위치 전달
        newEnemy.GetComponent<EnemyMove>().startPos = spawnDot.position;
        newEnemy.GetComponent<EnemyMove>().endPos = destroyDot.position.x;
        return newEnemy;
    }

    IEnumerator EnemyLoop()
    {
        while (true)
        {
            // 소환할 적을 정함 (랜덤)
            // 0 ~ 3 중 숫자를 정함
            int choosenEnemy = Random.Range(0, 4);
            // 적 생성
            GetEnemy(choosenEnemy);

            // 1~2초 뒤에 재실행
            float wait = Random.Range(1f, 2f);
            yield return new WaitForSeconds(wait);
        }
    }
}
