using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Button))]
public class BackButton : MenuOption {
    Image _image;
    GameManager _gameManager;

    // Use this for initialization
    protected override void Start () {
        base.Start();

        _image = GetComponent<Image>();
        _gameManager = FindObjectOfType<GameManager>();

        if(SceneManager.GetActiveScene().name == "DemoCharacterSelect" && FindObjectOfType<GameManager>().demoMode) {
            gameObject.SetActive(false);
        }
	}

    // Update is called once per frame
    protected override void Update () {
        base.Update();
	}

    protected override void Select() {
        base.Select();

        GetComponent<Button>().onClick.Invoke();
    }

    public void Press() {

    }

    public void BackToMainMenu() {
        _gameManager.MainMenuButton();
    }

    public void BackToLocalPlay() {
        SceneManager.LoadScene("LocalPlay");
    }

    public void BackToCharacterSelect() {
        _gameManager.CharacterSelectButton();
    }

    public void DisconnectFromRoom() {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("OnlineLobby");
    }
}
