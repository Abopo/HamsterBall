using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSprite : MonoBehaviour {

    public bool isHeld = false;
    public int node = -1;

    public bool isGravity;
    public Animator bubbleAnimator;
    public Animator hamsterAnimator;
    public bool isIce;
    public SpriteRenderer iceSprite;

    HAMSTER_TYPES type;

    PlasmaEffect _plasmaEffect;

    BoardEditor _boardEditor;

    public HAMSTER_TYPES Type {
        get { return type; }
    }

    private void Awake() {
        _boardEditor = FindObjectOfType<BoardEditor>();
        _plasmaEffect = GetComponentInChildren<PlasmaEffect>();
    }
    // Use this for initialization
    void Start () {
        if (isGravity) {
            // Turn on plasma effect
            _plasmaEffect.ForceActivate((int)type);
        }
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
                    Destroy(gameObject);
                }
            }
        }

        // Stay forward
        //transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    public void SetType(int inType) {
        // If this should be gravity
        if (inType >= (int)HAMSTER_TYPES.PLASMA) {
            inType -= (int)HAMSTER_TYPES.PLASMA;
            SetIsGravity(true);
        }
        type = (HAMSTER_TYPES)inType;
        bubbleAnimator.SetInteger("Type", (int)inType);
        hamsterAnimator.SetInteger("Type", (int)inType);
    }

    public void SetIsGravity(bool isGrav) {
        if(isGrav) {
            isGravity = true;
            // Turn on plasma effect
            _plasmaEffect.ForceActivate((int)type);
        } else {
            isGravity = false;
            _plasmaEffect.Deactivate();
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
            Destroy(gameObject);
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
