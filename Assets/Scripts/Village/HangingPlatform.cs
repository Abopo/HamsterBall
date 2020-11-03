using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangingPlatform : Platform {

    ParticleSystem _leafParticles;

    protected override void Awake() {
        base.Awake();

        _leafParticles = GetComponentInChildren<ParticleSystem>();
    }
    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }

    public override void React() {
        base.React();

        // Drop some leaves
        _leafParticles.Play();
    }
}
