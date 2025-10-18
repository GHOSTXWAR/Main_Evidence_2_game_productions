using UnityEngine;

public class ManaSystem : MonoBehaviour 
{
    public float maxMana = 100f;
    public float MP = 50f;

    private void OnEnable()
    {
        if (MP > maxMana) MP = maxMana;
    }
    public void DrainMana(float manaCost)
    {
        if (MP <= 0)
        {
            MP = 0;
        }
        if (MP > 0)
        {
            MP -= (int)manaCost;
        }
    }

    public void RecoverMana(float mana)
    {
        if (MP < maxMana)
        {
            MP += (int)mana;
        }
        if (MP >= maxMana)
        {
            MP = maxMana;
        }
    }
}
