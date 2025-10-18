using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    void Update()
    {
        transform.LookAt(player.transform);
        transform.position += transform.forward * 1f * Time.deltaTime;
    }
}
