using UnityEngine;

public class ItemLoad : MonoBehaviour
{
    public GameObject[] items;      // 장애물들
    public Transform poolParent;
    public GameObject inGameitems;

    // 길이 활성화 되었을때
    private void OnEnable()
    {
        // 아이템을 확률에 따라 소환할지 말지 정함
        if (Random.Range(0f, 1f) < 0.05f) // 5%
            SpawnItem();
    }

    private void OnDisable()
    {
        if (inGameitems != null)
            Destroy(inGameitems);
    }

    void SpawnItem()
    {
        // 어떤 아이템 스폰할지 정하고 스폰
        inGameitems = Instantiate(items[Random.Range(0, items.Length)], poolParent);
        inGameitems.transform.localPosition = new Vector3(Random.Range(-4, 5), 0, 0);
    }
}
