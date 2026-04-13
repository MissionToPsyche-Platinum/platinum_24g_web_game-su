using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMixerManager : MonoBehaviour
{

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider fxSlider;

    private void Start()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            LoadMasterVolume();
        }
        else
        {
            SetMasterVolume();
        }

        if (PlayerPrefs.HasKey("soundFXVolume"))
        {
            LoadSoundFXVolume();
        }
        else
        {
            SetSoundFXVolume();
        }
    }
    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat("masterVolume", volume);
    }

    public void SetSoundFXVolume()
    {
        float volume = fxSlider.value;
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat("soundFXVolume", volume);

    }

    private void LoadMasterVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
        SetMasterVolume();
    }
    private void LoadSoundFXVolume()
    {
        fxSlider.value = PlayerPrefs.GetFloat("soundFXVolume");
        SetSoundFXVolume();
    }
}
