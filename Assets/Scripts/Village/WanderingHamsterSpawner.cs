using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns a number of wandering hamsters depending on the village index
public class WanderingHamsterSpawner : MonoBehaviour {
    Object _wanderingHamsterObj;
    VillageManager _villageManager;

    HamsterRoom[] _hamsterRooms;
    //HamsterDirector[] _hamsterDirectors;
    List<HamsterDirector> _hamsterDirectors = new List<HamsterDirector>();

    VillageManager _village;

    private void Awake() {
        _wanderingHamsterObj = Resources.Load("Prefabs/Village/WanderingHamster");

        _hamsterRooms = FindObjectsOfType<HamsterRoom>();

        // Get all the directors
        HamsterDirector[] hamDirectors = FindObjectsOfType<HamsterDirector>();
        foreach(HamsterDirector hD in hamDirectors) {
            _hamsterDirectors.Add(hD);
        }

        _village = FindObjectOfType<VillageManager>();
    }
    // Start is called before the first frame update
    void Start() {
        StartCoroutine("SpawnHamsters");
        //SpawnHamsters();
    }

    IEnumerator SpawnHamsters() {
        yield return null;

        // Spawn x hamsters depending on village index
        int toSpawn = HamstersToSpawn();
        int rand = 0;
        GameObject tempHamster;
        List<HamsterDirector> _tempDirectors = _hamsterDirectors;

        for (int i = 0; i < toSpawn; ++i) {
            // 50/50 chance to spawn out at a director or in a room
            rand = Random.Range(0, 2);
            if (rand == 0) {
                // Spawn in a room that's not full
                do {
                    rand = Random.Range(0, _hamsterRooms.Length);
                } while (_hamsterRooms[rand].IsFull);

                tempHamster = Instantiate(_wanderingHamsterObj, transform) as GameObject;
                tempHamster.transform.position = _hamsterRooms[rand].transform.position;

                // Put the hamster in the room
                _hamsterRooms[rand].TakeHamster(tempHamster.GetComponent<WanderingHamster>());
                // Also need to make sure the hamCount is accurate
                _hamsterRooms[rand].hamCount++;
            } else {
                // Spawn at a director
                rand = Random.Range(0, _tempDirectors.Count);

                tempHamster = Instantiate(_wanderingHamsterObj, transform) as GameObject;
                tempHamster.transform.position = _tempDirectors[rand].transform.position;

                // Remove chosen director from list
                _tempDirectors.RemoveAt(rand);

                // Choose a room
                tempHamster.GetComponent<WanderingHamster>().ChooseRoom();
            }

            // Initialize the hamster type
            // The first 7 hamsters are in type order
            if(i < (int)HAMSTER_TYPES.NUM_NORM_TYPES) {
                tempHamster.GetComponent<WanderingHamster>().SetType(i);
            // Any hamster's type beyond 7 is random
            } else {
               int _type = Random.Range(0, (int)HAMSTER_TYPES.NUM_NORM_TYPES);
                tempHamster.GetComponent<WanderingHamster>().SetType(_type);
            }
        }
    }

    int HamstersToSpawn() {
        int hts = 7;

        switch(_village.villageIndex) {
            case 0: // Full spawn
                hts = 10;
                break;
            case 1: // No spawn
            case 2:
                hts = 0;
                break;
            case 3:
                hts = 1;
                break;
            case 4:
                hts = 2;
                break;
            case 5:
                hts = 3;
                break;
            case 6:
                hts = 4;
                break;
            case 7:
                hts = 5;
                break;
            case 8:
                hts = 6;
                break;
            case 9:
                hts = 7;
                break;
            case 10:
                hts = 8;
                break;
            case 11:
                hts = 9;
                break;
            case 12:
                hts = 10;
                break;
        }

        return hts;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
