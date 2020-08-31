using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindRoomMenu : Menu {
    public string roomName;
    public GameObject menuObj;

    // Start is called before the first frame update
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();

        if(InputState.GetButtonOnAnyControllerPressed("Cancel")) {
            Deactivate();
        }
    }

    public override void Activate() {
        base.Activate();

        menuObj.SetActive(true);
    }

    public override void Deactivate() {
        base.Deactivate();

        menuObj.SetActive(false);
    }
}
