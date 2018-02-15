using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapIcon : MonoBehaviour {

    public float moveSpeed;
    public int index; // 0-2; 0 = left spot, 1 = middle spot(selected) 2 = right spot
    public string mapName;
    public Text curMapText;
    public MapIcon icon1;
    public MapIcon icon2;
    public bool hasTransitioned;

    float _moveVelocity;
    bool _moving = false;
    float _stopAt;

    int _mapIndex;
    Sprite[] _mapImages = new Sprite[9];
    string[] _mapNames = new string[9];

    SpriteRenderer _spriteRenderer;

    public bool Moving {
        get { return _moving; }
    }

    // Use this for initialization
    void Start () {
        switch(index) {
            case 0:
                mapName = "OneTube - Forest";
                _mapIndex = 0;
                break;
            case 1:
                mapName = "TwoTubes - Laboratory";
                _mapIndex = 1;
                break;
            case 2:
                mapName = "TwoTubes - Sewers";
                _mapIndex = 2;
                break;
        }

        _mapImages[0] = Resources.Load<Sprite>("Art/UI/OneTube - Forest");
        _mapImages[1] = Resources.Load<Sprite>("Art/UI/TwoTubes - Laboratory");
        _mapImages[2] = Resources.Load<Sprite>("Art/UI/TwoTubes - Sewers");
        _mapImages[3] = Resources.Load<Sprite>("Art/UI/TwoTubes - City");
        _mapImages[4] = Resources.Load<Sprite>("Art/UI/TwoTubes - City2");
        _mapImages[5] = Resources.Load<Sprite>("Art/UI/TwoTubes - Mountain");
        _mapImages[6] = Resources.Load<Sprite>("Art/UI/OneTube - Fungals");
        _mapImages[7] = Resources.Load<Sprite>("Art/UI/TwoTubes - Space");
        _mapImages[8] = Resources.Load<Sprite>("Art/UI/OneTube - DarkForest");

        _mapNames[0] = "OneTube - Forest";
        _mapNames[1] = "TwoTubes - Laboratory";
        _mapNames[2] = "TwoTubes - Sewers";
        _mapNames[3] = "TwoTubes - City";
        _mapNames[4] = "TwoTubes - City2";
        _mapNames[5] = "TwoTubes - Mountain";
        _mapNames[6] = "OneTube - Fungals";
        _mapNames[7] = "TwoTubes - Space";
        _mapNames[8] = "OneTube - DarkForest";

        _spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(_moving) {
            transform.Translate(_moveVelocity*Time.deltaTime, 0f, 0f);

            // if moving left
            if (_moveVelocity < 0) {
                if (index == 0 && transform.position.x <= -18.25f) {
                    Transition();
                }

                if (hasTransitioned && transform.position.x <= _stopAt) {
                    transform.position = new Vector3(_stopAt, transform.position.y, transform.position.z);
                    _moving = false;
                }
              // if moving right
            } else if(_moveVelocity > 0) {
                if (index == 2 && transform.position.x >= 18.25f) {
                    Transition();
                }

                if (hasTransitioned && transform.position.x >= _stopAt) {
                    transform.position = new Vector3(_stopAt, transform.position.y, transform.position.z);
                    _moving = false;
                }
            }
        } else {
            if(index == 1) {
                curMapText.text = mapName;
            }
        }

	}

    void Transition() {
        if (_moveVelocity < 0) {
            transform.position = new Vector3(19.14f, transform.position.y, transform.position.z);
            index = 2;
            icon1.index = 0;
            icon2.index = 1;
            _mapIndex = icon2._mapIndex + 1;
            if (_mapIndex >= _mapImages.Length) {
                _mapIndex = 0;
            }
            /*
            for(int i = 0; i < 3; ++i) {
                _mapIndex -= 1;
                if(_mapIndex < 0) {
                    _mapIndex = _mapImages.Length - 1;
                }
            }
            */
        } else if(_moveVelocity > 0) {
            transform.position = new Vector3(-19.14f, transform.position.y, transform.position.z);
            index = 0;
            icon1.index = 1;
            icon2.index = 2;
            _mapIndex = icon1._mapIndex - 1;
            if (_mapIndex < 0) {
                _mapIndex = _mapImages.Length - 1;
            }
            /*
            for (int i = 0; i < 3; ++i) {
                _mapIndex += 1;
                if (_mapIndex > _mapImages.Length - 1) {
                    _mapIndex = 0;
                }
            }
            */
        }
        _spriteRenderer.sprite = _mapImages[_mapIndex];
        mapName = _mapNames[_mapIndex];

        hasTransitioned = true;
        icon1.hasTransitioned = true;
        icon2.hasTransitioned = true;
    }

    public void Move(bool left) {
        hasTransitioned = false;
        curMapText.text = "";

        _moving = true;
        if(left) {
            _moveVelocity = -moveSpeed;
            switch (index) {
                case 0:
                    _stopAt = 12.48f;
                    break;
                case 1:
                    _stopAt = -12.41f;
                    break;
                case 2:
                    _stopAt = 0;
                    break;
            }
        } else {
            _moveVelocity = moveSpeed;
            switch (index) {
                case 0:
                    _stopAt = 0f;
                    break;
                case 1:
                    _stopAt = 12.48f;
                    break;
                case 2:
                    _stopAt = -12.41f;
                    break;
            }
        }

    }
}
