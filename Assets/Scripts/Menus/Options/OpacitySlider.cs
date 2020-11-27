using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpacitySlider : MenuSlider {

    public SuperTextMesh valueText;
    public Image _hamsterSprite;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();

        _slider.value = ES3.Load("OpacitySetting", 88);
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    public void UpdateOpacity() {
        valueText.text = _slider.value.ToString();

        Color color = _hamsterSprite.color;
        color.a = _slider.value/100f;
        _hamsterSprite.color = color;

        ES3.Save<int>("OpacitySetting", (int)_slider.value);
    }
}
