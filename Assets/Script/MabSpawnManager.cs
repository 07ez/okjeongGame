using System.Collections.Generic;
using UnityEngine;

public class MabSpawnManager : MonoBehaviour
{
    public GameObject[] Loads;
    public static MabSpawnManager instance;

    // 4개의 풀 (0,1,2,3)
    private List<GameObject>[] pools = new List<GameObject>[4];

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

        // 풀 초기화
        for (int i = 0; i < 4; i++)
            pools[i] = new List<GameObject>();
    }

    /// <summary>
    /// 풀링 시스템
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    GameObject GetTile(int type)
    {
        List<GameObject> pool = pools[type];

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

    /// <summary>
    /// load / warning 생성
    /// </summary>
    int GetTileType()
    {
        float t = Mathf.Min((float)spawnLocation / maxLevelForScaling, 1.0f);

        float roadProbability = Mathf.Lerp(initialRoadProbability, maxRoadProbability, t);
        float r = Random.value;

        spawnLocation++;

        return (r < roadProbability) ? 1 : 0;
    }
    /// <summary>
    /// 1/2 확률로 돌리기
    /// </summary>
    /// <param name="tile"> 돌릴타일 </param>
    void RotateTile(GameObject tile)
    {
        if (tile.CompareTag("WarningLoad") && Random.Range(0, 2) == 1)
            tile.transform.rotation = Quaternion.Euler(0, 180, 0);
        else
            tile.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    /// <summary>
    /// 길 어두운색 밝은색 결정
    /// </summary>
    /// <param name="loadType"> 길 타입 </param>
    /// <param name="z"> 플레이어 z 위치 </param>
    /// <returns></returns>
    int GetLoadColor(int loadType, float z)
    {
        // z 짝홀 계산 (floor 사용)
        int zIndex = Mathf.Abs(Mathf.FloorToInt(z) % 2); // 짝=0, 홀=1

        // 최종 인덱스 (길:0/1, 위험:2/3)
        return loadType * 2 + zIndex;
    }

    /// <summary>
    /// 타일 스폰
    /// </summary>
    public void SpawnLoad()
    {
        // 플레이어 z위치
        float z = GameManager.instance.player.transform.position.z;

        Vector3 spawnLoadPos = new Vector3(0, 0, z + 20.2f);
        Vector3 destroyLoadPos = new Vector3(0, 0, z - 10);

        // 앞에 이미 있는지 검사
        if (!NeedSpawn(spawnLoadPos))
            return;

        // Load / warning 선택
        int baseType = GetTileType(); // 0 또는 1

        // 최종 프리팹(0~3)
        int finalType = GetLoadColor(baseType, z);

        // 타일 스폰
        GameObject tile = GetTile(finalType);
        tile.transform.position = spawnLoadPos;

        // 타일 돌릴지 말지 정함
        RotateTile(tile);

        // 뒤 타일 정리
        CleanOldTiles(destroyLoadPos.z);
    }

    /// <summary>
    /// 스폰 할수있는가
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    bool NeedSpawn(Vector3 pos)
    {
        for (int i = 0; i < 4; i++)
        {
            foreach (GameObject tile in pools[i])
            {
                if (tile.activeSelf &&
                    Vector3.Distance(tile.transform.position, pos) < 0.1f)
                {
                    return false;
                }
            }
        }
        return true;
    }
    /// <summary>
    /// 뒷타일 제거
    /// </summary>
    /// <param name="playerZ"></param>
    public void CleanOldTiles(float playerZ)
    {
        float destroyZ = playerZ - 10f;

        for (int i = 0; i < 4; i++)
        {
            foreach (GameObject tile in pools[i])
            {
                if (tile.activeSelf && tile.transform.position.z < destroyZ)
                    tile.SetActive(false);
            }
        }
    }
}