using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FilePicker : MonoBehaviour {

    public RectTransform scrollViewContent;
    public GameObject fileObj;

    List<GameObject> files = new List<GameObject>();

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //scrollViewContent.sizeDelta = new Vector2(scrollViewContent.sizeDelta.x, 300 + (files.Count - 8) * 30);
    }

    void LoadFileData() {
        // Clear out the files list
        files.Clear();

#if UNITY_EDITOR
        // Load the included boards
        TextAsset[] allFiles = Resources.LoadAll<TextAsset>("Text/Created Boards");
        string[] linesFromFile;
        foreach (TextAsset tA in allFiles) {
            linesFromFile = tA.text.Split("\n"[0]);
            linesFromFile[0] = linesFromFile[0].Replace("\r", "");

            CreateFileObject(linesFromFile[0]);
        }
#else
        // Load any player-made boards
        DirectoryInfo createdBoards = new DirectoryInfo(Application.dataPath + "/Created Boards");
        FileInfo[] boardFiles = createdBoards.GetFiles();
        foreach(FileInfo bF in boardFiles) {
            string name = bF.Name.Replace(".txt", "");
            CreateFileObject(name);
        }
#endif

        int index = 0;
        // Place files in proper positions
        foreach (GameObject file in files) {
            file.transform.localPosition = new Vector3(170f, -25f - (40 * index), 0f);
            index++;
        }

        // Add 30 per file over 8
        scrollViewContent.sizeDelta = new Vector2(scrollViewContent.sizeDelta.x, 300 + (files.Count - 8) * 60);
    }

    void CreateFileObject(string name) {
        // Create a fileObj for each of the files loaded
        GameObject file = GameObject.Instantiate(fileObj, scrollViewContent);
        file.SetActive(true);
        //file.transform.localPosition = new Vector3(170f, -25f - (40 * index), 0f);

        // Set the text to the name of the file
        file.GetComponentInChildren<Text>().text = name;

        files.Add(file);
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

    public void DeleteHighlightedFile() {
        string fullFileName = "Assets/Resources/Text/Created Boards/";
        BoardFile[] boardFiles = FindObjectsOfType<BoardFile>();

        foreach (BoardFile bF in boardFiles) {
            if (bF.isHighlighted) {
                // Get path of this file
                fullFileName += bF.GetComponentInChildren<Text>().text + ".txt";
                break;
            }
        }

        // Delete the file
        File.Delete(fullFileName);

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif

        // Delete the boardfiles
        foreach (GameObject file in files) {
            DestroyObject(file);
        }
        // Reload the file data
        LoadFileData();
    }
}
