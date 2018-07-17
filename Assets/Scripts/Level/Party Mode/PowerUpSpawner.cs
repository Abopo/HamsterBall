using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns power ups just like hamsters, only used in Party mode
public class PowerUpSpawner : MonoBehaviour {
    public GameObject powerUpObj;
    public bool rightSidePipe;

    public float _spawnTime;
    float _spawnTimer;

    Vector3 _spawnPosition;

    GameObject _powerUp; // reference to the currently spawned power up

    GameManager _gameManager;

    // Use this for initialization
    void Start () {
        ResetSpawnTime();

        powerUpObj = Resources.Load("Prefabs/Entities/Power Up") as GameObject;

        Transform spawnPoint = transform.GetChild(0);
        _spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z + 5f);

        _gameManager = FindObjectOfType<GameManager>();
    }

    void ResetSpawnTime() {
        _spawnTimer = 0f;
        _spawnTime = Random.Range(10, 20);
    }

    // Update is called once per frame
    void Update () {
        // Don't update if the game is over
        if (_gameManager.gameIsOver) {
            return;
        }

        if (_powerUp == null) {
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= _spawnTime) {
                SpawnPowerUp();

                // Choose the next power up right now

                _spawnTimer = 0;
            }
        }
    }

    void SpawnPowerUp() {
        _powerUp = Instantiate(powerUpObj, _spawnPosition, Quaternion.identity) as GameObject;
        PowerUp pUp = ChoosePowerUp(_powerUp);
        //PowerUp pUp = _powerUp.GetComponent<PowerUp>();
        if (rightSidePipe) {
            pUp.Flip();
            pUp.inRightPipe = true;
        } else {
            pUp.inRightPipe = false;
        }
        pUp.FaceUp();
    }

    PowerUp ChoosePowerUp(GameObject powUp) {
        PowerUp powerUp = null;
        int rPow = Random.Range(0, (int)POWERUPS.SMOKE);
        //int rPow = (int)POWERUPS.JUNK_SHIELD;

        switch ((POWERUPS)rPow) {
            case POWERUPS.ATK_UP:
                powerUp = powUp.AddComponent<AtkUpPower>();
                break;
            case POWERUPS.HAM_SPEED_UP:
                powerUp = powUp.AddComponent<HamSpeedUpPower>();
                break;
            case POWERUPS.INFINISHIFT:
                powerUp = powUp.AddComponent<InfinishiftPower>();
                break;
            case POWERUPS.JUNK_SHIELD:
                powerUp = powUp.AddComponent<JunkShieldPower>();
                break;
            case POWERUPS.MONOCHROME:
                powerUp = powUp.AddComponent<MonochromePower>();
                break;
            case POWERUPS.PLAY_SPEED_DOWN:
                powerUp = powUp.AddComponent<PlayerSpeedDownPower>();
                break;
            case POWERUPS.SMOKE:
                break;
            default:
                powerUp = new PowerUp();
                break;
        }

        return powerUp;
    }

}
