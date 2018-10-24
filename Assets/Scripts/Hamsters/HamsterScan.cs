using UnityEngine;
using System.Collections.Generic;

public class HamsterInfo {
    int specialSpawnOffset;
    List<int> okTypes;

    public int SpecialSpawnOffset {
        get { return specialSpawnOffset; }
        set { specialSpawnOffset = value; }
    }
    public List<int> OkTypes {
        get { return okTypes; }
        set { okTypes = value; }
    }
}

// This script scans the map (both sides) for hamsters and puts them in
// a list for the AI to access and make decisions with.
public class HamsterScan : MonoBehaviour {

    List<Hamster> _allHamsters = new List<Hamster>();
    List<Hamster> _allLeftHamsters = new List<Hamster>();
    List<Hamster> _allRightHamsters = new List<Hamster>();
    List<Hamster> _availableHamsters = new List<Hamster>();
    List<Hamster> _availableLeftHamsters = new List<Hamster>();
    List<Hamster> _availableRightHamsters = new List<Hamster>();
    Hamster[] tempHamsters;

    // This list will inform hamsters of what types are ok to spawn in as
    List<int> _okTypesLeft = new List<int>();
    List<int> _okTypesRight = new List<int>();
    // Used for counting up numbers of hamster types
    int[] _typeCounts = new int[8];

    // For AI purposes
    public List<Hamster> AllLeftHamsters {
        get { return _allLeftHamsters; }
    }
    public List<Hamster> AllRightHamsters {
        get { return _allRightHamsters; }
    }
    public List<Hamster> AvailableHamsters {
        get { return _availableHamsters; }
    }
    public List<Hamster> AvailableLeftHamsters {
        get { return _availableLeftHamsters; }
    }
    public List<Hamster> AvailableRightHamsters {
        get { return _availableRightHamsters; }
    }

    public HamsterInfo leftHamsterInfo = new HamsterInfo();
    public HamsterInfo rightHamsterInfo = new HamsterInfo();

    public int specialSpawnOffsetLeft;
    public List<int> OkTypesLeft {
        get { return _okTypesLeft; }
    }
    public int specialSpawnOffsetRight;
    public List<int> OkTypesRight {
        get { return _okTypesRight; }
    }

    // Single player only stuff //
    GameManager _gameManager;
    BubbleManager _bubbleManager;
    int[] _availableTypes = new int[8];

    private void Awake() {
        // Populate lists with all types
        for(int i = 0; i < 7; ++i) {
            _okTypesLeft.Add(i);
            _okTypesRight.Add(i);
        }

        leftHamsterInfo.SpecialSpawnOffset = -6;
        leftHamsterInfo.OkTypes = _okTypesLeft;
        rightHamsterInfo.SpecialSpawnOffset = -6;
        rightHamsterInfo.OkTypes = _okTypesRight;
    }

    // Use this for initialization
    void Start () {
        // Only used in single player modes
        _gameManager = FindObjectOfType<GameManager>();
        _bubbleManager = GameObject.FindGameObjectWithTag("BubbleManager1").GetComponent<BubbleManager>();
        _bubbleManager.boardChangedEvent.AddListener(ScanBoardForAvailableTypes);

        //FindHamsters();
        //ScanBoardForAvailableTypes();
        //UpdateLeftList();
        //UpdateRightList();
        //UpdateOKTypes();
    }

    // Update is called once per frame
    void Update () {
        // TODO: Right now doing this every frame, could optimize to only do on each hamster spawn.
        FindHamsters();

        //UpdateOKTypes();
	}

    void FindHamsters() {
        // Clear out the lists from last time.
        _allHamsters.Clear();
        _allLeftHamsters.Clear();
        _allRightHamsters.Clear();
        _availableHamsters.Clear();
        _availableLeftHamsters.Clear();
        _availableRightHamsters.Clear();

        // Find them hamsters
        tempHamsters = GameObject.FindObjectsOfType<Hamster>();
        foreach(Hamster hamster in tempHamsters) {
            if (hamster != null) {
                _allHamsters.Add(hamster);
                if (hamster.exitedPipe) {
                    _availableHamsters.Add(hamster);
                }
            }
        }

        // Sort the hamsters into left/right lists.
        foreach (Hamster hamster in _allHamsters) {
            if (hamster.transform.position.x < 0) {
                if (!_allLeftHamsters.Contains(hamster)) {
                    _allLeftHamsters.Add(hamster);
                }
            }
            if (hamster.transform.position.x > 0) {
                if (!_allRightHamsters.Contains(hamster)) {
                    _allRightHamsters.Add(hamster);
                }
            }
        }

        // AI stuff
        foreach (Hamster hamster in _availableHamsters) {
            if (hamster.transform.position.x < 0) {
                if (!_availableLeftHamsters.Contains(hamster)) {
                    _availableLeftHamsters.Add(hamster);
                }
            }
            if (hamster.transform.position.x > 0) {
                if (!_availableRightHamsters.Contains(hamster)) {
                    _availableRightHamsters.Add(hamster);
                }
            }
        }
    }

