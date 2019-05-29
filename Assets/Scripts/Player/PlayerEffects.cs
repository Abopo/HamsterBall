using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEffects : MonoBehaviour {

    public int particleIndex; // This changes based on the kind of platform the player is standing on

    ParticleSystem footstepParticles1;
    ParticleSystem jumpingParticles1;
    ParticleSystem landingParticles1;
    ParticleSystem dashParticles1;

    ParticleSystem footstepParticles2;
    ParticleSystem jumpingParticles2;
    ParticleSystem landingParticles2;
    ParticleSystem dashParticles2;

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
            // Dash
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/GrassDash") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            dashParticles1 = tempObject.GetComponent<ParticleSystem>();
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
            // Dash
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/SnowDash") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            dashParticles1 = tempObject.GetComponent<ParticleSystem>();
        }

        // Beach
    }

    public void PlayFootstep() {
        switch(particleIndex) {
            case 0:
                if (footstepParticles1 != null) {
                    PlayEffect(footstepParticles1);
                }
                break;
            case 1:
                if (footstepParticles2 != null) {
                    PlayEffect(footstepParticles2);
                }
                break;
        }
    }

    public void PlayJumping() {
        switch(particleIndex) {
            case 0:
                if (jumpingParticles1 != null) {
                    PlayEffect(jumpingParticles1);
                }
                break;
            case 1:
                if (jumpingParticles2 != null) {
                    PlayEffect(jumpingParticles2);
                }
                break;
        }
    }

    public void PlayLanding() {
        switch(particleIndex) {
            case 0:
                if (landingParticles1 != null) {
                    PlayEffect(landingParticles1);
                }
                break;
            case 1:
                if (landingParticles2 != null) {
                    PlayEffect(landingParticles2);
                }
                break;
        }
    }

    public void PlayDash() {
        switch (particleIndex) {
            case 0:
                if (dashParticles1 != null) {
                    PlayEffect(dashParticles1);
                }
                break;
            case 1:
                if (dashParticles2 != null) {
                    PlayEffect(dashParticles2);
                }
                break;
        }
    }

    void PlayEffect(ParticleSystem particles) {
        ProperFacing(particles);

        particles.Play();
    }

    void ProperFacing(ParticleSystem particles) {
        // Make sure the particle system is facing the correct way
        if(Mathf.Sign(transform.localScale.x) != Mathf.Sign(particles.transform.localScale.x)) {
            // If they aren't matching, flip the effect
            Vector3 theScale = particles.transform.localScale;
            theScale.x *= -1;
            particles.transform.localScale = theScale;
        }
    }
}
