using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPickButton : MonoBehaviour {

    public GameObject levelObject;
    public string levelName;

    BoardEditor _boardEditor;

	// Use this for initialization
	void Start () {
        _boardEditor = FindObjectOfType<BoardEditor>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnMouseUp() {
        // Load in the level geometry for this level
        GameObject newLevel = GameObject.Instantiate(levelObject, new Vector3(0, 0, 0), Quaternion.identity);
        HamsterSpawner[] spawners = newLevel.GetComponentsInChildren<HamsterSpawner>();
        foreach(HamsterSpawner hS in spawners) {
            hS.enabled = false;
        }
        Ceiling ceiling = FindObjectOfType<Ceiling>();
        ceiling.enabled = false;

        // Delete and replace the old level
        _boardEditor.ChangeLevel(newLevel, levelObject.name, levelName);

        // Close the picker
        GetComponentInParent<LevelPicker>().Close();
    }
}
