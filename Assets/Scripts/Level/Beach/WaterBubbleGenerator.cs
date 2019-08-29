using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBubbleGenerator : MonoBehaviour {
    public int team;
    public GameObject waterBubbleObj;
    public GameObject bubbleFishObj;

    float _baseSpawnTime = 8.0f;
    float _retrySpawnTime = 4.0f;
    float _curSpawnTime = 8.0f;
    float _spawnTimer = 0f;

    int _spawnOffset = 0;
    Vector3 _spawnPos;

	// Use this for initialization
	void Start () {
        _spawnPos = new Vector3(transform.position.x,
                                transform.position.y + 0.3f,
                                transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
        _spawnTimer += Time.deltaTime;
        if(_spawnTimer >= _curSpawnTime) {
            // See if we should spawn a bubble
            if(ShouldSpawn()) {
                //SpawnBubble();
                SpawnBubbleFish();
                _curSpawnTime = _baseSpawnTime;
            } else {
                _curSpawnTime = _retrySpawnTime;
            }

            _spawnTimer = 0f;
        }
	}

    bool ShouldSpawn() {
        bool spawn = false;

        // TODO: Somehow make this more consistently fair for both sides, so everyone gets the same spawn rate (in terms of like, spawns per minute)
        int rand = Random.Range(0 + _spawnOffset, 11);
        if(rand == 10) {
            // We should spawn
            spawn = true;
            _spawnOffset = 0;
        } else {
            _spawnOffset++;
        }

        return spawn;
    }

    public void SpawnBubble() {
        float x = Random.Range(-2f, 2f);
        Vector3 newSpawnPos = new Vector3(_spawnPos.x + x, _spawnPos.y, _spawnPos.z);
        GameObject waterBubble = Instantiate(waterBubbleObj, newSpawnPos, Quaternion.identity);
        waterBubble.GetComponent<WaterBubble>().team = team;
    }
    public void SpawnBubbleFish() {
        float x = Random.Range(-1.5f, 1.5f);
        Vector3 newSpawnPos = new Vector3(_spawnPos.x + x, -7.65f, _spawnPos.z);
        GameObject bubbleFish = Instantiate(bubbleFishObj, newSpawnPos, Quaternion.identity);
        bubbleFish.GetComponent<BubbleFish>().team = team;
    }
}
