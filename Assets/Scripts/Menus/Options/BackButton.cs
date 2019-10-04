using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(Button))]
public class BackButton : MenuButton {
    Image _image;
    GameManager _gameManager;

    // Use this for initialization
    protected override void Start () {
        base.Start();

        _image = GetComponent<Image>();
        _gameManager = FindObjectOfType<GameManager>();
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

    public void PlayBackSound() {
        switch (menuType) {
            case MENUTYPE.MAIN:
                FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.MainMenuBack);
                break;
            case MENUTYPE.SUB:
                FMODUnity.RuntimeManager.PlayOneShot(SoundManager.mainAudio.SubMenuBack);
                break;
        }
    }

    public void BackToMainMenu() {
        // Don't go back while in demo mode
        //if (!_gameManager.demoMode) {
            _gameManager.MainMenuButton();
        //}
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
