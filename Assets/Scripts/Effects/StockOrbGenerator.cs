using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockOrbGenerator : MonoBehaviour {

    public GameObject stockOrbObj;
    public int team;
    public BubbleEffects bubbleEffects;

    Vector3 _spawnPos;
    int _spawnAmount = 0;
    int _spawnCount = 0;
    float _spawnTime = 0.25f;
    float _spawnTimer = 0.0f;

    GameManager _gameManager;

    // Use this for initialization
    void Start () {
        _gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update () {
        // Don't update if the game is over
        if (_gameManager.gameIsOver) {
            return;
        }

        //if (_spawning) {
        _spawnTimer += Time.deltaTime;
            if (_spawnTimer > _spawnTime) {
                SpawnStockOrb();
                _spawnTimer = 0.0f;
                _spawnCount++;
            }
            if (_spawnCount >= _spawnAmount) {
                // Once all spawning is finished, destroy
                DestroyObject(this.gameObject);
            }
        //}
	}

    void SpawnStockOrb() {
        GameObject newStockOrb = GameObject.Instantiate(stockOrbObj, _spawnPos, Quaternion.identity);
        StockOrb stockOrb = newStockOrb.GetComponent<StockOrb>();
        stockOrb.Initialize();
        stockOrb.team = team;
        Transform target = bubbleEffects.GetNextTallyPosition();
        stockOrb.Launch(target);
    }

    public void BeginSpawning(int spawnAmount, Vector2 spawnPos) {
        _spawnAmount = spawnAmount;
        _spawnPos = new Vector3(spawnPos.x, spawnPos.y, -10f);
        _spawnTimer = 0.0f;
        _spawnCount = 0;
        //_spawning = true;
    }
}
