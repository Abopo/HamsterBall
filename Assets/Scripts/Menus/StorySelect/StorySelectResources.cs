using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorySelectResources : MonoBehaviour {

    public Sprite[] gameModes = new Sprite[3];
    public Sprite[] flowerSprites = new Sprite[3];
    public Sprite[] hangingFlowerSprites = new Sprite[3];

    private void Awake() {
        LoadFlowerSprites();
        LoadHangingFlowerSprites();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    void LoadFlowerSprites() {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/StorySelect/Demo-Story-masterfile");
        foreach (Sprite sp in sprites) {
            if (sp.name == "SunflowerSprout") {
                flowerSprites[0] = sp;
            } else if (sp.name == "SunflowerBud") {
                flowerSprites[1] = sp;
            } else if (sp.name == "SunflowerBloom") {
                flowerSprites[2] = sp;
            }
        }
    }


    void LoadHangingFlowerSprites() {
        Sprite[] sprites = Resources.LoadAll<Sprite>("Art/UI/StorySelect/Demo-Story-masterfile");
        foreach (Sprite sp in sprites) {
            if (sp.name == "HangingSprout") {
                hangingFlowerSprites[0] = sp;
            } else if (sp.name == "HangingBud") {
                hangingFlowerSprites[1] = sp;
            } else if (sp.name == "HangingBloom") {
                hangingFlowerSprites[2] = sp;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
