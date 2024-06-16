using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider VolumeSlider; 

    private void Start()
    {
        if (VolumeSlider != null)
        {
            VolumeSlider.onValueChanged.AddListener(SetVolume);
            VolumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f); 
            AudioListener.volume = VolumeSlider.value;  
        }
    }

    private void SetVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);  
    }

}