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


    public List<Hamster> AllLeftHamsters {
        get { return _allLeftHamsters; }
    }
    // For AI purposes; only the hamsters that are out of the pipe
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


    private void Awake() {
        leftHamsterInfo.SpecialSpawnOffset = -6;
        leftHamsterInfo.OkTypes = _okTypesLeft;
        rightHamsterInfo.SpecialSpawnOffset = -6;
        rightHamsterInfo.OkTypes = _okTypesRight;

        FindHamsters();
        UpdateOKTypes();
    }

    // Use this for initialization
    void Start () {
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
    }

    public void UpdateRightList() {
        UpdateOKTypeList(_okTypesRight, _allRightHamsters);
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
        PopulateOKTypesList(list);
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
        foreach(Hamster ham in AvailableHamsters) {
            if(ham.hamsterNum == hamsterNum) {
                return ham;
            }
        }

        return null;
    }
}
