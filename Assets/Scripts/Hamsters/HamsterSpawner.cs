using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon;

public class HamsterSpawner : Photon.PunBehaviour {
    public GameObject hamsterObj;
    public bool twoTubes;
    public int team;
    public bool rightSidePipe;
    public bool testMode;

    public int hamsterCount;
    public int maxHamsterCount;

    public float _spawnTime;
    float _spawnTimer;

    Vector3 _spawnPosition;

    int _nextHamsterType;
    HamsterInfo _hamsterInfo;
    //List<int> _okTypes;
    //public int _specialSpawnOffset;

    public static bool canBeRainbow = true;
    public static bool canBeDead = true;
    public static bool canBeGravity = true;
    public static bool canBeBomb = true;
    List<int> specialTypes = new List<int>();

    System.Random _random;
    public static int spawnSeed;
    public static int nextHamsterNum;

    GameManager _gameManager;
    HamsterScan _hamsterScan;

    public int NextHamsterType {
        get { return _nextHamsterType; }

        set { _nextHamsterType = value; }
    }

    // Use this for initialization
    void Start() {
        //_spawnTime = 4;
        _spawnTimer = 0;
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        if (_gameManager.gameMode == GAME_MODE.SP_POINTS) {
            // Score attack stages have set spawn sequences
            _random = new System.Random(spawnSeed);
        } else {
            _random = new System.Random((int)Time.realtimeSinceStartup);
        }

        Transform spawnPoint = transform.GetChild(0);
        _spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z + 5f);

        _nextHamsterType = -1;
        SetupSpecialTypes();
        _hamsterScan = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<HamsterScan>();
        if (team == 0) {
            _hamsterInfo = _hamsterScan.leftHamsterInfo;
            //_okTypes = _hamsterScan.OkTypesLeft;
            //_specialSpawnOffset = _hamsterScan.specialSpawnOffsetLeft;
        } else if (team == 1) {
            _hamsterInfo = _hamsterScan.rightHamsterInfo;
            //_okTypes = _hamsterScan.OkTypesRight;
        }
        _nextHamsterType = GetValidType();

        SetSpawnMax();

        testMode = _gameManager.testMode;

