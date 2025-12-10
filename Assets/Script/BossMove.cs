using UnityEngine;

public class BossMove : MonoBehaviour
{
    public float PlayerPosZ;
    public float speed = 0.1f;

    private void Update()
    {
        PlayerPosZ = GameManager.instance.player.transform.position.z;
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, PlayerPosZ);
        // 플레이어와 너무 많은 차이가 났을때
        if (transform.position.z < PlayerPosZ - 4)
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed * 5);
        }
        else // 가까이 있을때
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed);
        }
    }

    public void ResetPos()
    {
        transform.position = new Vector3 (transform.position.x, transform.position.y, -5);
    }
}
