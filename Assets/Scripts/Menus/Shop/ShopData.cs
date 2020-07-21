using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using UnityEngine;

[System.Serializable]
public struct ItemInfo {
    public string itemName;
    public int price;
    public string description;
    public bool unlocked;
    public bool purchased;

    public Sprite itemSprite;
}

[XmlRoot("ShopData")]
public class ShopData {

    [XmlArray("Palettes")]
    [XmlArrayItem("Palette")]
    public List<ItemInfo> paletteData = new List<ItemInfo>();

    [XmlArray("Music")]
    [XmlArrayItem("Track")]
    public List<ItemInfo> musicData = new List<ItemInfo>();

    public ItemInfo[] stageData;
    public ItemInfo[] onlineData;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void LoadShopData() {

    }

    public void Save(string path) {
        XmlSerializer serializer = new XmlSerializer(typeof(ShopData));
        using (FileStream stream = new FileStream(path, FileMode.Open)) {
            serializer.Serialize(stream, this);
        }
    }

    public static ShopData Load(string path) {
        XmlSerializer serializer = new XmlSerializer(typeof(ShopData));
        using (FileStream stream = new FileStream(path, FileMode.Open)) {
            return serializer.Deserialize(stream) as ShopData;
        }
    }
}
