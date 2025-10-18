using UnityEngine;

public class SPSystem : MonoBehaviour 
{
    public float SP = 100.0f;
    public float spRecoveryRate = 0.2f;
    public float spDepletionRate = 0.2f;
    public float maxSP = 100f;
    public bool isSPCooldown = false;
    public bool isSprinting = false;

    private void OnEnable()
    {
        if (SP > maxSP) { SP = maxSP; }
    }
    private void FixedUpdate()
    {
        if (SP <= 0)
        {
            isSPCooldown = true;
        }
        if (isSprinting)
            SP -= spDepletionRate;
        if ((SP < maxSP) && !isSprinting)
            SP += spRecoveryRate;
        else if (SP > maxSP)
        {
            isSPCooldown = false;
            SP = maxSP;
        }
    }
    
}
