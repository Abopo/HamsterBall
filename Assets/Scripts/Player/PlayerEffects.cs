using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEffects : MonoBehaviour {

    PlayerController _playerController;

    // Particles
    ParticleSystem footstepParticles1;
    ParticleSystem jumpingParticles1;
    ParticleSystem landingParticles1;
    ParticleSystem dashParticles1;

    ParticleSystem footstepParticles2;
    ParticleSystem jumpingParticles2;
    ParticleSystem landingParticles2;
    ParticleSystem dashParticles2;

    // Sounds
    public FMOD.Studio.EventInstance footstepEvent1;
    public FMOD.Studio.EventInstance footstepEvent2;

    GameManager _gameManager;

    private void Awake() {
        _playerController = transform.parent.GetComponent<PlayerController>();
        _gameManager = FindObjectOfType<GameManager>();
    }

    // Use this for initialization
    void Start () {
        // TODO: Change particles based on stage

        LoadEffects();
	}

    void LoadEffects() {
        GameObject tempPrefab;
        GameObject tempObject;

        Scene activeScene = SceneManager.GetActiveScene();

        // Forest
        if (_gameManager.selectedBoard == BOARDS.FOREST || activeScene.name.Contains("Village")) {
            // Footstep
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/GrassFootstep") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            footstepParticles1 = tempObject.GetComponent<ParticleSystem>();
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/DustFootstep1") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            footstepParticles2 = tempObject.GetComponent<ParticleSystem>();
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
            // Sounds
            footstepEvent1 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.GrassPlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent1, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
            footstepEvent2 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.WoodPlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent2, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
        }
        // Mountain
        if (_gameManager.selectedBoard == BOARDS.MOUNTAIN) {
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
            // Sounds
            footstepEvent1 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.SnowPlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent1, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
            footstepEvent2 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.IcePlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent2, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
        }
        // Beach
        if (_gameManager.selectedBoard == BOARDS.BEACH) {
            // Footstep
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/DustFootstep1") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            footstepParticles1 = tempObject.GetComponent<ParticleSystem>();
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/SplashFootstep") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            footstepParticles2 = tempObject.GetComponent<ParticleSystem>();
            // Jump
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/SplashJump") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            jumpingParticles2 = tempObject.GetComponent<ParticleSystem>();
            // Land
            landingParticles2 = jumpingParticles2;
            // Dash
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/SnowDash") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            dashParticles1 = tempObject.GetComponent<ParticleSystem>();
            // Sounds
            footstepEvent1 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.SnowPlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent1, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
            footstepEvent2 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.IcePlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent2, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
        }
    }

    // Update is called once per frame
    void Update () {
        footstepEvent1.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerController.gameObject, _playerController.GetComponent<Rigidbody>()));
        footstepEvent2.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerController.gameObject, _playerController.GetComponent<Rigidbody>()));
    }

    public void PlayFootstep() {
        switch(_playerController.platformIndex) {
            case 0:
                if (footstepParticles1 != null) {
                    PlayEffect(footstepParticles1);
                }
                footstepEvent1.start();
                break;
            case 1:
                if (footstepParticles2 != null) {
                    PlayEffect(footstepParticles2);
                }
                footstepEvent2.start();
                break;
        }
    }

    public void PlayJumping() {
        switch(_playerController.platformIndex) {
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
        switch(_playerController.platformIndex) {
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
        switch (_playerController.platformIndex) {
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
