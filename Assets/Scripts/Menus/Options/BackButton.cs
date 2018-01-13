using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Button))]
public class BackButton : MenuOption {

    // Use this for initialization
    protected override void Start () {
        base.Start();
	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    protected override void Select() {
        base.Select();

        GetComponent<Button>().onClick.Invoke();
    }

    public void BackToMainMenu() {
        SceneManager.LoadScene("MainMenu");
    }

    public void DisconnectFromRoom() {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("OnlineLobby");
    }
}
