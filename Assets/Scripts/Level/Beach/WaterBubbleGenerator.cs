using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBubbleGenerator : MonoBehaviour {
    public int team;
    public GameObject waterBubbleObj;
    public GameObject bubbleFishObj;

    public float spawnDist;
    public int spawnCount;

    public WaterBubbleGenerator otherBubbleGenerator;

    float _baseSpawnTime = 4.0f;
    float _retrySpawnTime = 1.0f;
    float _curSpawnTime = 8.0f;
    float _spawnTimer = 0f;

    int _spawnOffset = 0;
    Vector3 _spawnPos;

    LevelManager _levelManager;
    GameManager _gameManager;

	// Use this for initialization
	void Start () {
        _levelManager = FindObjectOfType<LevelManager>();
        _gameManager = FindObjectOfType<GameManager>();
        _gameManager.gameOverEvent.AddListener(ClearAllBubbles);

        _spawnPos = new Vector3(transform.position.x,
                                transform.position.y + 0.3f,
                                transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
        if(_levelManager == null || _levelManager.GameOver || _gameManager.gameMode == GAME_MODE.SP_POINTS) {
            return;
        }

        _spawnTimer += Time.deltaTime;
        if(_spawnTimer >= _curSpawnTime) {
            // See if we should spawn a bubble
            if(ShouldSpawn()) {
                //SpawnBubble();
                SpawnBubbleFish();
                spawnCount++;
                _curSpawnTime = _baseSpawnTime;
            } else {
                _curSpawnTime = _retrySpawnTime;
            }

            _spawnTimer = 0f;
        }

        // If we've spawned more bubbles than the opposing side
        if(otherBubbleGenerator.spawnCount < spawnCount) {
            // Slow down our spawn rate
            _baseSpawnTime = 6.0f;
            _retrySpawnTime = 2.0f;
        } else {
            // Normal spawn rate
            // Slow down our spawn rate
            _baseSpawnTime = 4.0f;
            _retrySpawnTime = 1.0f;
        }
    }

    bool ShouldSpawn() {
        bool spawn = false;

        // TODO: Somehow make this more consistently fair for both sides, so everyone gets the same spawn rate (in terms of like, spawns per minute)
        int rand = Random.Range(0 + _spawnOffset, 21);
        if(rand == 20) {
            // We should spawn
            spawn = true;
            _spawnOffset = 0;
        } else {
            _spawnOffset++;
        }

        return spawn;
    }

    void ClearAllBubbles() {
        WaterBubble[] waterBubbles = FindObjectsOfType<WaterBubble>();
        foreach(WaterBubble wB in waterBubbles) {
            wB.Pop();
        }
    }

    public void SpawnBubbleFish() {
        float x = Random.Range(-spawnDist, spawnDist);
        Vector3 newSpawnPos = new Vector3(_spawnPos.x + x, -7.65f, _spawnPos.z);
        GameObject bubbleFish = Instantiate(bubbleFishObj, newSpawnPos, Quaternion.identity);
        bubbleFish.GetComponent<BubbleFish>().team = team;
    }
}
