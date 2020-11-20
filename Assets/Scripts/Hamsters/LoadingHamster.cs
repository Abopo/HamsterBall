using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingHamster : MonoBehaviour {
    Animator _animator;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start() {
        _animator.SetInteger("State", 1);

        int color = Random.Range(0, 7);
        _animator.SetInteger("Type", color);
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void ChooseRandomHamster() {
        int color = Random.Range(0, 7);
        _animator.SetInteger("Type", color);
    }
}
