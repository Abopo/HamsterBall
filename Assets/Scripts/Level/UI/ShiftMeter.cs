using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShiftMeter : MonoBehaviour {
    public Image meterBar;
    public GameObject meterEndEffect;

    public SpriteRenderer GetIcon() {
        return transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public Image GetMeterFront() {
        return transform.GetChild(3).GetComponent<Image>();
    }

    public Image GetMeterEnd() {
        return transform.GetChild(2).GetComponent<Image>();
    }

    public void Full() {
        meterEndEffect.SetActive(true);
    }
    public void Empty() {
        meterEndEffect.SetActive(false);
    }
}
