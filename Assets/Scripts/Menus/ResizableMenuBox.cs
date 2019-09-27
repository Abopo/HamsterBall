using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public enum BOXMATERIAL { SIMPLEWOOD = 0, DARKWOOD, LIGHTWOOD, NUM_MATS }
[ExecuteInEditMode]
public class ResizableMenuBox : MonoBehaviour {
    public Image topLeftCorner;
    public Image topRightCorner;
    public Image bottomLeftCorner;
    public Image bottomRightCorner;

    public Image topBorder;
    public Image rightBorder;
    public Image bottomBorder;
    public Image leftBorder;

    public Image backer;

    public float cornerOffset;
    public float borderOffsetX;
    public float borderOffsetY;
    public float backerSize;
    public float borderSizeX;
    public float borderSizeY;

    public int width = 150;
    public int height = 150;

    public BOXMATERIAL material;

    BOXMATERIAL curMaterial;
    int _baseWidth = 150;
    int _baseHeight = 150;

    private void Awake() {
    }
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        // Position the images to match the size of the box

        int widthDif = width - _baseWidth;
        int heightDif = height - _baseHeight;

        // Top corners: y locked
        topLeftCorner.rectTransform.transform.localPosition = new Vector3(-cornerOffset - widthDif/2, topLeftCorner.rectTransform.transform.localPosition.y);
        topRightCorner.rectTransform.transform.localPosition = new Vector3(cornerOffset + widthDif/2, topRightCorner.rectTransform.transform.localPosition.y);

        // Top border: x & y locked
        // So just change width
        topBorder.rectTransform.sizeDelta = new Vector2(width, topBorder.rectTransform.sizeDelta.y);

        // Left/Right Borders: x locked
        leftBorder.rectTransform.sizeDelta = new Vector2(leftBorder.rectTransform.sizeDelta.x, borderSizeY + height);
        rightBorder.rectTransform.sizeDelta = new Vector2(rightBorder.rectTransform.sizeDelta.x, borderSizeY + height);
        leftBorder.rectTransform.transform.localPosition = new Vector3(-borderOffsetX - widthDif / 2, leftBorder.rectTransform.transform.localPosition.y);
        rightBorder.rectTransform.transform.localPosition = new Vector3(borderOffsetX + widthDif / 2, rightBorder.rectTransform.transform.localPosition.y);

        // Bottom corners
        bottomLeftCorner.rectTransform.transform.localPosition = new Vector3(-cornerOffset - widthDif / 2, -cornerOffset - heightDif);
        bottomRightCorner.rectTransform.transform.localPosition = new Vector3(cornerOffset + widthDif / 2, -cornerOffset - heightDif);

        // Bottom border: x locked
        bottomBorder.rectTransform.sizeDelta = new Vector2(width, bottomBorder.rectTransform.sizeDelta.y);
        bottomBorder.rectTransform.transform.localPosition = new Vector3(bottomBorder.rectTransform.transform.localPosition.x, -borderOffsetY - heightDif);

        // Backer: x & y locked
        backer.rectTransform.sizeDelta = new Vector2(backerSize + widthDif, backerSize + heightDif);

        if(material != curMaterial) {
            curMaterial = material;
            // Load the new material
            //LoadBoxMaterial();
        }
    }

    void LoadBoxMaterial() {
        Sprite corner = null, border1 = null, border2 = null, back = null;
        Sprite[] boxResources = Resources.LoadAll<Sprite>("Art/UI/Menus/Tilable-Menus-Assets");

        switch(curMaterial) {
            case BOXMATERIAL.SIMPLEWOOD:
                corner = boxResources[14];
                border1 = boxResources[12];
                border2 = boxResources[13];
                back = boxResources[2];
                break;
            case BOXMATERIAL.DARKWOOD:
                corner = boxResources[5];
                border1 = boxResources[3];
                border2 = boxResources[4];
                back = boxResources[2];

                cornerOffset = 159.2f;
                borderOffsetX = 199.2f;
                borderOffsetY = 205.2f;
                break;
            case BOXMATERIAL.LIGHTWOOD:
                corner = boxResources[11];
                border1 = boxResources[9];
                border2 = boxResources[10];
                back = boxResources[8];
                break;
        }

        topLeftCorner.sprite = corner;
        topRightCorner.sprite = corner;
        bottomLeftCorner.sprite = corner;
        bottomRightCorner.sprite = corner;

        topBorder.sprite = border2;
        rightBorder.sprite = border1;
        bottomBorder.sprite = border2;
        leftBorder.sprite = border1;

        backer.sprite = back;
    }

}
