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

    ParticleSystem footstepParticles3;
    ParticleSystem jumpingParticles3;
    ParticleSystem landingParticles3;
    ParticleSystem dashParticles3;

    // Sounds
    public FMOD.Studio.EventInstance footstepEvent1;
    public FMOD.Studio.EventInstance footstepEvent2;
    public FMOD.Studio.EventInstance footstepEvent3;

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
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/GrassFootstep") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            footstepParticles1 = tempObject.GetComponent<ParticleSystem>();
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/DustFootstep1") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            footstepParticles2 = tempObject.GetComponent<ParticleSystem>();
            // Jump
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/GrassJump") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            jumpingParticles1 = tempObject.GetComponent<ParticleSystem>();
            // Land
            landingParticles1 = jumpingParticles1;
            // Dash
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/GrassDash") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            dashParticles1 = tempObject.GetComponent<ParticleSystem>();
            
            // Sounds
            footstepEvent1 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.GrassPlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent1, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
            footstepEvent2 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.WoodPlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent2, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());

        // Mountain
        } else if (_gameManager.selectedBoard == BOARDS.MOUNTAIN) {
            // Footstep
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/SnowFootstep") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            footstepParticles1 = tempObject.GetComponent<ParticleSystem>();
            // Jump
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/SnowJump") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            jumpingParticles1 = tempObject.GetComponent<ParticleSystem>();
            // Land
            landingParticles1 = jumpingParticles1;
            // Dash
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/SnowDash") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            dashParticles1 = tempObject.GetComponent<ParticleSystem>();
            
            // Sounds
            footstepEvent1 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.SnowPlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent1, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
            footstepEvent2 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.IcePlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent2, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
        // Beach
        } else if (_gameManager.selectedBoard == BOARDS.BEACH) {
            // Footstep
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/DustFootstep1") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            footstepParticles1 = tempObject.GetComponent<ParticleSystem>();
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/SplashFootstep") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            footstepParticles2 = tempObject.GetComponent<ParticleSystem>();
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/BubbleFootstep") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            footstepParticles3 = tempObject.GetComponent<ParticleSystem>();
            // Jump
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/SplashJump") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            jumpingParticles2 = tempObject.GetComponent<ParticleSystem>();
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/BubbleJump") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            jumpingParticles3 = tempObject.GetComponent<ParticleSystem>();
            // Land
            landingParticles2 = jumpingParticles2;
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/BubbleLand") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            tempObject.transform.localPosition = new Vector3(0f, -0.9f, -1f);
            landingParticles3 = tempObject.GetComponent<ParticleSystem>();
            // Dash
            tempPrefab = Resources.Load("Prefabs/Effects/Environmental/Footsteps/BubbleDash") as GameObject;
            tempObject = Instantiate(tempPrefab, this.transform, false);
            dashParticles3 = tempObject.GetComponent<ParticleSystem>();

            // Sounds
            footstepEvent1 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.WoodPlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent1, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
            footstepEvent2 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.IcePlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent2, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
            footstepEvent3 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.SnowPlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent3, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
        } else {
            footstepParticles1 = null;
            footstepParticles2 = null;
            footstepParticles3 = null;
            jumpingParticles1 = null;
            jumpingParticles2 = null;
            jumpingParticles3 = null;
            landingParticles1 = null;
            landingParticles2 = null;
            landingParticles3 = null;
            dashParticles1 = null;
            dashParticles2 = null;
            dashParticles3 = null;

            footstepEvent1 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.GrassPlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent1, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
            footstepEvent2 = FMODUnity.RuntimeManager.CreateInstance(SoundManager.mainAudio.WoodPlayerFootstep);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(footstepEvent2, _playerController.GetComponent<Transform>(), _playerController.GetComponent<Rigidbody>());
        }
    }

    // Update is called once per frame
    void Update () {
        footstepEvent1.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerController.gameObject, _playerController.GetComponent<Rigidbody>()));
        footstepEvent2.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerController.gameObject, _playerController.GetComponent<Rigidbody>()));
        footstepEvent3.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerController.gameObject, _playerController.GetComponent<Rigidbody>()));
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
            case 2:
                if (footstepParticles3 != null) {
                    PlayEffect(footstepParticles3);
                }
                footstepEvent3.start();
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
            case 2:
                if (jumpingParticles3 != null) {
                    PlayEffect(jumpingParticles3);
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
            case 2:
                if (landingParticles3 != null) {
                    PlayEffect(landingParticles3);
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
            case 2:
                if (dashParticles3 != null) {
                    PlayEffect(dashParticles3);
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

    public void Flip() {
        // Since changing the scale of the particle system does jack shit, we have to change the rotation instead
        float rotOffset = Mathf.Abs(transform.rotation.x) - 90;

        // If we are facing right
        if(transform.localScale.x > 0) {
            // For now only footsteps and dashes should really be affected
            transform.rotation = Quaternion.Euler(-90 - rotOffset, transform.rotation.y, transform.rotation.z);
            
            // Flip the scale anyway so we can keep track of facing
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}
