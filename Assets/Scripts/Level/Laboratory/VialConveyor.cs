using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VialConveyor : MonoBehaviour {

    public Object vialObj;
    public float conveyorMoveSpd;

    GameObject[] _allVials = new GameObject[100];
    Sprite[] _vialSprites = new Sprite[6];

    float _vialSpacing = 0.368f;

    // Start is called before the first frame update
    void Start() {
        LoadVialSprites();

        SpawnVials();
    }

    void LoadVialSprites() {
        Sprite[] allsprites = Resources.LoadAll<Sprite>("Art/Levels/Laboratory/Laboratory-Stage-Final-Masterfile");
        int index = 0;
        foreach(Sprite s in allsprites) {
            if(s.name.Contains("Vial")) {
                _vialSprites[index] = s;
                index++;
            }
        }
    }

    void SpawnVials() {
        float startPosX = -18f;
        int rand = 0;
        for(int i = 0; i < 100; ++i) {
            // Spawn the vial object
            _allVials[i] = Instantiate(vialObj, new Vector3(startPosX + (_vialSpacing * i), transform.position.y, transform.position.z), Quaternion.identity, transform) as GameObject;
            
            // Give the vial a random color
            rand = Random.Range(0, 6);
            _allVials[i].transform.Find("Vial").GetComponent<SpriteRenderer>().sprite = _vialSprites[rand];
        }
    }

    // Update is called once per frame
    void Update() {
        // Move conveyor belt
        foreach(GameObject vial in _allVials) {
            vial.transform.Translate(conveyorMoveSpd * Time.deltaTime, 0f, 0f);
        }

        // If the last vial gets far enough, move it back to front of line
        if(_allVials[99].transform.position.x > 18f) {
            _allVials[99].transform.position = new Vector3(_allVials[0].transform.position.x - _vialSpacing, transform.position.y, transform.position.z);

            // Adjust array
            GameObject tempVial = _allVials[99];
            for(int i = 99; i > 0; --i) {
                _allVials[i] = _allVials[i - 1];
            }
            _allVials[0] = tempVial; 
        }
    }
}
