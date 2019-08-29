using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaCreatureSpawner : MonoBehaviour {
    public bool left;

    GameObject[] _seaCreatureObjs = new GameObject[4];

    GameObject _onScreenCreature; // Can only have one at a time

    float _time = 1.0f;
    float _timer = 0f;
    int rand;

	// Use this for initialization
	void Start () {
        // Load the sea creatures
        _seaCreatureObjs[0] = Resources.Load<GameObject>("Prefabs/Effects/Environmental/SeaCreatures/Jellyfish");
        _seaCreatureObjs[1] = Resources.Load<GameObject>("Prefabs/Effects/Environmental/SeaCreatures/Octopus");
        _seaCreatureObjs[2] = Resources.Load<GameObject>("Prefabs/Effects/Environmental/SeaCreatures/Shark");
        _seaCreatureObjs[3] = Resources.Load<GameObject>("Prefabs/Effects/Environmental/SeaCreatures/Whale");

        _onScreenCreature = new GameObject();
        _onScreenCreature.transform.position = new Vector3(18f, 0f);
    }

    // Update is called once per frame
    void Update () {
        // If the current creature has wandered far enough off screen
		if(Mathf.Abs(_onScreenCreature.transform.position.x) > 17f) {
            // Can spawn a new creature
            _timer += Time.deltaTime;
            if(_timer > _time) {
                // Try to spawn a creature
                rand = Random.Range(0, 20);
                if(rand == 0) {
                    // Spawn a creature
                    SpawnSeaCreature();
                }

                _timer = 0f;
            }
        }
	}

    void SpawnSeaCreature() {
        // Destroy the old creature
        Destroy(_onScreenCreature);

        rand = Random.Range(0, 4);
        _onScreenCreature = Instantiate(_seaCreatureObjs[rand], this.transform);
        float randf = Random.Range(-1f, 1f);
        if(left) {
            _onScreenCreature.transform.position = new Vector3(transform.position.x + 1.0f, transform.position.y + randf, _onScreenCreature.transform.position.z);
            _onScreenCreature.GetComponentInChildren<SeaCreature>().left = true;
        } else {
            _onScreenCreature.transform.position = new Vector3(transform.position.x - 1.0f, transform.position.y + randf, _onScreenCreature.transform.position.z);
            _onScreenCreature.GetComponentInChildren<SeaCreature>().left = false;
        }
    }
}
