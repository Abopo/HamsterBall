using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIcon : MenuOption {

    public CHARACTERNAMES characterName;
    public bool isLocked;

    SpriteRenderer[] _sprites;

    // Use this for initialization
    protected override void Start() {
        base.Start();

        isLocked = false;
        _sprites = GetComponentsInChildren<SpriteRenderer>();

        if(!isReady) {
            Lock();
        }
    }

    public void Initialize(PlayerInfo pI) {
    }

    // Update is called once per frame
    protected override void Update() {
    }

    protected override void Select() {
        //base.Select();
    }

    public override void Highlight() {
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
