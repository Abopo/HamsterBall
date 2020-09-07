using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;

[System.Serializable]
public class DialogueDictionary : SerializableDictionaryBase<int, string> { }

public class VillageHamster : MonoBehaviour {

    public int spawnIndex; // This determines when in the story this hamster appears

    // I'm thinking there might need to be a despawnIndex as well?

    public DialogueDictionary dialogueDictionary;
    public string demoDialogue;

    int _villageIndex; // The current index of the village

    HamsterDialogue hamsterDialogue;
    VillageManager _villageManager;

	// Use this for initialization
	protected virtual void Start () {
        _villageManager = FindObjectOfType<VillageManager>();
        _villageIndex = _villageManager.villageIndex;

        if (_villageIndex < spawnIndex && _villageIndex != 0) {
            gameObject.SetActive(false);
        } else {
            hamsterDialogue = GetComponentInChildren<HamsterDialogue>();

            if (hamsterDialogue != null && FindObjectOfType<GameManager>().demoMode) {
                hamsterDialogue.dialogue = demoDialogue;
            } else {
                // Search backwards for the closest dialogue to the current index
                int tempIndex = _villageIndex;
                if (hamsterDialogue != null) {
                    while (!dialogueDictionary.ContainsKey(tempIndex) && tempIndex > 0) {
                        tempIndex--;
                    }

                    if (dialogueDictionary.ContainsKey(tempIndex)) {
                        hamsterDialogue.dialogue = dialogueDictionary[tempIndex];
                    }
                }
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
