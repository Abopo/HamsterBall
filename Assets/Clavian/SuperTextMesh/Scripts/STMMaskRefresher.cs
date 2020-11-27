using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Something weird with STM makes the masking get messed up sometimes when the game object is disabled/reenabled.
// Seems to be fixed by just rebuilding when it's enabled
// Since RebuildAll() is global, this really only needs to be added to one SuperText per scene
public class STMMaskRefresher : MonoBehaviour {

    SuperTextMesh _superText;

    private void Awake() {
        _superText = GetComponent<SuperTextMesh>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    private void OnEnable() {
        SuperTextMesh.RebuildAll();
    }
}
