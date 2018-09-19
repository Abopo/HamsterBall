using UnityEngine;
using System.Collections.Generic;

public class HamsterMeter : MonoBehaviour {
    public int shields; // Shields block orbs from adding to the meter

    public GameObject hamsterStockSprite;
    public GameObject hamsterTallyObj;
    public int team;

    int _curStock;
    int _baseMeterSize;
    int _meterSize;
    List<Transform> _stockTallies = new List<Transform>();
    List<GameObject> _stockSprites = new List<GameObject>();

    int _nextTallyIndex;

    public int CurStock {
        get { return _curStock; }
    }
    public List<Transform> StockTallies {
        get { return _stockTallies; }
    }

    public int MeterSize {
        get { return _meterSize; }
    }

    GameObject _shieldSpriteObj;
    GameObject[] _shieldSprites = new GameObject[6];

    BubbleManager _bubbleManager;
    AudioSource _audioSource;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }
    // Use this for initialization
    void Start() {
        // if we haven't been initialized yet
        if(_baseMeterSize == 0) {
            //Initialize with default values
            Initialize(13);
        }

        _curStock = 0;
        //GetChildren();

        _nextTallyIndex = 0;

        _shieldSpriteObj = Resources.Load<GameObject>("Prefabs/Effects/ShieldSprite");

        // FindObjectOfType correct bubble manager
        BubbleManager[] bManagers = FindObjectsOfType<BubbleManager>();
        foreach(BubbleManager bMan in bManagers) {
            if(bMan.team == team) {
                _bubbleManager = bMan;
            }
        }

    }

    public void Initialize(int lineLength) {
        int tallies = transform.childCount;

        // Remove tallies until lined up with handicap.
        while (lineLength != tallies) {
            if (lineLength < tallies) {
                DestroyImmediate(transform.GetChild(tallies - 1).gameObject);
                --tallies;
            } else if (lineLength > tallies) {
                // Adjust Hamster Meter accordingly
                // - Add a sprite
                GameObject hamsterTally = GameObject.Instantiate(hamsterTallyObj, transform) as GameObject;
                hamsterTally.transform.position = new Vector3(transform.GetChild(transform.childCount - 2).transform.position.x + 0.77f,
                                                               transform.position.y,
                                                               transform.position.z);
                tallies++;
            }
        }


        // - Adjust position of meter to be centered.
        //float x = 2.09f - ((tallies/2) * 0.5f);
        //transform.position = new Vector3(xPos + x, transform.position.y, transform.position.z);

        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
        }

        GetChildren();

        _baseMeterSize = tallies;
        _meterSize = _baseMeterSize;
        BecomeShort();
    }

    void GetChildren() {
        Transform tally;
        for (int i = 0; i < transform.childCount; ++i) {
            tally = transform.GetChild(i);
            if (tally.tag == "Tally") {
                _stockTallies.Add(tally);
            }
        }
    }

    // Update is called once per frame
    void Update() {
        // Update shield stuff
        if (shields > 0) {
            // If there are no stock orbs in play
            if (FindObjectOfType<StockOrb>() == null) {
                // Find out how many shields are missing
                int count = 0;
                while (_shieldSprites[count] == null) {
                    count++;
                }

                if (count > 0) {
                    // Move remaining shields down
                    for (int i = 0; i < _shieldSprites.Length; ++i) {
                        if (_shieldSprites[i] != null) {
                            _shieldSprites[i].transform.position = new Vector3(_shieldSprites[i].transform.position.x - (1f * count),
                                                                                _shieldSprites[i].transform.position.y,
                                                                                _shieldSprites[i].transform.position.z);
                            _shieldSprites[i - count] = _shieldSprites[i];
                            _shieldSprites[i] = null;
                        }
                    }
                }
            }
        }
    }

    // so far in all cases inc is going to be 1
    public void IncreaseStock(int inc) {
        // Block a point of junk with a shield if any are left
        while (inc != 0 && shields != 0) {
            inc -= 1;
            //shields -= 1;
            _nextTallyIndex -= 1;
            LoseShield();
        }

        _curStock += inc;
        // If we've filled the entire meter
        if (_curStock >= _meterSize) {
            while (_curStock >= _meterSize) {
                // If more than 1 stock was added at once and we went over limit, overflow to the next line
                _curStock = _curStock - _meterSize;

                // Clear out stockSprites
                foreach (GameObject sprite in _stockSprites) {
                    Destroy(sprite);
                }
                _stockSprites.Clear();

                // Add line to bubble manager
                if (!PhotonNetwork.connectedAndReady || (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient)) {
                    // Update meter size
                    if(_meterSize == _baseMeterSize) {
                        BecomeShort();
                    } else {
                        BecomeLong();
                    }

                    //bubbleManager.AddLine();
                    _bubbleManager.TryAddLine();
                }

                // Add new stock sprites if we need to
                if (_curStock > 0) {
                    for (int i = 0; i < _curStock; ++i) {
                        CreateNewStockSprite();
                    }
                }
            }

            // Set next tally index to front of meter
            _nextTallyIndex = 0;
        } else { // Otherwise just create new stock sprites based on inc
            for (int i = 0; i < inc; ++i) {
                CreateNewStockSprite();
            }
            PlaySound();
        }

        UpdateStockSprites();

        // Set bubbles managers stock
        _bubbleManager.bubbleStock = _curStock;
    }

    void CreateNewStockSprite() {
        // Create a new stock sprite.
        GameObject newStockSprite = GameObject.Instantiate(hamsterStockSprite, transform);

        // Set new stock sprite to correct color.
        int type = _bubbleManager.GetNextLineBubble(_bubbleManager.NextLineIndex + _curStock - 1);
        Animator[] animators = newStockSprite.GetComponentsInChildren<Animator>();
        foreach (Animator anim in animators) {
            anim.SetInteger("Type", type);
        }

        // Add new sprite to the list of stockSprites
        _stockSprites.Add(newStockSprite);
    }

    void UpdateStockSprites() {
        // Move all previous stocks up one spot
        int i = 0;
        foreach (GameObject stockSprite in _stockSprites) {
            if (i < _stockTallies.Count) {
                stockSprite.transform.position = new Vector3(_stockTallies[i].position.x,
                                                             _stockTallies[i].position.y,
                                                             _stockTallies[i].position.z - 1);
            }
            ++i;
        }
    }

    // These change the meter to either be 12 bubbles long or 13 bubbles long
    void BecomeShort() {
        // Turn off furthest tally
        _stockTallies[_baseMeterSize-1].gameObject.SetActive(false);

        // Move meter right one tally
        transform.Translate(0.38f, 0f, 0f);

        // Update meter size
        _meterSize -= 1;
    }
    void BecomeLong() {
        // Turn on furthest tally
        _stockTallies[_baseMeterSize-1].gameObject.SetActive(true);

        // Move meter left one tally
        transform.Translate(-0.38f, 0f, 0f);

        // Update meter size
        _meterSize += 1;
    }

    void PlaySound() {
        float curStock = _curStock;
        float meterSize = _meterSize;
        float volume = (curStock / meterSize);
        _audioSource.volume = volume;
        _audioSource.Play();
    }

    public Transform GetNextTalleyPosition() {
        Transform t;

        t = StockTallies[_nextTallyIndex];

        if(shields == 0) {
            _nextTallyIndex++;
            if (_nextTallyIndex >= StockTallies.Count - (_meterSize == _baseMeterSize ? 1 : 0)) {
                _nextTallyIndex = 0;
            }
        }

        return t;
    }

    public void GainShields(int amount) {
        shields = amount;

        Vector3 position;
        for(int i = 0; i < shields; ++i) {
            position = new Vector3(_stockTallies[i].transform.position.x, _stockTallies[i].transform.position.y, _stockTallies[i].transform.position.z - 5);
            _shieldSprites[i] = Instantiate(_shieldSpriteObj, position, Quaternion.identity, transform);
        }
    }

    public void LoseShield() {
        shields -= 1;

        // Find the lowest shield and destroy it
        int index = 0;
        while(_shieldSprites[index] == null) {
            index++;
        }
        DestroyObject(_shieldSprites[index]);
    }

    public void LoseAllShields() {
        shields = 0;

        for(int i = 0; i < _shieldSprites.Length; ++i) {
            if(_shieldSprites[i] != null) {
                DestroyObject(_shieldSprites[i]);
            }
        }
    }
}
