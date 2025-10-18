using UnityEngine;
using UnityEngine.UI;

public class MusicVolumeControl : MonoBehaviour
{
    [SerializeField] private Slider MusicSlider;

    void Start()
    {
        if (SoundManager.Instance != null)
        {

            MusicSlider.value = SoundManager.Instance.musicVolume;

            MusicSlider.onValueChanged.AddListener(SoundManager.Instance.SetMusicVolume);
        }
    }
}
