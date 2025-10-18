using UnityEngine;
using UnityEngine.UI;

public class MovementVolumeControl : MonoBehaviour
{
    [SerializeField] private Slider MovementSlider;

    void Start()
    {
        if (SoundManager.Instance != null)
        {
           
           MovementSlider.value = SoundManager.Instance.movementVolume;
            
           MovementSlider.onValueChanged.AddListener(SoundManager.Instance.SetMovementVolume);
        }
    }
}