        nextHamsterNum = 0;

    }

    void SetupSpecialTypes() {
        //_specialSpawnOffset = -6;

        if (canBeDead) {
            specialTypes.Add((int)HAMSTER_TYPES.DEAD);
        }
        if (canBeRainbow) {
            specialTypes.Add((int)HAMSTER_TYPES.RAINBOW);
            specialTypes.Add((int)HAMSTER_TYPES.RAINBOW); // Adding a second time increases the chance of this type to be chosen
        }
        if (canBeGravity) {
            specialTypes.Add(0);
        }
        if (canBeBomb) {
            specialTypes.Add((int)HAMSTER_TYPES.BOMB);
            specialTypes.Add((int)HAMSTER_TYPES.BOMB);
        }
    }

    void SetSpawnMax() {
        if (twoTubes) {
            maxHamsterCount = _gameManager.HamsterSpawnMax/2;
        } else {
            maxHamsterCount = _gameManager.HamsterSpawnMax;
        }
    }

    // Update is called once per frame
    void Update () {
        // Don't update if the game is over
        if (_gameManager.gameIsOver) {
            return;
        }

        _spawnTimer += Time.deltaTime;
		if (_spawnTimer >= _spawnTime && hamsterCount < maxHamsterCount) {
			SpawnHamster();
            // Choose the next hamster type right now
            _nextHamsterType = GetValidType();
            //Debug.Log(_nextHamsterType.ToString());
            _spawnTimer = 0;
		}

        if(testMode) {
            CheckInput();
        }
	}

    void CheckInput() {
        if (Input.GetKeyDown("1")) {
            _nextHamsterType = 0;
        }
        if (Input.GetKeyDown("2")) {
            _nextHamsterType = 1;
        }
        if (Input.GetKeyDown("3")) {
            _nextHamsterType = 2;
        }
        if (Input.GetKeyDown("4")) {
            _nextHamsterType = 3;
        }
        if (Input.GetKeyDown("5")) {
            _nextHamsterType = 4;
        }
        if (Input.GetKeyDown("6")) {
            _nextHamsterType = 5;
        }
        if (Input.GetKeyDown("7")) {
            _nextHamsterType = 8;
        }
        if (Input.GetKeyDown("8")) {
            _nextHamsterType = 9;
        }
        if (Input.GetKeyDown("9")) {
            _nextHamsterType = 10;
        }
    }

    void SpawnHamster() {
        GameObject hamsterGO;
        
        // If we are online and connected
        if (PhotonNetwork.connectedAndReady) {
            // And we are the master client
            if (PhotonNetwork.isMasterClient) {
                // Instantiate a hamster on the network
                InstantiateNetworkHamster();
            }
        } else {
            hamsterGO = Instantiate(hamsterObj, _spawnPosition, Quaternion.identity) as GameObject;
            Hamster hamster = hamsterGO.GetComponent<Hamster>();
            if (rightSidePipe) {
                hamster.Flip();
                hamster.inRightPipe = true;
            } else {
                hamster.inRightPipe = false;
            }
            hamster.FaceUp();
            hamster.Initialize(team);
            hamster.testMode = testMode;
            hamster.ParentSpawner = this;
            if (_nextHamsterType != -1) {
                hamster.SetType(_nextHamsterType);
                _nextHamsterType = -1;
            } else {
                int type = GetValidType();
                hamster.SetType(type);
            }

            hamster.hamsterNum = nextHamsterNum;
            nextHamsterNum++;

            // Increase hamster count
            hamsterCount++;
        }
    }

    void InstantiateNetworkHamster() {
        // Set up instantiation data
        object[] hamsterInfo = new object[5];
        hamsterInfo[0] = false; // has exitedPipe
        if (rightSidePipe) {
            hamsterInfo[1] = true; // hamster.inRightPipe
        } else {
            hamsterInfo[1] = false;
        }
        hamsterInfo[2] = team;
        hamsterInfo[3] = _nextHamsterType;
        hamsterInfo[4] = false; // isGravity

        // Use the network instantiate method
        PhotonNetwork.Instantiate("Prefabs/Networking/Hamster_PUN", _spawnPosition, Quaternion.identity, 0, hamsterInfo);
    }

    public int GetValidType() {
        int rType = 0;

        // If we can be a special hamster
        if (_hamsterInfo.OkTypes.Contains(7)) {
            if (_hamsterInfo.SpecialSpawnOffset >= 0) {
                // Decide if this hamster will be special
                int special = _random.Next(_hamsterInfo.SpecialSpawnOffset, 16);
                if (special == 15 && specialTypes.Count > 0) {
                    // Choose which special hamster it will be
                    int sType = _random.Next(0, specialTypes.Count);
                    switch (specialTypes[sType]) {
                        case 8: // Rainbow
                            rType = (int)HAMSTER_TYPES.RAINBOW;
                            break;
                        case 9: // Dead
                            rType = (int)HAMSTER_TYPES.DEAD;
                            break;
                        case 10: // Bomb
                            rType = (int)HAMSTER_TYPES.BOMB;
                            break;
                        case 0: // Gravity
                            rType = 11;
                            break;
                    }

                    // Remove specials from okTypes list
                    _hamsterInfo.OkTypes.Remove(7);
                    _hamsterInfo.SpecialSpawnOffset = -6;
                } else {
                    rType = SelectValidNormalType();
                }
            } else {
                rType = SelectValidNormalType();
            }

            // Increase the special spawn offset
            _hamsterInfo.SpecialSpawnOffset += 1;
            if(_hamsterInfo.SpecialSpawnOffset > 15) {
                _hamsterInfo.SpecialSpawnOffset = 15;
            }
        } else {
            rType = SelectValidNormalType();
        }

        return rType;
    }

    int SelectValidNormalType() {
        int validType = 0;
        // If special types are OK, don't include it here.
        int special = _hamsterInfo.OkTypes.Contains(7) ? 1 : 0;
        int rIndex = _random.Next(0, _hamsterInfo.OkTypes.Count - special);

        validType = _hamsterInfo.OkTypes[rIndex];

        // Remove chosen type from okTypes
        _hamsterInfo.OkTypes.RemoveAt(rIndex);

        // If okTypes is empty (excluding specials)
        if (_hamsterInfo.OkTypes.Count <= special) {
            // Update okTypes
            if (team == 0) {
                _hamsterScan.UpdateLeftList();
            } else if (team == 1) {
                _hamsterScan.UpdateRightList();
            }
        }

        return validType;
    }

    public void ReduceHamsterCount() {
        if(hamsterCount >= maxHamsterCount) {
            _spawnTimer = 0;
        }

        hamsterCount--;
    }

    private void OnDestroy() {
        // Reset spawn seed to true random
        //spawnSeed = (int)Time.realtimeSinceStartup;
    }

}
