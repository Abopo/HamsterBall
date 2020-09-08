using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MasterVolumeSlider : MenuOption {

    public Text volumeText;
    Slider _slider;

    FMOD.Studio.Bus MasterBus;
    FMOD.Studio.Bus MusicBus;
    FMOD.Studio.Bus SFXBus;

    // Use this for initialization
    protected override void Start() {

        base.Start();

        //_selectedPos = transform.parent.position;
        _slider = GetComponentInChildren<Slider>();
        _slider.value = AudioListener.volume * 100;

        MasterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
        MusicBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        SFXBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if(isHighlighted) {
            // Right
            if (_player.GetButtonDown("Right")) {
                // Adjust slider to the right
                _slider.value += 5;
                if(_slider.value > 100) {
                    _slider.value = 100;
                }
            }
            // Left
            if (_player.GetButtonDown("Left")) {
                // Adjust slider to the left
                _slider.value -= 5;
                if(_slider.value < 0) {
                    _slider.value = 0;
                }
            }
        }
    }

    protected override void Select() {
        base.Select();
    }

    public void UpdateVolumeValue() {
        AudioListener.volume = (_slider.value / 100);
        volumeText.text = _slider.value.ToString();
        //MusicBus.setVolume(AudioListener.volume);
        //SFXBus.setVolume(AudioListener.volume);
        MasterBus.setVolume(AudioListener.volume);
        ES3.Save<float>("MasterVolume", _slider.value);
    }
}