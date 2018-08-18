using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShiftMeter : MonoBehaviour {
    public SpriteRenderer GetIcon() {
        return transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    public Image GetMeterBack() {
        return transform.GetChild(1).GetComponent<Image>();
    }

    public Image GetMeterFront() {
        return transform.GetChild(2).GetComponent<Image>();
    }

    public SpriteRenderer GetMeterEnd() {
        return transform.GetChild(3).GetComponent<SpriteRenderer>();
    }
}
