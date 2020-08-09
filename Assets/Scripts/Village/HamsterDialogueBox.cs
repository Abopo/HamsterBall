using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterDialogueBox : MonoBehaviour {

    public SuperTextMesh dialogueText;
    public Transform bottomPieces;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        // If the dialogue has gone to the 3rd line
        if (dialogueText.rawBottomRightTextBounds.y < -50f) {
            // Extend the box down
            bottomPieces.localPosition = new Vector3(bottomPieces.localPosition.x, -87.5f, bottomPieces.localPosition.z);
        } else {
            // Extend the box down
            bottomPieces.localPosition = new Vector3(bottomPieces.localPosition.x, -60f, bottomPieces.localPosition.z);
        }
    }
}
