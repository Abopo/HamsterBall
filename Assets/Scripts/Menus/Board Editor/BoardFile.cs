using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardFile : MonoBehaviour {
    public bool isHighlighted;
    public Image highlightImage;

	// Use this for initialization
	void Start () {
        isHighlighted = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseUp() {
        // Highlight this file
        Highlight();
    }

    public void Highlight() {
        // De-highlight all the other files
        BoardFile[] boardFiles = FindObjectsOfType<BoardFile>();
        foreach (BoardFile bF in boardFiles) {
            bF.Unhighlight();
        }

        highlightImage.enabled = true;
        isHighlighted = true;
    }

    public void Unhighlight() {
        isHighlighted = false;
        highlightImage.enabled = false;
    }
}
