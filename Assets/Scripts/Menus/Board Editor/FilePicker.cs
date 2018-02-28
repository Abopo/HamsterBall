using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilePicker : MonoBehaviour {

    public Transform scrollViewContent;
    public GameObject fileObj;

    List<GameObject> files = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LoadFileData() {
        TextAsset[] allFiles = Resources.LoadAll<TextAsset>("Text/Created Boards");
        string[] linesFromFile;
        int index = 0;
        foreach (TextAsset tA in allFiles) {
            linesFromFile = tA.text.Split("\n"[0]);
            linesFromFile[0] = linesFromFile[0].Replace("\r", "");

            // Create a fileObj for each of the files loaded
            GameObject file = GameObject.Instantiate(fileObj, scrollViewContent);
            file.SetActive(true);
            file.transform.localPosition = new Vector3(170f, -25f - (40 * index), 0f);

            // Set the text to the name of the file
            file.GetComponentInChildren<Text>().text = linesFromFile[0];

            files.Add(file);

            index++;
        }
    }

    public void Open() {
        gameObject.SetActive(true);
        LoadFileData();
    }

    public void Close() {
        gameObject.SetActive(false);

        // Delete the boardfiles
        foreach(GameObject file in files) {
            DestroyObject(file);
        }

        files.Clear();
    }

    public void OpenHighlightedFile() {
        BoardFile[] boardFiles = FindObjectsOfType<BoardFile>();
        foreach(BoardFile bF in boardFiles) {
            if(bF.isHighlighted) {
                // Load this board
                BoardEditor boardEditor = FindObjectOfType<BoardEditor>();
                boardEditor.LoadBoard(bF.GetComponentInChildren<Text>().text);
            }
        }

        // Close the file picker
        Close();
    }
}
