using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    // Use this for initialization
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

    public void ResumeButton() {
        Time.timeScale = 1;
        gameObject.SetActive(false);
    }

    public void Activate() {
        gameObject.SetActive(true);

        // Pause the game
        Time.timeScale = 0;
    }
}
