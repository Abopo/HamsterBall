using UnityEngine;
using System.Collections;
using Rewired.UI.ControlMapper;

public class OptionsMenu : MonoBehaviour {

    public OptionsPage[] pages;

    int _curPageIndex;

    ControlMapper _controlMapper;

    // Use this for initialization
    void Start () {
        _curPageIndex = 0;

        // Find the control remapper canvas from the game manager
        _controlMapper = FindObjectOfType<GameManager>().GetComponentInChildren<ControlMapper>();
	}
	
	// Update is called once per frame
	void Update () {
        if(InputState.GetButtonOnAnyControllerPressed("Right")) {
            // Move page right
            MovePageRight();
        } else if(InputState.GetButtonOnAnyControllerPressed("Left")) {
            // Move page left
            MovePageLeft();
        }
	}

    void MovePageRight() {
        pages[_curPageIndex].Deactivate();

        _curPageIndex++;
        if(_curPageIndex >= pages.Length) {
            _curPageIndex = 0;
        }

        pages[_curPageIndex].Activate();
    }

    void MovePageLeft() {
        pages[_curPageIndex].Deactivate();

        _curPageIndex--;
        if (_curPageIndex < 0) {
            _curPageIndex = pages.Length-1;
        }

        pages[_curPageIndex].Activate();
    }

    public void OpenControlRemapper() {
        _controlMapper.Open();

        // Disable focus on current page
        pages[_curPageIndex].LoseFocus();
    }

    public void CloseControlRemapper() {
        _controlMapper.Close(false);

        // Enable current page
        pages[_curPageIndex].TakeFocus();
    }

    public void DeleteSaveData() {
        // Delete the main es3 file
        ES3.DeleteFile();
    }
}
