using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSprite : MonoBehaviour {

    public bool isHeld;

    HAMSTER_TYPES type;
    int node = -1;

    BoardEditor _boardEditor;

    public HAMSTER_TYPES Type {
        get { return type; }
    }

    // Use this for initialization
    void Start () {
        _boardEditor = FindObjectOfType<BoardEditor>();
        isHeld = true;

        transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
	}
	
	// Update is called once per frame
	void Update () {
        // If this bubble is being held
        if (isHeld) {
            // Follow the mouse
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        // Stay forward
        transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
    }

    public void SetType(HAMSTER_TYPES inType) {
        type = inType;
        GetComponent<Animator>().SetInteger("Type", (int)inType);
        transform.GetChild(0).GetComponent<Animator>().SetInteger("Type", (int)inType);
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
            // Find closest node and drop there
            int closestNode = _boardEditor.FindClosestNode(transform.position);
            
            // If a valid node was found
            if(closestNode != -1) {
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
        } else {
            // Destroy this
            DestroyObject(this.gameObject);
        }
    }
}
