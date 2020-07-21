using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicItem : ShopItem {

    protected override void Awake() {
        base.Awake();
    }
    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    protected override void LoadIcon() {
        base.LoadIcon();
        
        // All music uses the same icon? (for now)
        // So just color it based on track
        if(_itemInfo.itemName.Contains("Seren")) {
            icon.color = new Color(0.097f, 0.76f, 0.1f);
        } else if(_itemInfo.itemName.Contains("Mount")) {
            icon.color = new Color(0.058f, 0.35f, 0.72f);
        } else if (_itemInfo.itemName.Contains("Conch")) {
            icon.color = new Color(0.92f, 0.75f, 0.6f);
        } else if (_itemInfo.itemName.Contains("City")) {
            icon.color = new Color(0.69f, 0.69f, 0.69f);
        } else if (_itemInfo.itemName.Contains("Corporation")) {
            icon.color = new Color(0.9f, 0.9f, 0.25f);
        } else if (_itemInfo.itemName.Contains("Laboratory")) {
            icon.color = new Color(0.7f, 0.2f, 0.8f);
        } else if (_itemInfo.itemName.Contains("Airship")) {
            icon.color = new Color(0.84f, 0.09f, 0.12f);
        }
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }
}
