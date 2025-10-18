using UnityEngine;
using UnityEngine.UI;

public class SFXVolumeControl : MonoBehaviour
{
    [SerializeField] private Slider sfxSlider;

    void Start()
    {
        if (SoundManager.Instance != null)
        {
           
            sfxSlider.value = SoundManager.Instance.sfxVolume;
            
            sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetSFXVolume);
        }
    }
}
