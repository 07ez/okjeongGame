using System.Collections.Generic;
using UnityEngine;

public class EnemyLoad : MonoBehaviour
{
    public GameObject[] barriers;      // 장애물들
    public Transform poolParent;
    public GameObject[] inGameBarriers;

    // 길이 활성화 되었을때
    private void OnEnable()
    {
        List<int> pos = new List<int>() {-4, -3, -2, -1, 0, 1, 2, 3, 4};

        // 1. 소환할 적 갯수 (1 ~ 3개)
        int spawnNum = Random.Range(1, 4);
        inGameBarriers = new GameObject[spawnNum];

        // 리스트 섞기   
        Shuffle(pos);

        for (int i = 0; i < spawnNum; i++)
        {
            // 1. 스폰할 프리팹 선택
            GameObject prefab = barriers[Random.Range(0, barriers.Length)];

            // 2. 리스트 번호 뽑기
            int xPos = pos[0];
            pos.RemoveAt(0);

            // 3. Instantiate
            GameObject enemy = GetFromPool(prefab);
            enemy.transform.localPosition = new Vector3(xPos, 0, 0);
            enemy.transform.localRotation = Quaternion.Euler(0, 180, 0);
            enemy.SetActive(true);

            inGameBarriers[i] = enemy;
        }
    }

    private void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randIndex = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[randIndex];
            list[randIndex] = temp;
        }
    }

    private GameObject GetFromPool(GameObject prefab)
    {
        // 1. 이미 만들어진 같은 프리팹이 있는지 검사
        for (int i = 0; i < poolParent.childCount; i++)
        {
            GameObject obj = poolParent.GetChild(i).gameObject;

            if (!obj.activeSelf && obj.name.Contains(prefab.name))
                return obj;   // 비활성 오브젝트 재사용
        }

        // 2. 없으면 새로 만든다
        GameObject newObj = Instantiate(prefab, poolParent);
        newObj.SetActive(false);
        newObj.GetComponent<EnemyMove>().enabled = false;
        return newObj;
    }

    private void OnDisable()
    {
        for (int i = 0; i < inGameBarriers.Length; i++)
        {
            inGameBarriers[i].SetActive(false);
        }
    }
}
