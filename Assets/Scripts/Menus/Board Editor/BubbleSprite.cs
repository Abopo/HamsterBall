using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSprite : MonoBehaviour {

    public bool isHeld = false;
    public int node = -1;

    public bool isGravity;
    public GameObject spiralEffectObj;
    public GameObject spiralEffectInstance;
    public bool isIce;
    public SpriteRenderer iceSprite;

    HAMSTER_TYPES type;

    BoardEditor _boardEditor;

    public HAMSTER_TYPES Type {
        get { return type; }
    }

    // Use this for initialization
    void Start () {
        _boardEditor = FindObjectOfType<BoardEditor>();

        transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
	}
	
	// Update is called once per frame
	void Update () {
        // If this bubble is being held
        if (isHeld) {
            // Follow the mouse
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) {
                if (_boardEditor.IsWithinBounds(transform.position)) {
                    DropOntoBoard();
                } else {
                    // Destroy this
                    DestroyObject(this.gameObject);
                }
            }
        }

        // Stay forward
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    public void SetType(int inType) {
        // If this should be gravity
        if (inType >= 11) {
            inType -= 11;
            SetIsGravity(true);
        }
        type = (HAMSTER_TYPES)inType;
        GetComponent<Animator>().SetInteger("Type", (int)inType);
        transform.GetChild(0).GetComponent<Animator>().SetInteger("Type", (int)inType);
    }

    public void SetIsGravity(bool isGrav) {
        if(isGrav) {
            isGravity = true;
            spiralEffectInstance = Instantiate(spiralEffectObj, transform.position, Quaternion.Euler(-90, 0, 0)) as GameObject;
            spiralEffectInstance.transform.parent = transform;
            spiralEffectInstance.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
        } else {
            isGravity = false;
        }
    }

    public void SetIsIce(bool iced) {
        isIce = iced;
        iceSprite.enabled = iced;
    }

    private void OnMouseDown() {
        // Become held
        isHeld = true;

        // Detach from node
        EditorNode n = _boardEditor.GetNode(node);
        if(n != null) {
            n.bubble = null;
        }
    }

    private void OnMouseUp() {
        // If dropped in valid space
        if (_boardEditor.IsWithinBounds(transform.position)) {
            DropOntoBoard();
        } else {
            // Destroy this
            DestroyObject(this.gameObject);
        }
    }

    void DropOntoBoard() {
        // Find closest node and drop there
        int closestNode = _boardEditor.FindClosestFreeNode(transform.position);

        // If a valid node was found
        if (closestNode != -1) {
            // Set node
            node = closestNode;
            EditorNode n = _boardEditor.GetNode(closestNode);
            if (n != null) {
                n.bubble = this;
            }

            // Drop on that position
            transform.position = (Vector2)_boardEditor.GetNode(closestNode).nPosition;

            // Stop being held
            isHeld = false;
        }
    }
}
