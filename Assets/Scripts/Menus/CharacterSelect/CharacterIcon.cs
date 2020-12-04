using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIcon : MenuOption {

    public CHARACTERS charaName;
    public bool isLocked;
    public SpriteRenderer icon;
    public SpriteRenderer backer;

    SpriteRenderer[] _sprites;
    Sprite _backerBase;
    Sprite _backerHighlighted;


    protected override void Awake() {
        base.Awake();

        _sprites = GetComponentsInChildren<SpriteRenderer>();

        Sprite[] spriteSheet = Resources.LoadAll<Sprite>("Art/UI/Character Select/Character-Portraits-and-windows");
        _backerBase = spriteSheet[25];
        _backerHighlighted = spriteSheet[26];
    }
    // Use this for initialization
    protected override void Start() {
        base.Start();

        if(isLocked) {
            // Check if we should unlock
            switch(charaName) {
                case CHARACTERS.LACKEY:
                    if(ES3.Load("Lackey", false)) {
                        isLocked = false;
                    }
                    break;
                case CHARACTERS.CROC:
                    if(ES3.Load("Croc", false)) {
                        isLocked = false;
                    }
                    break;
            }

            // If we're still locked
            Lock();
        }
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

        if (backer != null && !isLocked) {
            backer.sprite = _backerHighlighted;
        }
    }

    public void Lock() {
        isLocked = true;

        // Turn off icon
        icon.enabled = false;

        // replace backer with locked
        backer.sprite = FindObjectOfType<CharacterSelectResources>().lockedBacker;

        //foreach(SpriteRenderer sr in _sprites) {
        //    sr.color = new Color32(128, 128, 128, 255);
        //}
    }

    public void Unlock() {
        isLocked = false;

        // Turn on icon
        icon.enabled = true;

        // replace backer
        backer.sprite = FindObjectOfType<CharacterSelectResources>().unlockedBacker;

        //foreach (SpriteRenderer sr in _sprites) {
        //    sr.color = new Color32(255, 255, 255, 255);
        //}
    }
}
