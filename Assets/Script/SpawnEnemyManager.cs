using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class SpawnEnemyManager : MonoBehaviour
{
    public GameObject[] enemys;
    public List<GameObject> enemyPool = new List<GameObject>();

    public Transform spawnDot;
    public Transform destroyDot;

    public Transform poolParent;

    private Coroutine enemyRoutine;

    void OnEnable()
    {
        if (enemyRoutine == null)
            enemyRoutine = StartCoroutine(EnemyLoop());
    }

    void OnDisable()
    {
        if (enemyRoutine != null)
        {
            StopCoroutine(enemyRoutine);
            enemyRoutine = null;
        }
    }


    GameObject GetEnemy(int choosenEnemyNum)
    {
        // 1. 풀에서 비활성화된 적 찾기
        foreach (GameObject enemy in enemyPool)
        {
            if (!enemy.activeSelf)
            {
                enemy.SetActive(true);
                enemy.transform.position = spawnDot.position;

                enemy.GetComponent<EnemyMove>().endPos = destroyDot.position.x;
                return enemy;
            }
        }

        // 2. 없으면 새로 생성
        GameObject newEnemy = Instantiate(enemys[choosenEnemyNum], spawnDot.position, Quaternion.identity, poolParent);
        enemyPool.Add(newEnemy);

        // 적 기본 세팅

        // 1) 끝 위치 전달
        newEnemy.GetComponent<EnemyMove>().endPos = destroyDot.position.x;
        return newEnemy;
    }

    IEnumerator EnemyLoop()
    {
        while (true)
        {
            // 소환할 적을 정함 (랜덤)
            // 0 ~ 3 중 숫자를 정함
            int choosenEnemy = Random.Range(0, enemys.Length);
            // 적 생성
            GetEnemy(choosenEnemy);

            // 1~3초 뒤에 재실행
            float wait = Random.Range(1.2f, 2.4f);
            yield return new WaitForSeconds(wait);
        }
    }
}
