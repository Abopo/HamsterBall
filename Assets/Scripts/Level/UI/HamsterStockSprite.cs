using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterStockSprite : MonoBehaviour {

    SpriteRenderer[] _sprites;

    private void Awake() {
        _sprites = GetComponentsInChildren<SpriteRenderer>();
    }
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Transparent() {
        foreach(SpriteRenderer s in _sprites) {
            s.color = new Color(s.color.r, s.color.g, s.color.b, 0.5f);
        }
    }

    public void FillIn() {
        foreach (SpriteRenderer s in _sprites) {
            s.color = new Color(s.color.r, s.color.g, s.color.b, 1f);
        }
    }
}
