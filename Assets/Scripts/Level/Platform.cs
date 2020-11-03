using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour {

    public int platformIndex; // This is used to tell the player what particles to create on footsteps/jumps/lands
    protected Animator _animator;

    protected virtual void Awake() {
        _animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void React() {
        if (_animator != null) {
            // Play the react animation
            _animator.Play("PlayerLandReact");
        }
    }
}
