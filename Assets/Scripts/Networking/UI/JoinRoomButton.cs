using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomButton : MenuButton {
    public SuperTextMesh roomName;
    public OnlineLobby onlineLobby;

    // Use this for initialization
    protected override void Start() {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update() {
        base.Update();
    }

    protected override void Select() {
        base.Select();

        GetComponent<Button>().onClick.Invoke();

        if (onlineLobby != null) {
            onlineLobby.JoinRoom(roomName.text);
        }
    }

    public void Click() {
        if (onlineLobby != null) {
            onlineLobby.JoinRoom(roomName.text);
        }
    }
}
