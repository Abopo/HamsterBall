using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDoor : VillageDoor {

    VillageCharacterSelect _characterSelectWindow;

    protected override void Start() {
        base.Start();

        _characterSelectWindow = FindObjectOfType<VillageCharacterSelect>();
    }

    protected override void EnterDoor() {
        // Open the character select window
        _characterSelectWindow.Activate(_playerController);
    }
}
