using UnityEngine;
using System.Collections;

public class BubblePopAnimation : MonoBehaviour {

    public GameObject bubblePieceObj;

    HAMSTER_TYPES _type;
    Rigidbody2D _hamsterSprite;
    //Sprite[] _bubblePiecesSprites = new Sprite[4];
    GameObject[] _bubblePieces = new GameObject[4];
    Sprite[] _bubblePiecesSprites = new Sprite[4];

    bool _popped;
    float _destroyTime = 2.0f;
    float _destroyTimer = 0.0f;

    static Sprite[] _allBubblePiecesSprites;

    void Awake() {
        if (_allBubblePiecesSprites == null) {
            _allBubblePiecesSprites = Resources.LoadAll<Sprite>("Art/Hamsters_and_Bubbles/BubblePieces");
        }
    }

    // Use this for initialization
    void Start () {

        _hamsterSprite = transform.GetChild(0).GetComponent<Rigidbody2D>();
        _popped = false;
    }

    // Update is called once per frame
    void Update() {
        if(_popped) {
            _destroyTimer += Time.deltaTime;
            if(_destroyTimer >= _destroyTime) {
                DestroyObject(this.gameObject);
            }
        }
    }

    public void LoadPieces(HAMSTER_TYPES type) {
        _type = type;
        int index = 0;
        switch(type) {
            case HAMSTER_TYPES.BLUE:
                index = 0;
                break;
            case HAMSTER_TYPES.BOMB:
                index = 4;
                break;
            case HAMSTER_TYPES.DEAD:
                index = 8;
                break;
            case HAMSTER_TYPES.GRAY:
                index = 12;
                break;
            case HAMSTER_TYPES.GREEN:
                index = 16;
                break;
            case HAMSTER_TYPES.ORANGE:
                index = 20;
                break;
            case HAMSTER_TYPES.PINK:
                index = 24;
                break;
            case HAMSTER_TYPES.PURPLE:
                index = 28;
                break;
            case HAMSTER_TYPES.RED:
                index = 32;
                break;
            case HAMSTER_TYPES.RAINBOW:
                index = -1;
                break;
        }

        if (index >= 0) {
            for (int i = 0; i < 4; ++i) {
               _bubblePiecesSprites[i]  = _allBubblePiecesSprites[index + i];
            }
        } else if (index == -1) {
            // pick some colors at random

        }
    }

    public void Pop() {
        // Create 4 bubble pieces, and set the appropriate sprites.
        for(int i = 0; i < 4; ++i) {
            _bubblePieces[i] = GameObject.Instantiate(bubblePieceObj, this.transform) as GameObject;
            _bubblePieces[i].GetComponent<SpriteRenderer>().sprite = _bubblePiecesSprites[i];
            _bubblePieces[i].transform.position = transform.position;
        }

        // Launch off bubble pieces with slight random velocities and rotations.
        float rX, rY;

        rX = Random.Range(-1f, 1f);
        rY = Random.Range(-1f, 2f);
        _bubblePieces[0].GetComponent<Rigidbody2D>().velocity = new Vector2(-2f + rX, 4f + rY);
        _bubblePieces[0].GetComponent<Rigidbody2D>().rotation = Random.Range(-20f, 20f);

        rX = Random.Range(-1f, 1f);
        rY = Random.Range(-1f, 1f);
        _bubblePieces[1].GetComponent<Rigidbody2D>().velocity = new Vector2(2f + rX, 4f + rY);
        _bubblePieces[1].GetComponent<Rigidbody2D>().rotation = Random.Range(-20f, 20f);

        rX = Random.Range(-1f, 1f);
        rY = Random.Range(-1f, 1f);
        _bubblePieces[2].GetComponent<Rigidbody2D>().velocity = new Vector2(-2f + rX, 2f + rY);
        _bubblePieces[2].GetComponent<Rigidbody2D>().rotation = Random.Range(-20f, 20f);

        rX = Random.Range(-1f, 1f);
        rY = Random.Range(-1f, 1f);
        _bubblePieces[3].GetComponent<Rigidbody2D>().velocity = new Vector2(2f + rX, 2f + rY);
        _bubblePieces[3].GetComponent<Rigidbody2D>().rotation = Random.Range(-20f, 20f);

        // Launch off hamster sprite
        rX = Random.Range(-1f, 1f);
        rY = Random.Range(-0.5f, 2f);
        _hamsterSprite.velocity = new Vector2(1f + rX, 4f + rY);
        _hamsterSprite.gravityScale = 2;
        _hamsterSprite.isKinematic = false;

        // Turn off normal bubble sprite and collision
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;

        // Start destroy timer
        _popped = true;
    }

}
