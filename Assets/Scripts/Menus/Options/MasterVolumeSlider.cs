using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MasterVolumeSlider : MenuSlider {

    public SuperTextMesh volumeText;

    public int volumeType; // 0 - master, 1 - bgm, 2 - sfx

    FMOD.Studio.Bus MasterBus;
    FMOD.Studio.Bus MusicBus;
    FMOD.Studio.Bus SFXBus;

    // Use this for initialization
    protected override void Start() {

        base.Start();

        //_selectedPos = transform.parent.position;
        //_slider = GetComponentInChildren<Slider>();
        if (volumeType == 0) {
            _slider.value = ES3.Load("MasterVolume", 100f);
        } else if(volumeType == 1) {
            _slider.value = ES3.Load("BGMVolume", 100f);
        } else if(volumeType == 2) {
            _slider.value = ES3.Load("SFXVolume", 100f);
        }

        MasterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        MusicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override void Select() {
        base.Select();
    }

    public void UpdateVolumeValue() {
        volumeText.text = _slider.value.ToString();

        if(volumeType == 0) {
            AudioListener.volume = (_slider.value / 100);
            MasterBus.setVolume(AudioListener.volume);
            ES3.Save<float>("MasterVolume", _slider.value);
        } else if (volumeType == 1) {
            MusicBus.setVolume(_slider.value / 100);
            ES3.Save<float>("BGMVolume", _slider.value);
        } else if(volumeType == 2) {
            SFXBus.setVolume(_slider.value / 100);
            ES3.Save<float>("SFXVolume", _slider.value);
        }
    }
}