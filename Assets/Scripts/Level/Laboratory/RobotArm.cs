using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ARM_TYPE { FILLER = 0, PRESSER, DRILLER, STABBER };
public class RobotArm : MonoBehaviour {

    public ARM_TYPE armType;
    public bool working;

    Animator _animator;

    ConveyorManager _conveyor;

    ParticleSystem[] _steamParticles;

    private void Awake() {
        _animator = GetComponent<Animator>();
        _conveyor = FindObjectOfType<ConveyorManager>();

        _steamParticles = GetComponentsInChildren<ParticleSystem>();
    }
    // Start is called before the first frame update
    void Start() {
        BeginTask();
    }

    // Update is called once per frame
    void Update() {
        
    }

    public void BeginTask() {
        working = true;

        // Play our animation
        switch(armType) {
            case ARM_TYPE.FILLER:
                _animator.Play("Fill");
                break;
            case ARM_TYPE.PRESSER:
                _animator.Play("Press");
                break;
            case ARM_TYPE.DRILLER:
                _animator.Play("Drill");
                break;
            case ARM_TYPE.STABBER:
                _animator.Play("Stab");
                break;
        }
    }

    public void FinishedTask() {
        working = false;
    }

    public void SteamParticles() {
        foreach(ParticleSystem steam in _steamParticles) {
            steam.Play();
        }
    }

    public void PlaceTopper() {
        // Get the closest hamster container
        GameObject hamsterContainer = null;
        float closest = 1000, dist = 0;
        foreach(GameObject hC in _conveyor.hamsterContainers) {
            dist = Mathf.Abs(transform.position.x - hC.transform.position.x);
            if (dist < closest) {
                hamsterContainer = hC;
                closest = dist;
            }
        }

        // Turn on it's topper
        if (hamsterContainer != null) {
            hamsterContainer.transform.Find("Top").GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
