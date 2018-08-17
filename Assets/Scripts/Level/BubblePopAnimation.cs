﻿using UnityEngine;
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

    void Awake() {
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
                _bubblePiecesSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/BlueBubblePieces");
                break;
            case HAMSTER_TYPES.GRAY:
                _bubblePiecesSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/GrayBubblePieces");
                break;
            case HAMSTER_TYPES.GREEN:
                _bubblePiecesSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/GreenBubblePieces");
                break;
            case HAMSTER_TYPES.ORANGE:
                _bubblePiecesSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/YellowBubblePieces");
                break;
            case HAMSTER_TYPES.PINK:
                _bubblePiecesSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/PinkBubblePieces");
                break;
            case HAMSTER_TYPES.PURPLE:
                _bubblePiecesSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/PurpleBubblePieces");
                break;
            case HAMSTER_TYPES.RED:
                _bubblePiecesSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/RedBubblePieces");
                break;
            case HAMSTER_TYPES.RAINBOW:
                index = -1;
                break;
            case HAMSTER_TYPES.BOMB:
                _bubblePiecesSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/RedBubblePieces");
                break;
            case HAMSTER_TYPES.DEAD:
                _bubblePiecesSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/GrayBubblePieces");
                break;
        }

        if (index == -1) {
            // TODO: pick some colors at random
            int color = 0;
            Sprite[] tempSprites;

            for(int i = 0; i < 4; ++i) {
                color = Random.Range(0, 7);
                switch ((HAMSTER_TYPES)color) {
                    case HAMSTER_TYPES.BLUE:
                        tempSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/BlueBubblePieces");
                        _bubblePiecesSprites[i] = tempSprites[i];
                        break;
                    case HAMSTER_TYPES.GRAY:
                        tempSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/GrayBubblePieces");
                        _bubblePiecesSprites[i] = tempSprites[i];
                        break;
                    case HAMSTER_TYPES.GREEN:
                        tempSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/GreenBubblePieces");
                        _bubblePiecesSprites[i] = tempSprites[i];
                        break;
                    case HAMSTER_TYPES.ORANGE:
                        tempSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/YellowBubblePieces");
                        _bubblePiecesSprites[i] = tempSprites[i];
                        break;
                    case HAMSTER_TYPES.PINK:
                        tempSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/PinkBubblePieces");
                        _bubblePiecesSprites[i] = tempSprites[i];
                        break;
                    case HAMSTER_TYPES.PURPLE:
                        tempSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/PurpleBubblePieces");
                        _bubblePiecesSprites[i] = tempSprites[i];
                        break;
                    case HAMSTER_TYPES.RED:
                        tempSprites = Resources.LoadAll<Sprite>("Art/Characters/Hamsters/Bubbles/RedBubblePieces");
                        _bubblePiecesSprites[i] = tempSprites[i];
                        break;
                }

            }
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
        _hamsterSprite.GetComponent<Animator>().SetInteger("State", 1);

        // Turn off normal bubble sprite and collision
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;

        // Start destroy timer
        _popped = true;
    }

}
