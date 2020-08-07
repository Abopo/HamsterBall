using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// These hamsters wander around the village
public class WanderingHamster : MonoBehaviour {
    public HamsterRoom targetRoom; // the room this hamster is trying to get to

    float _moveSpeed = 2.5f;
    bool _inRoom;
    int _type;

    Animator _animator;

    HamsterRoom[] _allRooms;

    void Awake() {
        _animator = GetComponentInChildren<Animator>();
        _allRooms = FindObjectsOfType<HamsterRoom>();
    }
    // Start is called before the first frame update
    void Start() {

        _inRoom = false;


        if (targetRoom == null) {
            ChooseRoom();
        }

        // Choose a type at random
        _type = Random.Range(0, (int)HAMSTER_TYPES.NUM_NORM_TYPES);
        _animator.SetInteger("Type", _type);
    }

    public void ChooseRoom() {
        // Find an empty room
        // TODO: this is kinda bad and could technically take a while
        int rand = 0;
        do {
            rand = Random.Range(0, (int)HAMSTERROOMS.NUM_ROOMS);

        } while (_allRooms[rand].IsFull && _allRooms[rand] != targetRoom);

        targetRoom = _allRooms[rand];

        // Increase the rooms hamCount so other hamsters don't target the same room
        targetRoom.hamCount++;
    }

    // Update is called once per frame
    void Update() {

        if (!_inRoom) {
            // Run forward
            transform.Translate(transform.right * (_moveSpeed * Mathf.Sign(transform.localScale.x) * Time.deltaTime), Space.World);
        } else {
            // Maybe leave the room?

        }
    }

    protected void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "HamsterRoom") {
            if(collision.GetComponent<HamsterRoom>().room == targetRoom.room) {
                collision.GetComponent<HamsterRoom>().TakeHamster(this);
                //EnterRoom();
            }
        }
    }

    public void EnterRoom() {
        // FOR TESTING

        if (targetRoom.room == HAMSTERROOMS.CHARACTER || 
            targetRoom.room == HAMSTERROOMS.LEFT || 
            targetRoom.room == HAMSTERROOMS.NETWORK ||
            targetRoom.room == HAMSTERROOMS.VERSUS) {
            FaceRight();
        } else if (targetRoom.room == HAMSTERROOMS.MUSHROOM ||
            targetRoom.room == HAMSTERROOMS.STORY ||
            targetRoom.room == HAMSTERROOMS.OPTIONS) {
            FaceLeft();
        }

        ChooseRoom();
    }

    public void ExitRoom() {
        // Choose a new room
        ChooseRoom();

        // Set the type again cuz it gets reset somehow?
        _animator.SetInteger("Type", _type);
    }

    // Directional Functions
    public void FaceUp() {
        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Sign(transform.localScale.x) * 90f);
    }
    public void FaceDown() {
        transform.eulerAngles = new Vector3(0f, 0f, Mathf.Sign(transform.localScale.x) * -90f);
    }
    public void FaceLeft() {
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        if (transform.localScale.x > 0) {
            Flip();
        }
    }
    public void FaceRight() {
        transform.eulerAngles = new Vector3(0f, 0f, 0f);
        if (transform.localScale.x < 0) {
            Flip();
        }
    }

    public void Flip() {
        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
