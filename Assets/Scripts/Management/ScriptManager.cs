using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour {

    ScriptingController _scriptToRun;

    private void Awake() {
        _scriptToRun = GetComponent<ScriptingController>();
    }
    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {

    }

}
