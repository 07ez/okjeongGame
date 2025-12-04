using System.Collections.Generic;
using UnityEngine;

public class MabSpawnManager : MonoBehaviour
{
    public GameObject[] Loads;
    public static MabSpawnManager instance;
    public List<GameObject> loadPool = new List<GameObject>();
    private void Awake()
    {
        instance = this;
    }
    GameObject GetTile()
    {
        // 1. 풀에서 비활성화된 타일 찾기
        foreach (GameObject tile in loadPool)
        {
            if (!tile.activeSelf)
            {
                tile.SetActive(true);
                return tile;
            }
        }

        // 2. 없으면 새로 생성
        GameObject newTile = Instantiate(Loads[0]);
        loadPool.Add(newTile);
        return newTile;
    }

    public void SpawnLoad()
    {
        Vector3 spawnLoadPos = new Vector3(0, -0.7f, GameManager.instance.player.transform.position.z + 13);
        Vector3 destroyLoadPos = new Vector3(0, -0.7f, GameManager.instance.player.transform.position.z - 10);

        // 앞쪽에 타일이 없으면 추가 생성(또는 재사용)
        bool needSpawn = true;  

        // 앞쪽에 이미 타일이 있는지 검사 (리스트 기반)
        foreach (GameObject tile in loadPool)
        {
            if (tile.activeSelf && Vector3.Distance(tile.transform.position, spawnLoadPos) < 0.1f)
            {
                needSpawn = false;  // 이미 있음
                break;
            }
        }

        // 앞 타일 생성 or 재사용
        if (needSpawn)
        {
            GameObject tile = GetTile();
            tile.transform.position = spawnLoadPos;
        }

        // 뒤 타일 비활성화
        foreach (GameObject tile in loadPool)
        {
            if (tile.activeSelf && tile.transform.position.z < destroyLoadPos.z)
            {
                tile.SetActive(false);
            }
        }
    }
}