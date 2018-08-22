using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMarker : MonoBehaviour {
    public int team;
    public bool isFilledIn;

    Sprite _filledInSprite;

    SpriteRenderer _spriteRenderer;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start () {
        isFilledIn = false;
        _filledInSprite = Resources.LoadAll<Sprite>("Art/UI/Level UI/Tally")[3];
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void FillIn() {
        isFilledIn = true;
        _spriteRenderer.sprite = _filledInSprite;
    }
}
