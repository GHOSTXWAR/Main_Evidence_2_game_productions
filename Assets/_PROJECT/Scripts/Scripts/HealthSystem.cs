using UnityEngine;

public class HealthSystem : MonoBehaviour 
{
    public int maxHealth = 100;
    public int HP = 100;
    [SerializeField] private bool IsPlayer = true;
    [SerializeField] GameObject DeathScene;
    private void OnEnable()
    {
        if (HP > maxHealth) { HP = maxHealth; }
    }
    public void DealDamage(float damage)
    {
        if (HP > 0)
        {
            HP -= (int)damage;      
        }
        if (HP <= 0)
        {
            HP = 0;
            if (DeathScene != null) {DeathScene.GetComponent<SceneLoader>().LoadScene(); }
            if (!IsPlayer) Destroy(gameObject);
        }
    }
    public void HealDamage(float damage)
    {
        if (HP < maxHealth)
        {
            HP += (int)damage;
        }
        if (HP >= maxHealth)
        {
            HP = maxHealth;
        }
    }

}
