using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Just plays a single animation
public class BasicAnimate : MonoBehaviour {
    public string animationToPlay;

    Animator _animator;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start() {
        _animator.Play(animationToPlay);    
    }

    // Update is called once per frame
    void Update() {
        
    }
}
