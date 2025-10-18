using UnityEngine;
using UnityEngine.UI;

public class DamageHealing : MonoBehaviour 
{
  
    private HealthSystem healthSys;
    private ManaSystem manaSys;
    public int damage = 0;
    public enum DamageType {Heal,Deal,Mana}
    public DamageType damageType;

 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<HealthSystem>() != null)
        {
            healthSys = other.gameObject.GetComponent<HealthSystem>();
            switch (damageType)
            {
                case (DamageType.Heal):
                    healthSys.HealDamage(damage);
                    if (other.GetComponentInChildren<EnemyStatsBar>() != null)
                    {
                        other.GetComponentInChildren<EnemyStatsBar>().UpdateValue(other.GetComponentInChildren<Image>());
                        
                    }
                        break;
                case (DamageType.Deal):
                    healthSys.DealDamage(damage);
                    if (other.GetComponentInChildren<EnemyStatsBar>() != null)
                    {
                        other.GetComponentInChildren<EnemyStatsBar>().UpdateValue(other.GetComponentInChildren<Image>());
                    }
                        break;
            }
        }
        if (damageType == DamageType.Mana && other.gameObject.GetComponent<ManaSystem>() != null)
        {
            manaSys = other.gameObject.GetComponent<ManaSystem>();
            manaSys.RecoverMana(damage);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<HealthSystem>() != null)
        {
            healthSys = other.gameObject.GetComponent<HealthSystem>();
            switch (damageType)
            {
                case (DamageType.Heal):
                    healthSys.HealDamage(damage);
                    
                    if (other.gameObject.GetComponentInChildren<EnemyStatsBar>() != null)
                    {
                        other.gameObject.GetComponentInChildren<EnemyStatsBar>().UpdateValue(other.gameObject.GetComponentInChildren<Image>());

                    }
                    break;
                case (DamageType.Deal):
                    healthSys.DealDamage(damage);
                    if (other.gameObject.GetComponentInChildren<EnemyStatsBar>() != null)
                    {
                        other.gameObject.GetComponentInChildren<EnemyStatsBar>().UpdateValue(other.gameObject.GetComponentInChildren<Image>());
                    }
                    break;
                
                    
                    
            }
        }
        if (damageType == DamageType.Mana && other.gameObject.GetComponent<ManaSystem>() != null)
        {
            manaSys = other.gameObject.GetComponent<ManaSystem>();
            manaSys.RecoverMana(damage);
        }
    }
}
