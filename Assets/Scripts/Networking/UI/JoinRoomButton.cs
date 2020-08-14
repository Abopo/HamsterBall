using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoinRoomButton : MenuButton {
    public SuperTextMesh roomName;
    public OnlineLobby onlineLobby;

    public int numPlayers;
    public int maxPlayers;

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
            onlineLobby.TryJoinRoom(roomName.text);
        }
    }

    public void Click() {
        if (onlineLobby != null && numPlayers < maxPlayers) {
            onlineLobby.TryJoinRoom(roomName.text);
        } else {

        }
    }
}
