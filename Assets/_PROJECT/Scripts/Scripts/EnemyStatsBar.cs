using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatsBar : MonoBehaviour 
{
    public GameObject Enemy;
    private HealthSystem healthSys;
    //private Image fillBar;
    private Camera playerCam;
    private Transform target;
    private TextMeshProUGUI healthText;
    [Header("To show health text, add a text box as a child of the image")]
    public Vector3 barOffset;
   
    
    private void Awake()
    {
        healthSys = Enemy.GetComponent<HealthSystem>();
        target = Enemy.GetComponent<RectTransform>();
        playerCam = Camera.main;
        if (GetComponentInChildren<TextMeshProUGUI>() != null)
        {
            healthText = GetComponentInChildren<TextMeshProUGUI>();
        }
        UpdateValue(GetComponent<Image>());
    }
    private void FixedUpdate()
    {
        transform.rotation = playerCam.transform.rotation;
        transform.position = target.position + barOffset;
    }

    public void UpdateValue(Image FillBar)
    {
        FillBar.fillAmount = Mathf.Clamp(healthSys.HP / (float)healthSys.maxHealth, 0f, 1f);
        if (healthText != null )
        {
            healthText.text = healthSys.HP.ToString() +"/"+healthSys.maxHealth.ToString();
        }
    }

}
