using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer master;
    [SerializeField] private AudioSource BGM;
    [SerializeField] private AudioSource[] SFX;

    private float bgmDefaultVolume = 0.025f;
    private float[] sfxDefaultVolume = new float[]{0.154f, 1};
    public void SetMasterVolume (float volume){
        master.SetFloat("MasterVolume", volume);
    }

    public void SetSFXVolume (float volume){
        for (int i = 0; i < SFX.Length; i++)
        {
            SFX[i].volume = volume * sfxDefaultVolume[i];
        }
    }

    public void SetBGMVolume (float volume){
        BGM.volume = volume*bgmDefaultVolume;
    }

    public void SetQuality (int qualityIndex){
        QualitySettings.SetQualityLevel(qualityIndex);
    }
}
