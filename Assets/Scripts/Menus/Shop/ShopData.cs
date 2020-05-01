using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemInfo {
    public string itemName;
    public int itemCost;
    public string itemDescription;
    public Sprite itemSprite;
    public bool purchased;
}

public class ShopData : MonoBehaviour {
    public ItemInfo[] paletteData;
    public ItemInfo[] stageData;
    public ItemInfo[] onlineData;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
