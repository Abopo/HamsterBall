using UnityEngine;
using System.Collections;
using Rewired.UI.ControlMapper;

public class OptionsMenu : MonoBehaviour {

    public OptionsPage[] pages;

    public PopupWindow[] popups;

    public ArrowButton leftArrow;
    public ArrowButton rightArrow;

    int _curPageIndex;
    int _curPopupIndex;

    // Use this for initialization
    void Start () {
        _curPageIndex = 0;
        FMODUnity.RuntimeManager.PlayOneShot("event:/Menu Sounds/Book Open");
        // Find the control remapper canvas from the game manager
        //_controlMapper = GameManager.instance.GetComponentInChildren<ControlMapper>();
    }
	
	// Update is called once per frame
	void Update () {
        /*
        if (!_poppedUp) {
            if (InputState.GetButtonOnAnyControllerPressed("PageRight")) {
                // Move page right
                MovePageRight();
            } else if (InputState.GetButtonOnAnyControllerPressed("PageLeft")) {
                // Move page left
                MovePageLeft();
            }
        }
        */
	}

    void MovePageRight() {
        //pages[_curPageIndex].Deactivate();


        // If we are chaning to the next page on the right, we need to close to the left
        pages[_curPageIndex].ClosePageLeft();

        _curPageIndex++;
        if(_curPageIndex >= pages.Length) {
            _curPageIndex = 0;
        }

        pages[_curPageIndex].OpenPageLeft();

        //pages[_curPageIndex].Activate();
    }

    void MovePageLeft() {
        //pages[_curPageIndex].Deactivate();
        // If we are chaning to the next page on the left, we need to close to the right
        pages[_curPageIndex].ClosePageRight();

        _curPageIndex--;
        if (_curPageIndex < 0) {
            _curPageIndex = pages.Length-1;
        }

        pages[_curPageIndex].OpenPageRight();

        //pages[_curPageIndex].Activate();
    }

    public void OpenPopup(int index) {
        popups[index].OpenPopup();

        _curPopupIndex = index;

        // Disable focus on current page
        pages[_curPageIndex].LoseFocus();
    }

    public void CloseControlRemapper() {
        popups[_curPopupIndex].ClosePopup();

        // Enable current page
        pages[_curPageIndex].TakeFocus();
    }

    public void DeleteSaveData() {
        // Delete the main es3 file
        ES3.DeleteFile();
    }
}
