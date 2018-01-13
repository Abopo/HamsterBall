using UnityEngine;
using System.Collections.Generic;

public class HamsterMeter : MonoBehaviour {
    public GameObject hamsterStockSprite;
    public BubbleManager bubbleManager;
    public GameObject hamsterTallyObj;

    int curStock;
    int meterSize;
    List<Transform> stockTallies = new List<Transform>();
    List<GameObject> stockSprites = new List<GameObject>();

    public int CurStock {
        get { return curStock; }
    }
    public List<Transform> StockTallies {
        get { return stockTallies; }
    }

    // Use this for initialization
    void Start () {
        curStock = 0;
        meterSize = transform.childCount;
        GetChildren();
	}
	
    void GetChildren() {
        for(int i = 0; i < transform.childCount; ++i) {
            stockTallies.Add(transform.GetChild(i));
        }
    }

	// Update is called once per frame
	void Update () {
	
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

    public void IncreaseStock(int inc) {
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
                    bubbleManager.AddLine();
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
        GameObject newStockSprite = GameObject.Instantiate(hamsterStockSprite);
        // Set new stock sprite to correct color.

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
}
