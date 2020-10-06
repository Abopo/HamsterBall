using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeEntrance : MonoBehaviour {
    public HamsterSpawner parentSpawner;

    private void Awake() {
        if (parentSpawner == null) {
            parentSpawner = transform.parent.GetComponent<HamsterSpawner>();
        }
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
