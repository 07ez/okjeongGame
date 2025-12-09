using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float moveTime = 0.7f;
    private Vector3 startPos;
    private Vector3 endPos;
    private bool moving = false;
    private float timer = 0f;
    public GameObject explosionPrefab;

    public void Fire(Vector3 start)
    {
        startPos = start;
        endPos = start + new Vector3(0, 0, 5f); // 5Ä­ ¾ÕÀ¸·Î
        timer = 0f;
        moving = true;
    }

    private void Update()
    {
        if (!moving) return;

        gameObject.tag = "Untagged";

        timer += Time.deltaTime / moveTime;

        float y = Mathf.Sin(timer * Mathf.PI) * 1.5f;
        transform.position = Vector3.Lerp(startPos, endPos, timer) + new Vector3(0, y, 0);

        if (timer >= 1f)
        {
            moving = false;
            Explode();
        }
    }

    private void Explode()
    {
        Vector3 pos = transform.position;
        Vector3 halfSize = new Vector3(1.5f, 1.5f, 1.5f);

        Collider[] hits = Physics.OverlapBox(pos, halfSize);

        foreach (Collider c in hits)
        {
            if (c.CompareTag("Enemy"))
                c.gameObject.SetActive(false);
        }
        GameObject explotion = Instantiate(explosionPrefab ,endPos, Quaternion.identity);
        Destroy(explotion, 1f);
        Destroy(gameObject);
    }
}
