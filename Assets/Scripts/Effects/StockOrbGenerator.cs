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

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
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
        _spawnPos = new Vector3(spawnPos.x, spawnPos.y, -5f);
        _spawnTimer = 0.0f;
        _spawnCount = 0;
        //_spawning = true;
    }
}
