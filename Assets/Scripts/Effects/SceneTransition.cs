using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour {

    Scene _curScene;
    string _sceneToLoad;
    bool _transitioning;

    Animator _animator;
    LoadingHamster _hamster;

    Camera _oldCamera;

    public FMOD.Studio.EventInstance WheelSqueakEvent;

    private void Awake() {
        _animator = GetComponent<Animator>();
        _hamster = GetComponentInChildren<LoadingHamster>();
    }
    // Start is called before the first frame update
    void Start() {
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += EndTransition;

        // Get the camera in the scene, and scale ourselves to it
        _oldCamera = FindObjectOfType<Camera>();
        transform.localScale = new Vector3(_oldCamera.orthographicSize / 6.4f, _oldCamera.orthographicSize / 6.4f, _oldCamera.orthographicSize / 6.4f);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void StartTransition(string scene) {

        
        // Save the current scene to unload later
        _curScene = SceneManager.GetActiveScene();

        _sceneToLoad = scene;

        // Get a random hamster
        _hamster.ChooseRandomHamster();

        _animator.SetBool("End", false);
        _animator.Play("SceneTransStart");

        //WheelSqueakEvent = FMODUnity.RuntimeManager.CreateInstance("event:/Menu Sounds/Hamster Wheel");
        //WheelSqueakEvent.start();
    }

    public void TransitionFilled() {
        // Load the next scene
        SceneManager.LoadSceneAsync(_sceneToLoad, LoadSceneMode.Additive);
    }

    void SceneLoaded(Scene scene, LoadSceneMode mode) {
        // If we loaded additive
        if (mode == LoadSceneMode.Additive) {
            // Set the new scene to active
            SceneManager.SetActiveScene(scene);

            // turn off the old camera
            _oldCamera.enabled = false;

            // Scale ourselves to the camera of the new scene to make sure everything looks ok
            CameraScale();

            // Unload the old scene
            SceneManager.UnloadSceneAsync(_curScene);

            // Wait until unloaded to end the transition

        // otherwise
        } else {
            // Scale ourselves to the camera of the new scene to make sure everything looks ok
            CameraScale();

            // Just end the transition now
            EndTransition(scene);
        }
    }

    void CameraScale() {
        float camScale = 1;
    
        Camera[] cams = FindObjectsOfType<Camera>();
        foreach(Camera cam in cams) {
            if(cam != _oldCamera) {
                camScale = cam.orthographicSize;
                // hold onto the new camera for next transition
                _oldCamera = cam;
            }
        }
        transform.localScale = new Vector3(camScale / 6.4f, camScale / 6.4f, camScale / 6.4f);
    }

    void EndTransition(Scene scene) {
        // Play end transition
        //WheelSqueakEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        _animator.SetBool("End", true);
    }
}
