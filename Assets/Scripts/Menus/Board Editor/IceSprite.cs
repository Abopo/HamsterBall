using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSprite : MonoBehaviour {

    BoardEditor _boardEditor;

    // Use this for initialization
    void Start () {
        _boardEditor = FindObjectOfType<BoardEditor>();

        transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
    }

    // Update is called once per frame
    void Update () {
        // Follow the mouse
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonUp(0)) {
            if (_boardEditor.IsWithinBounds(transform.position)) {
                DropOntoBoard();
            } else {
                // Destroy this
                DestroyObject(this.gameObject);
            }
        }

        // Stay forward
        transform.position = new Vector3(transform.position.x, transform.position.y, -5f);
    }


    void DropOntoBoard() {
        // Find closest node and drop there
        int closestNode = _boardEditor.FindClosestNode(transform.position);

        // If a valid node was found
        if (closestNode != -1 && _boardEditor.GetNode(closestNode).bubble != null) {
            // Get the bubble from the node
            BubbleSprite bSprite = _boardEditor.GetNode(closestNode).bubble;

            // Ice that sprite
            bSprite.SetIsIce(!bSprite.isIce);

            // Destroy this
            DestroyObject(this.gameObject);
        }
    }
}
