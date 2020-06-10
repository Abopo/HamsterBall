using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HAMSTERROOMS { CHARACTER = 0, SHOP, OPTIONS, MIDDLE, MUSHROOM, LEFT, NETWORK, VERSUS, STORY, NUM_ROOMS };

// Wandering hamsters enter/exit these locations
public class HamsterRoom : MonoBehaviour {
    public HAMSTERROOMS room;
    public int hamsterLimit; // How many hamsters are allowed in this room
    public int contHamCount; // How many hamsters are actually contained
    public int hamCount; // How many hamsters this room has (contained + targetted)

    float _releaseTime = 2.0f;
    float _releaseTimer = 0f;

    protected WanderingHamster[] _containedHamsters = new WanderingHamster[2];

    public bool IsFull {
        get { return hamCount >= hamsterLimit; }
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if(contHamCount > 0) {
            // Maybe release a hamster
            _releaseTimer += Time.deltaTime;
            if(_releaseTimer >= _releaseTime) {
                // Chance to release a hamster
                if(Random.Range(0, 4) == 0) {
                    ReleaseHamster();
                }

                _releaseTimer = 0f;
            }
        }
    }

    public void TakeHamster(WanderingHamster wHamster) {
        // Hold onto the hamster
        _containedHamsters[contHamCount] = wHamster;

        // Increase our hamster count
        contHamCount++;

        // Make the wHamster disappear
        wHamster.gameObject.SetActive(false);
    }

    void ReleaseHamster() {
        // Release the first hamster that came in
        _containedHamsters[0].gameObject.SetActive(true);

        // Have the hamster exit the room
        _containedHamsters[0].ExitRoom();

        // Direct the hamster
        DirectHamster();

        // Reduce hamster counts
        contHamCount--;
        hamCount--;

        // Move other hamster into first slot
        _containedHamsters[0] = _containedHamsters[1];
        _containedHamsters[1] = null;
    }

    protected virtual void DirectHamster() {

    }

    protected void FaceHamsterLeft() {
        // Hamster is heading left
        // So set the hamster to our y position
        _containedHamsters[0].transform.position = new Vector3(transform.position.x,
                                                                transform.position.y,
                                                                _containedHamsters[0].transform.position.z);

        // Then face the hamster left
        _containedHamsters[0].FaceLeft();

        // Platform is flat so make sure the hamster rotation is flat
        transform.rotation = Quaternion.identity;
    }
    protected void FaceHamsterRight() {
        // Hamster is heading left
        // So set the hamster to our y position
        _containedHamsters[0].transform.position = new Vector3(transform.position.x,
                                                                transform.position.y,
                                                                _containedHamsters[0].transform.position.z);

        // Then face the hamster right
        _containedHamsters[0].FaceRight();

        // Platform is flat so make sure the hamster rotation is flat
        transform.rotation = Quaternion.identity;
    }
}
