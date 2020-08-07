﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleFish : MonoBehaviour {
    public GameObject waterBubbleObj;
    public int team;

    float _swimSpeed = 1f;
    float _blowPos = -6.11f;

    bool _blewBubble = false;
    float _blowBubbleTime = 1.0f;
    float _blowBubbleTimer = 0f;

    bool _swimDown;

    Animator _animator;

    PhotonView _photonView;

    private void Awake() {
        _animator = GetComponentInChildren<Animator>();
    }
    // Use this for initialization
    void Start () {
        _photonView = transform.parent.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update () {
		if(!_blewBubble) {
            // Move up to the blow position
            transform.parent.Translate(0f, _swimSpeed * Time.deltaTime, 0f);

            if(transform.position.y >= _blowPos) {
                BlowBubble();
            }
        } else{
            _blowBubbleTimer += Time.deltaTime;
            if (_blowBubbleTimer > _blowBubbleTime && _swimDown) {
                // Move down offscreen
                transform.parent.Translate(0f, -_swimSpeed * 1.5f * Time.deltaTime, 0f);

                // If we are sufficiently offscreen, destroy self
                if (transform.parent.position.y < -8f) {
                    Destroy(transform.parent.gameObject);
                }
            }
        }
	}

    void BlowBubble() {
        Vector3 newSpawnPos = new Vector3(transform.parent.position.x, transform.parent.position.y+0.3f, transform.parent.position.z);

        if (PhotonNetwork.connectedAndReady) {
            // Only the master client should spawn things
            if (PhotonNetwork.isMasterClient) {
                object[] data = new object[1];
                data[0] = team;
                GameObject waterBubble = PhotonNetwork.Instantiate("Prefabs/Networking/WaterBubble_PUN", newSpawnPos, Quaternion.identity, 0, data);
                waterBubble.GetComponent<WaterBubble>().team = team;
            }
        } else {
            GameObject waterBubble = Instantiate(waterBubbleObj, newSpawnPos, Quaternion.identity);
            waterBubble.GetComponent<WaterBubble>().team = team;
        }

        _blewBubble = true;
        _animator.SetBool("BlewBubble", true);
    }

    public void SwimDown() {
        _swimDown = true;
    }
}
