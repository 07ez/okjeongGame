using System.Collections.Generic;
using UnityEngine;

public class MabSpawnManager : MonoBehaviour
{
    public GameObject[] Loads;
    public static MabSpawnManager instance;

    public List<GameObject> pool0 = new List<GameObject>(); // Load[0] 풀
    public List<GameObject> pool1 = new List<GameObject>(); // Load[1] 풀

    public Transform poolParent;

    [Header("길 생성 확률")]
    [SerializeField] private float initialRoadProbability = 0.5f; // 시작 확률 (50%)
    [SerializeField] private float maxRoadProbability = 0.9f;     // 최대 확률 (90%)
    [SerializeField] private int maxLevelForScaling = 250;        // 확률이 최대로 도달하는 레벨

    // 현재 생성 위치를 추적하는 변수
    private int spawnLocation = 0;

    private void Awake()
    {
        instance = this;
    }
    GameObject GetTile(int type)
    {
        List<GameObject> pool = (type == 0) ? pool0 : pool1;

        foreach (GameObject tile in pool)
        {
            if (!tile.activeSelf)
            {
                tile.SetActive(true);
                return tile;
            }
        }

        // 없으면 새로 생성
        GameObject newTile = Instantiate(Loads[type], poolParent);
        pool.Add(newTile);
        return newTile;
    }

    int GetTileType()
    {
        float t = Mathf.Min((float)spawnLocation / maxLevelForScaling, 1.0f);

        float roadProbability = Mathf.Lerp(initialRoadProbability, maxRoadProbability, t);
        float r = Random.value;

        spawnLocation++;

        return (r < roadProbability) ? 1 : 0;
    }

    public void SpawnLoad()
    {
        float z = GameManager.instance.player.transform.position.z;
        Vector3 spawnLoadPos = new Vector3(0, -0.7f, z + 20);
        Vector3 destroyLoadPos = new Vector3(0, -0.7f, z - 10);

        // 앞에 이미 있는지 검사
        bool needSpawn = true;

        foreach (GameObject tile in pool0)
            if (tile.activeSelf && Vector3.Distance(tile.transform.position, spawnLoadPos) < 0.1f)
                needSpawn = false;

        foreach (GameObject tile in pool1)
            if (tile.activeSelf && Vector3.Distance(tile.transform.position, spawnLoadPos) < 0.1f)
                needSpawn = false;

        if (needSpawn)
        {
            int type = GetTileType(); // 0 == load / 1 == warningLoad
            GameObject tile = GetTile(type);
            tile.transform.position = spawnLoadPos;
        }

        // 뒤 타일 정리
        foreach (GameObject tile in pool0)
            if (tile.activeSelf && tile.transform.position.z < destroyLoadPos.z)
                tile.SetActive(false);

        foreach (GameObject tile in pool1)
            if (tile.activeSelf && tile.transform.position.z < destroyLoadPos.z)
                tile.SetActive(false);
    }
}