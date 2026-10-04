using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    private const float MinimumLinearVolume = 0.0001f;

    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private void Start()
    {
        float master = LoadVolume("MasterVolume");
        float music = LoadVolume("MusicVolume");
        float sfx = LoadVolume("SFXVolume");

        // Initialising a Slider through value invokes its persistent callback.
        // That caused the saved value to be written back while the layout and
        // controls were still being initialised, which could visibly move the
        // handle. Apply UI state and mixer state as two explicit steps instead.
        masterSlider.SetValueWithoutNotify(master);
        musicSlider.SetValueWithoutNotify(music);
        sfxSlider.SetValueWithoutNotify(sfx);

        ApplyMixerVolume("MasterVolume", master);
        ApplyMixerVolume("MusicVolume", music);
        ApplyMixerVolume("SFXVolume", sfx);
    }

    public void SetMasterVolume(float value)
    {
        SetVolume("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        SetVolume("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SetVolume("SFXVolume", value);
    }

    private static float LoadVolume(string preferenceKey)
    {
        return Mathf.Clamp(
            PlayerPrefs.GetFloat(preferenceKey, 1f),
            MinimumLinearVolume,
            1f
        );
    }

    private void SetVolume(string parameterName, float value)
    {
        float clampedValue = Mathf.Clamp(
            value,
            MinimumLinearVolume,
            1f
        );

        ApplyMixerVolume(parameterName, clampedValue);
        PlayerPrefs.SetFloat(parameterName, clampedValue);
    }

    private void ApplyMixerVolume(string parameterName, float value)
    {
        if (audioMixer == null)
        {
            Debug.LogWarning("Audio mixer has not been assigned.", this);
            return;
        }

        audioMixer.SetFloat(
            parameterName,
            Mathf.Log10(Mathf.Max(value, MinimumLinearVolume)) * 20f
        );
    }

    private void OnDisable()
    {
        PlayerPrefs.Save();
    }
}