    void UpdateOKTypes() {
        // Reset lists
        _okTypesLeft.Clear();
        _okTypesRight.Clear();

        // Left Update
        foreach(Hamster hamster in _allLeftHamsters) {
            if (hamster.isGravity) {
                _typeCounts[6]++;
            } else {
                switch (hamster.type) {
                    case HAMSTER_TYPES.GREEN:
                        _typeCounts[0]++;
                        break;
                    case HAMSTER_TYPES.RED:
                        _typeCounts[1]++;
                        break;
                    case HAMSTER_TYPES.ORANGE:
                        _typeCounts[2]++;
                        break;
                    case HAMSTER_TYPES.GRAY:
                        _typeCounts[3]++;
                        break;
                    case HAMSTER_TYPES.BLUE:
                        _typeCounts[4]++;
                        break;
                    case HAMSTER_TYPES.PINK:
                        _typeCounts[5]++;
                        break;
                    case HAMSTER_TYPES.PURPLE:
                        _typeCounts[6]++;
                        break;
                    case HAMSTER_TYPES.RAINBOW:
                    case HAMSTER_TYPES.DEAD:
                        _typeCounts[7]++;
                        break;
                }
            }
        }
        PopulateOKTypesList(_okTypesLeft);

        // Right Update
        foreach (Hamster hamster in _allRightHamsters) {
            if (hamster.isGravity) {
                _typeCounts[6]++;
            } else {
                switch (hamster.type) {
                    case HAMSTER_TYPES.GREEN:
                        _typeCounts[0]++;
                        break;
                    case HAMSTER_TYPES.RED:
                        _typeCounts[1]++;
                        break;
                    case HAMSTER_TYPES.ORANGE:
                        _typeCounts[2]++;
                        break;
                    case HAMSTER_TYPES.GRAY:
                        _typeCounts[3]++;
                        break;
                    case HAMSTER_TYPES.BLUE:
                        _typeCounts[4]++;
                        break;
                    case HAMSTER_TYPES.PINK:
                        _typeCounts[5]++;
                        break;
                    case HAMSTER_TYPES.PURPLE:
                        _typeCounts[6]++;
                        break;
                    case HAMSTER_TYPES.RAINBOW:
                    case HAMSTER_TYPES.DEAD:
                    case HAMSTER_TYPES.BOMB:
                        _typeCounts[7]++;
                        break;
                }
            }
        }
        PopulateOKTypesList(_okTypesRight);
    }

    public void UpdateLeftList() {
        UpdateOKTypeList(_okTypesLeft, _allLeftHamsters);

        if(_gameManager.gameMode == GAME_MODE.SP_CLEAR) {
            // Remove from ok types any types that aren't currently on the board
            ReduceToAvailableTypes();
        }

        PopulateOKTypesList(_okTypesLeft);
    }

    public void UpdateRightList() {
        UpdateOKTypeList(_okTypesRight, _allRightHamsters);
        PopulateOKTypesList(_okTypesRight);
    }

    void UpdateOKTypeList(List<int> list, List<Hamster> hamsters) {
        // Reset lists
        list.Clear();

        foreach (Hamster hamster in hamsters) {
            if (hamster.isGravity) {
                _typeCounts[7]++;
            } else {
                switch (hamster.type) {
                    case HAMSTER_TYPES.GREEN:
                        _typeCounts[0]++;
                        break;
                    case HAMSTER_TYPES.RED:
                        _typeCounts[1]++;
                        break;
                    case HAMSTER_TYPES.ORANGE:
                        _typeCounts[2]++;
                        break;
                    case HAMSTER_TYPES.GRAY:
                        _typeCounts[3]++;
                        break;
                    case HAMSTER_TYPES.BLUE:
                        _typeCounts[4]++;
                        break;
                    case HAMSTER_TYPES.PINK:
                        _typeCounts[5]++;
                        break;
                    case HAMSTER_TYPES.PURPLE:
                        _typeCounts[6]++;
                        break;
                    case HAMSTER_TYPES.RAINBOW:
                    case HAMSTER_TYPES.DEAD:
                    case HAMSTER_TYPES.BOMB:
                        _typeCounts[7]++;
                        break;
                }
            }
        }
    }

    void PopulateOKTypesList(List<int> list) {
        for (int i = 0; i < 7; ++i) {
            // If there are less than 2 or 3 of a given type, it is OK to spawn as that type
            if (_typeCounts[i] < 3) {
                list.Add(i);
            }

            // Reset array for next side check
            _typeCounts[i] = 0;
        }

        // There can only be one special type on each side at a time.
        if (_typeCounts[7] < 1) {
            list.Add(7);
        }
        _typeCounts[7] = 0;
    }

    public Hamster GetHamster(int hamsterNum) {
        foreach (Hamster ham in AvailableHamsters) {
            if (ham.hamsterNum == hamsterNum) {
                return ham;
            }
        }

        return null;
    }


    // Single player only stuff //

    void ReduceToAvailableTypes() {
        // For each type
        for (int i = 0; i < 8; ++i) {
            // If it is not on the board, 
            if (_availableTypes[i] == 0) {
                // Don't spawn that type
                _typeCounts[i] = 10;
            }
        }
    }

    // This will trigger when the board changes
    void ScanBoardForAvailableTypes() {
        for(int i = 0; i < 8; ++i) {
            _availableTypes[i] = 0;
        }

        foreach (Bubble bub in _bubbleManager.Bubbles) {
            if(bub != null) {
                switch (bub.type) {
                    case HAMSTER_TYPES.GREEN:
                        _availableTypes[0] = 1;
                        break;
                    case HAMSTER_TYPES.RED:
                        _availableTypes[1] = 1;
                        break;
                    case HAMSTER_TYPES.ORANGE:
                        _availableTypes[2] = 1;
                        break;
                    case HAMSTER_TYPES.GRAY:
                        _availableTypes[3] = 1;
                        break;
                    case HAMSTER_TYPES.BLUE:
                        _availableTypes[4] = 1;
                        break;
                    case HAMSTER_TYPES.PINK:
                        _availableTypes[5] = 1;
                        break;
                    case HAMSTER_TYPES.PURPLE:
                        _availableTypes[6] = 1;
                        break;
                }
            }
        }
    }
}
