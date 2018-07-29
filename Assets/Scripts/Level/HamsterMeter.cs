using UnityEngine;
using System.Collections.Generic;

public class HamsterMeter : MonoBehaviour {
    public int shields; // Shields block orbs from adding to the meter

    public GameObject hamsterStockSprite;
    public BubbleManager bubbleManager;
    public GameObject hamsterTallyObj;

    int curStock;
    int meterSize;
    List<Transform> stockTallies = new List<Transform>();
    List<GameObject> stockSprites = new List<GameObject>();

    int _nextTallyIndex;

    public int CurStock {
        get { return curStock; }
    }
    public List<Transform> StockTallies {
        get { return stockTallies; }
    }

    GameObject _shieldSpriteObj;
    GameObject[] _shieldSprites = new GameObject[6];

    // Use this for initialization
    void Start () {
        curStock = 0;
        meterSize = transform.childCount;
        GetChildren();

        _nextTallyIndex = 0;

        _shieldSpriteObj = Resources.Load<GameObject>("Prefabs/Effects/ShieldSprite");
	}
	
    void GetChildren() {
        for(int i = 0; i < transform.childCount; ++i) {
            stockTallies.Add(transform.GetChild(i));
        }
    }

	// Update is called once per frame
	void Update () {
	    if(shields > 0) {
            // If there are no stock orbs in play
            if(FindObjectOfType<StockOrb>() == null) {
                // Find out how many shields are missing
                int count = 0;
                while (_shieldSprites[count] == null) {
                    count++;
                }

                if (count > 0) {
                    // Move remaining shields down
                    for (int i = 0; i < _shieldSprites.Length; ++i) {
                        if (_shieldSprites[i] != null) {
                            _shieldSprites[i].transform.position = new Vector3(_shieldSprites[i].transform.position.x,
                                                                                _shieldSprites[i].transform.position.y - (1f * count),
                                                                                _shieldSprites[i].transform.position.z);
                            _shieldSprites[i - count] = _shieldSprites[i];
                            _shieldSprites[i] = null;
                        }
                    }
                }
            }
        }
	}

    public void Initialize(int handicap) {
        int tallies = transform.childCount;

        // Remove tallies until lined up with handicap.
        while (handicap != tallies) {
            if (handicap < tallies) {
                DestroyImmediate(transform.GetChild(tallies - 1).gameObject);
                --tallies;
            } else if (handicap > tallies) {
                // Adjust Hamster Meter accordingly
                // - Add a sprite
                GameObject hamsterTally = GameObject.Instantiate(hamsterTallyObj, transform) as GameObject;
                hamsterTally.transform.position = new Vector3(transform.position.x,
                                                               transform.GetChild(transform.childCount - 2).transform.position.y + 0.95f,
                                                               transform.position.z);
                tallies++;
            }
        }

        // - Adjust position of meter to be centered.
        float y = 2.09f - ((tallies - 6) * 0.5f);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);

        for (int i = 0; i < transform.childCount; ++i) {
            transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
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

        curStock += inc;
        // If we've filled the entire meter
        if(curStock >= meterSize) {
            while (curStock >= meterSize) {
                // If more than 1 stock was added at once and we went over limit, overflow to the next line
                curStock = curStock - meterSize;

                // Clear out stockSprites
                foreach (GameObject sprite in stockSprites) {
                    Destroy(sprite);
                }
                stockSprites.Clear();

                // Add line to bubble manager
                if (!PhotonNetwork.connectedAndReady || (PhotonNetwork.connectedAndReady && PhotonNetwork.isMasterClient)) {
                    //bubbleManager.AddLine();
                    bubbleManager.TryAddLine();
                }

                // Add new stock sprites if we need to
                if (curStock > 0) {
                    for (int i = 0; i < curStock; ++i) {
                        CreateNewStockSprite();
                    }
                }
            }
        } else { // Otherwise just create new stock sprites based on inc
            for(int i = 0; i < inc; ++i) {
                CreateNewStockSprite();
            }
            GetComponent<AudioSource>().Play();
        }

        UpdateStockSprites();

        // Set bubbles managers stock
        bubbleManager.bubbleStock = curStock;
    }

    void CreateNewStockSprite() {
        // Create a new stock sprite.
        GameObject newStockSprite = GameObject.Instantiate(hamsterStockSprite, transform);

        // Set new stock sprite to correct color.
        int type = bubbleManager.GetNextLineBubble(bubbleManager.NextLineIndex+curStock-1);
        Animator[] animators = newStockSprite.GetComponentsInChildren<Animator>();
        foreach(Animator anim in animators) {
            anim.SetInteger("Type", type);
        }

        // Add new sprite to the list of stockSprites
        stockSprites.Add(newStockSprite);
    }

    void UpdateStockSprites() {
        // Move all previous stocks up one spot
        int i = 0;
        foreach (GameObject stockSprite in stockSprites) {
            if (i < stockTallies.Count) {
                stockSprite.transform.position = new Vector3(stockTallies[i].position.x,
                                                             stockTallies[i].position.y,
                                                             stockTallies[i].position.z - 1);
            }
            ++i;
        }
    }

    public Transform GetNextTalleyPosition() {
        Transform t;

        t = StockTallies[_nextTallyIndex];

        if(shields == 0) {
            _nextTallyIndex++;
            if (_nextTallyIndex >= StockTallies.Count) {
                _nextTallyIndex = 0;
            }
        }

        return t;
    }

    public void GainShields(int amount) {
        shields = amount;

        Vector3 position;
        for(int i = 0; i < shields; ++i) {
            position = new Vector3(stockTallies[i].transform.position.x, stockTallies[i].transform.position.y, stockTallies[i].transform.position.z - 5);
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
