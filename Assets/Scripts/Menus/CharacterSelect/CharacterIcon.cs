using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIcon : MenuOption {

    public CHARACTERS charaName;
    public bool isLocked;
    public SpriteRenderer backer;

    SpriteRenderer[] _sprites;
    Sprite _backerBase;
    Sprite _backerHighlighted;

    // Use this for initialization
    protected override void Start() {
        base.Start();

        isLocked = false;
        _sprites = GetComponentsInChildren<SpriteRenderer>();

        if(!isReady) {
            Lock();
        }

        Sprite[] spriteSheet = Resources.LoadAll<Sprite>("Art/UI/Character Select/Character-Portraits-and-windows");
        _backerBase = spriteSheet[25];
        _backerHighlighted = spriteSheet[26];
    }

    public void Initialize(PlayerInfo pI) {
    }

    // Update is called once per frame
    protected override void Update() {


        if (backer != null) {
            if (!isHighlighted && backer.sprite == _backerHighlighted) {
                backer.sprite = _backerBase;
            }
        }
    }

    protected override void Select() {
        //base.Select();
    }

    public override void Highlight() {
        isHighlighted = true;

        if (backer != null) {
            backer.sprite = _backerHighlighted;
        }
    }

    public void Lock() {
        isLocked = true;
        foreach(SpriteRenderer sr in _sprites) {
            sr.color = new Color32(128, 128, 128, 255);
        }
    }

    public void Unlock() {
        isLocked = false;
        foreach (SpriteRenderer sr in _sprites) {
            sr.color = new Color32(255, 255, 255, 255);
        }
    }
}
