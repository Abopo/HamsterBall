using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HamsterDialogueBox : MonoBehaviour {

    public SuperTextMesh dialogueText;
    public Transform bottomPieces;
    public Transform innerPieces;

    public HAMSTER_TYPES color;

    // Start is called before the first frame update
    void Start() {
        // Set box colors
        Color outColor = Color.white;

        switch (color) {
            case HAMSTER_TYPES.BLUE:
                ColorUtility.TryParseHtmlString("#6790F1", out outColor);
                    break;
            case HAMSTER_TYPES.GRAY:
                ColorUtility.TryParseHtmlString("#DADADA", out outColor);
                break;
            case HAMSTER_TYPES.GREEN:
                ColorUtility.TryParseHtmlString("#24C34C", out outColor);
                break;
            case HAMSTER_TYPES.PINK:
                ColorUtility.TryParseHtmlString("#E57CC4", out outColor);
                break;
            case HAMSTER_TYPES.PURPLE:
                ColorUtility.TryParseHtmlString("#D254FF", out outColor);
                break;
            case HAMSTER_TYPES.RED:
                ColorUtility.TryParseHtmlString("#FF6B6B", out outColor);
                break;
            case HAMSTER_TYPES.YELLOW:
                ColorUtility.TryParseHtmlString("#F5F167", out outColor);
                break;
            case HAMSTER_TYPES.SKULL:
                ColorUtility.TryParseHtmlString("#FFFFFF", out outColor);
                break;
            case HAMSTER_TYPES.RAINBOW:
                ColorUtility.TryParseHtmlString("#FFFFFF", out outColor);
                break;
            case HAMSTER_TYPES.BOMB:
                ColorUtility.TryParseHtmlString("#FFFFFF", out outColor);
                break;
        }

        Image[] images = transform.GetComponentsInChildren<Image>(true);
        foreach (Image im in images) {
            im.color = outColor;
        }
    }

    // Update is called once per frame
    void Update() {

        // If the dialogue has gone to the 4th line
        if (dialogueText.rawBottomRightTextBounds.y < -75f) {
            // Extend the box down
            bottomPieces.localPosition = new Vector3(bottomPieces.localPosition.x, -115f, bottomPieces.localPosition.z);
            innerPieces.gameObject.SetActive(true);
        // If the dialogue has gone to the 3rd line
        } else if (dialogueText.rawBottomRightTextBounds.y < -50f) {
            // Extend the box down
            bottomPieces.localPosition = new Vector3(bottomPieces.localPosition.x, -87.5f, bottomPieces.localPosition.z);
            innerPieces.gameObject.SetActive(true);
        } else {
            // default box position
            bottomPieces.localPosition = new Vector3(bottomPieces.localPosition.x, -60f, bottomPieces.localPosition.z);
            innerPieces.gameObject.SetActive(false);
        }
    }
}
