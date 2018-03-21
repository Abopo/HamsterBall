using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLine : MonoBehaviour {

    GameObject _aimDotObj;

    List<GameObject> _aimDots = new List<GameObject>();

    bool _generating;
    float _generateTime = 0.1f;
    float _generateTimer = 0f;

	// Use this for initialization
	void Start () {
        _aimDotObj = Resources.Load<GameObject>("Prefabs/Effects/AimDot");
	}
	
	// Update is called once per frame
	void Update () {
        if (_generating) {
            _generateTimer += Time.deltaTime;
            if (_generateTimer >= _generateTime) {
                GenerateAimDot();
                _generateTimer = 0f;
            }
        }
	}

    void GenerateAimDot() {
        GameObject aimDot = Instantiate(_aimDotObj, transform);
        aimDot.transform.localPosition = new Vector3(0.5f, 0f, -1f);
        aimDot.transform.localScale = new Vector3(2f, 2f, 2f);
        _aimDots.Add(aimDot);
    }

    public void Begin() {
        _generating = true;
    }

    public void Stop() {
        // Destroy all aim dots
        foreach(GameObject aimDot in _aimDots) {
            DestroyObject(aimDot);
        }

        _aimDots.Clear();

        _generating = false;
    }
}
