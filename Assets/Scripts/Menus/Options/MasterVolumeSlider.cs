using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent (typeof(Slider))]
public class MasterVolumeSlider : MenuOption {

    public Text volumeText;
    Slider _slider;

    // Use this for initialization
    protected override void Start() {
        base.Start();

        //_selectedPos = transform.parent.position;
        _slider = GetComponent<Slider>();
        _slider.value = AudioListener.volume * 100;
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if(isHighlighted) {
            // Right
            if (Input.GetAxis("Horizontal") > 0.3f || Input.GetKey(KeyCode.D)) {
                // Adjust slider to the right
                _slider.value += 1;
                if(_slider.value > 100) {
                    _slider.value = 100;
                }
            }
            // Left
            if (Input.GetAxis("Horizontal") < -0.3f || Input.GetKey(KeyCode.A)) {
                // Adjust slider to the left
                _slider.value -= 1;
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
        PlayerPrefs.SetFloat("MasterVolume", _slider.value);
    }
}