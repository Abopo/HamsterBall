using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGFishSpawner : MonoBehaviour {
    GameObject bgFishObj;
    
    // Start is called before the first frame update
    void Start() {
        bgFishObj = Resources.Load("Prefabs/Effects/Environmental/SeaCreatures/BGFish") as GameObject;

        SpawnBGFish();
    }

    void SpawnBGFish() {
        GameObject fish;
        float randX, randY;
        int rand;
        for(int i = 0; i < 7; ++i) {
            fish = Instantiate(bgFishObj, transform);
            randX = Random.Range(-5f, 5f);
            randY = Random.Range(-1f, 1f);
            fish.transform.position = new Vector3(transform.position.x + randX, transform.position.y + randY, 2);

            randX = Random.Range(0.15f, 0.25f);
            fish.transform.localScale = new Vector3(randX, randX, randX);

            rand = Random.Range(0, 2);
            if(rand == 1) {
                fish.GetComponent<BGFish>().FaceRight();
            }
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
