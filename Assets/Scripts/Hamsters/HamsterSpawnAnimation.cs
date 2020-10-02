using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterSpawnAnimation : MonoBehaviour {

    float _animTime = 0.5f;
    float _animTimer;

    bool _animOn;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    protected virtual void Update() {
        if(_animOn) {
            _animTimer += Time.deltaTime;
            if(_animTimer >= _animTime) {
                EndAnim();
            }
        }
    }

    // Animation for when a hamster is spawned
    public virtual void SpawnAnim() {
        _animOn = true;
        _animTimer = 0f;
    }

    // Animation for after the hamster is gone
    public virtual void EndAnim() {
        _animOn = false;
    }
}
