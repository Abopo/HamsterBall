using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {
    public int number;
    public Bubble bubble;
    public bool isRelevant;
    public LayerMask checkMask;
    public bool canBeHit;

    private Bubble[] adjBubbles;

    public Vector3 nPosition {
        get { return transform.position; }
    }

    public Bubble[] AdjBubbles {
        get {
            if (bubble != null) {
                return bubble.adjBubbles;
            } else {
                return adjBubbles;
            }
        }

        set {
            adjBubbles = value;
        }
    }

    void Awake() {
        adjBubbles = new Bubble[6];
    }

    // Use this for initialization
    void Start () {
        GetComponentInParent<BubbleManager>().boardChangedEvent.AddListener(CheckCanBeHit);
        CheckCanBeHit();
	}
	
	// Update is called once per frame
	void Update () {
        if(isRelevant) {
            Vector3 end = new Vector3(transform.position.x,
                                        transform.position.y - 0.1f,
                                        transform.position.z);
            Debug.DrawLine(transform.position, end, Color.green);
        }
    }

    public void CheckCanBeHit() {
        if (IsRelevant() && CanBeHit()) {
            isRelevant = true;
        } else {
            isRelevant = false;
        }
    }

    public bool IsRelevant() {
        int count = 0;
        
        // If the node has any bubbles above it, it has a chance to be hit.
        for (int i = 0; i < 2; ++i) {
            if(adjBubbles[i] != null) {
                count++;
            }
        }

        // This checks for bubbles below the node, if both these are here
        // then the node can't be hit anyway, and shouldn't be considered.
        for (int i = 3; i < 5; ++i) {
            if (adjBubbles[i] != null) {
                count--;
            }
        }

        // If the adjBubbles close off the node, then it can't be hit
        if((adjBubbles[2] != null && adjBubbles[4] != null) || (adjBubbles[5] != null && adjBubbles[3] != null)) {
            count = 0;
        }

        if(count > 0) {
            return true;
        } else {
            return false;
        }
    }

    // This function scans around the node to check whether or not it's possible to be hit by the player.
    bool CanBeHit() {
        canBeHit = false;

        RaycastHit2D hit;
        Vector2 rayDir;

        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

        int hitCount = 0;
        Vector2 origin;
        for (int i = 0; i < 4; ++i) {
            hitCount = 0;

            for (float j = -0.2f; j < 0.4f; j += 0.2f) {
                rayDir = new Vector2(-1 + (i * 0.45f) + j / 2, -1);
                origin = new Vector2(transform.position.x + j, transform.position.y);
                hit = Physics2D.Raycast(origin, rayDir, 10f, checkMask);
                if (hit && hit.transform.tag == "Platform") {
                    hitCount++;
                    //Debug.DrawRay(origin, rayDir * hit.distance);
                }
            }

            if (hitCount > 2) {
                canBeHit = true;
                break;
            }
        }

        return canBeHit;
    }

    public void ClearAdjBubbles() {
        for (int i = 0; i < 6; ++i) {
            adjBubbles[i] = null;
        }
    }
}
