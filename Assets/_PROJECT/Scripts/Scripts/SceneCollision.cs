using UnityEngine;

public class SceneCollision : MonoBehaviour
{
    [SerializeField] private GameObject SceneObject;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            SceneObject.GetComponent<SceneLoader>().LoadScene();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        SceneObject.GetComponent<SceneLoader>().LoadScene();
    }
}
