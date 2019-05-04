using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns power ups just like hamsters, only used in Party mode
public class PowerUpSpawner : MonoBehaviour {
    public GameObject powerUpObj;

    public float _spawnTime;
    float _spawnTimer;

    Vector3 _spawnPosition;

    GameObject _powerUp;

    // There can only be one power up per side of the stage
    // References to the currently spawned power up
    public static GameObject _leftPowerUp;
    public static GameObject _rightPowerUp;

    GameManager _gameManager;
    HamsterSpawner _hamsterSpawner;

    // Use this for initialization
    void Start () {
        ResetSpawnTime();

        powerUpObj = Resources.Load("Prefabs/Entities/Power Up") as GameObject;

        Transform spawnPoint = transform.GetChild(0);
        _spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z + 5f);

        _gameManager = FindObjectOfType<GameManager>();
        _hamsterSpawner = GetComponent<HamsterSpawner>();
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

        if(_hamsterSpawner.team == 1 && _rightPowerUp == null) {
            _spawnTimer += Time.deltaTime;
        } else if(_hamsterSpawner.team == 0 && _leftPowerUp == null) {
            _spawnTimer += Time.deltaTime;
        }
    }

    public bool TrySpawnPowerUp() {
        if (_spawnTimer >= _spawnTime) {
            SpawnPowerUp();

            // Choose the next power up right now (for networking?)

            _spawnTimer = 0;

            return true;
        }

        return false;
    }

    void SpawnPowerUp() {
        PowerUp pUp;
        if(_hamsterSpawner.team == 1) {
            _rightPowerUp = Instantiate(powerUpObj, _spawnPosition, Quaternion.identity) as GameObject;
            pUp = ChoosePowerUp(_rightPowerUp);
        } else {
            _leftPowerUp = Instantiate(powerUpObj, _spawnPosition, Quaternion.identity) as GameObject;
            pUp = ChoosePowerUp(_leftPowerUp);
        }
        if (_hamsterSpawner.rightSidePipe) {
            pUp.Flip();
            pUp.inRightPipe = true;
            if (_hamsterSpawner.twoTubes) {
                pUp.FaceRight();
            } else {
                pUp.FaceLeft();
            }
        } else {
            pUp.inRightPipe = false;
            if (_hamsterSpawner.twoTubes) {
                pUp.FaceLeft();
            } else {
                pUp.FaceRight();
            }
        }
        pUp.ParentSpawner = _hamsterSpawner;

        _hamsterSpawner.HamsterLine.Add(pUp);
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
