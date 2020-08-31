using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Spawns a number of wandering hamsters depending on the village index
public class WanderingHamsterSpawner : MonoBehaviour {
    Object _wanderingHamsterObj;
    VillageManager _villageManager;

    HamsterRoom[] _hamsterRooms;
    HamsterDirector[] _hamsterDirectors;

    private void Awake() {
        _wanderingHamsterObj = Resources.Load("Prefabs/Village/WanderingHamster");

        _hamsterRooms = FindObjectsOfType<HamsterRoom>();
        _hamsterDirectors = FindObjectsOfType<HamsterDirector>();
    }
    // Start is called before the first frame update
    void Start() {
        StartCoroutine("SpawnHamsters");
        //SpawnHamsters();
    }

    IEnumerator SpawnHamsters() {
        yield return null;

        // Spawn x hamsters depending on village index
        int toSpawn = 10;
        int rand = 0;
        GameObject tempHamster;

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
                // TODO: don't spawn two hamsters at the same director
                rand = Random.Range(0, _hamsterDirectors.Length);

                tempHamster = Instantiate(_wanderingHamsterObj, transform) as GameObject;
                tempHamster.transform.position = _hamsterDirectors[rand].transform.position;

                // Choose a room
                tempHamster.GetComponent<WanderingHamster>().ChooseRoom();
                // Direct the hamsters
                //_hamsterDirectors[rand].DirectHamster(tempHamster.GetComponent<WanderingHamster>());
            }
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
