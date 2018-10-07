using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockOrbTester : MonoBehaviour {

    public GameObject stockOrbGeneratorObj;
    public HamsterMeter hamsterMeter;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.O)) {
            StockOrbEffect(1, transform.position);
        }
	}

    public void StockOrbEffect(int spawnAmount, Vector3 pos) {
        // Create new StockOrbGenerator
        GameObject stockOrbGenerator = GameObject.Instantiate(stockOrbGeneratorObj, pos, Quaternion.identity);
        StockOrbGenerator soGenerator = stockOrbGenerator.GetComponent<StockOrbGenerator>();
        soGenerator.team = 0;
        soGenerator.hamsterMeter = hamsterMeter;

        soGenerator.BeginSpawning(spawnAmount, pos);
    }
}
