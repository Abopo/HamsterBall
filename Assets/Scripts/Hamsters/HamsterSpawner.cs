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
    List<int> _okTypes;

    public static bool canBeRainbow = true;
    public static bool canBeDead = true;
    public static bool canBeGravity = true;
    public static bool canBeBomb = true;
    List<int> specialTypes = new List<int>();

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

        Transform spawnPoint = transform.GetChild(0);
        _spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z + 5f);

        _nextHamsterType = -1;
        SetupSpecialTypes();
        _hamsterScan = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<HamsterScan>();
        if (team == 0) {
            _okTypes = _hamsterScan.OkTypesLeft;
        } else if (team == 1) {
            _okTypes = _hamsterScan.OkTypesRight;
        }
        _nextHamsterType = GetValidType();

        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        SetSpawnMax();

        testMode = _gameManager.testMode;

        nextHamsterNum = 0;
    }

    void SetupSpecialTypes() {
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
        object[] hamsterInfo = new object[3];
        if (rightSidePipe) {
            hamsterInfo[0] = true; // hamster.inRightPipe
        } else {
            hamsterInfo[0] = false;
        }
        hamsterInfo[1] = team;
        hamsterInfo[2] = _nextHamsterType;

        // Use the network instantiate method
        PhotonNetwork.Instantiate("Prefabs/Networking/Hamster_PUN", _spawnPosition, Quaternion.identity, 0, hamsterInfo);
    }

    public int GetValidType() {
        int rType = 0;

        // If we can be a special hamster
        if (_okTypes.Contains(7)) {
            int special = Random.Range(0, 16);
            if (special == 1 && specialTypes.Count > 0) {
                int sType = Random.Range(0, specialTypes.Count);
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
            } else {
                rType = SelectValidNormalType();
            }
        } else {
            rType = SelectValidNormalType();
        }

        return rType;
    }

    int SelectValidNormalType() {
        int validType = 0;
        // If special types are OK, don't include it here.
        int special = _okTypes.Contains(7) ? 1 : 0;
        int rIndex = Random.Range(0, _okTypes.Count - special);

        validType = _okTypes[rIndex];

        // Remove chosen type from okTypes
        _okTypes.RemoveAt(rIndex);

        // If okTypes is empty (excluding specials)
        if (_okTypes.Count <= special) {
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
}
