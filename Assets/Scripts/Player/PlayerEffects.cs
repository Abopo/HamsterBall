using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEffects : MonoBehaviour {

    public int particleIndex; // This changes based on the kind of platform the player is standing on

    ParticleSystem footstepParticles1;
    ParticleSystem jumpingParticles1;
    ParticleSystem landingParticles1;

    ParticleSystem footstepParticles2;
    ParticleSystem jumpingParticles2;
    ParticleSystem landingParticles2;

    // Use this for initialization
    void Start () {
        // TODO: Change particles based on stage

        LoadParticles();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void LoadParticles() {
        GameObject tempPrefab;
        GameObject tempObject;

        Scene activeScene = SceneManager.GetActiveScene();

        // Forest
        if (activeScene.name.Contains("Forest") || activeScene.name.Contains("Village")) {
            // Footstep
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/GrassFootstep") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            footstepParticles1 = tempObject.GetComponent<ParticleSystem>();
            // Jump
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/GrassJump") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            jumpingParticles1 = tempObject.GetComponent<ParticleSystem>();
            // Land
            landingParticles1 = jumpingParticles1;
        }
        // Mountain
        if (activeScene.name.Contains("Mountain")) {
            // Footstep
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/SnowFootstep") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            footstepParticles1 = tempObject.GetComponent<ParticleSystem>();
            // Jump
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/SnowJump") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            jumpingParticles1 = tempObject.GetComponent<ParticleSystem>();
            // Land
            landingParticles1 = jumpingParticles1;
        }

        // Beach
    }

    public void PlayFootstep() {
        switch(particleIndex) {
            case 0:
                if (footstepParticles1 != null) {
                    footstepParticles1.Play();
                }
                break;
            case 1:
                if (footstepParticles2 != null) {
                    footstepParticles2.Play();
                }
                break;
        }
    }

    public void PlayJumping() {
        switch(particleIndex) {
            case 0:
                if (jumpingParticles1 != null) {
                    jumpingParticles1.Play();
                }
                break;
            case 1:
                if (jumpingParticles2 != null) {
                    jumpingParticles2.Play();
                }
                break;
        }
    }

    public void PlayLanding() {
        switch(particleIndex) {
            case 0:
                if (landingParticles1 != null) {
                    landingParticles1.Play();
                }
                break;
            case 1:
                if (landingParticles2 != null) {
                    landingParticles2.Play();
                }
                break;
        }
    }
}
