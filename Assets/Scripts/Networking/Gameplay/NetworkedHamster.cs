using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedHamster : Photon.MonoBehaviour {
    Hamster _hamster;
    PhotonView _photonView;

    int _nextHamsterType = -1;
    Quaternion _correctRot;
    int _hamsterType;
    int _hamsterState;

    public int corFacing;

    float timer;
    float time = 0.2f;

    private void Awake() {
        _hamster = GetComponent<Hamster>();
        _photonView = GetComponent<PhotonView>();
    }

    // Use this for initialization
    void Start() {
    }

    void OnPhotonInstantiate(PhotonMessageInfo info) {
        // Initialize hamster with team info
        _hamster.Initialize((int)_photonView.instantiationData[2]);

        // Set the hamsters type
        _nextHamsterType = (int)_photonView.instantiationData[3];
        if (_nextHamsterType != -1) {
            // If this hamster should be gravity
            if ((bool)_photonView.instantiationData[4]) {
                _hamster.SetType(HAMSTER_TYPES.PLASMA, (HAMSTER_TYPES)_nextHamsterType);
            } else {
                _hamster.SetType(_nextHamsterType);
            }
        }

        _hamster.hamsterNum = HamsterSpawner.nextHamsterNum++;

        // If it spawned from a pipe
        if (!(bool)_photonView.instantiationData[0]) { // if has not exited pipe
            HamsterSpawner hSpawner = FindObjectOfType<HamsterSpawner>();
            // TODO: for some reason the hamsters in the right pipe are walking backwards
            if ((bool)_photonView.instantiationData[1]) { // right side pipe
                _hamster.Flip();
                _hamster.inRightPipe = true;
                if (hSpawner.twoTubes) {
                    _hamster.FaceRight();
                } else {
                    _hamster.FaceLeft();
                }
            } else {
                _hamster.inRightPipe = false;
                if (hSpawner.twoTubes) {
                    _hamster.FaceLeft();
                } else {
                    _hamster.FaceRight();
                }
            }

            // Set a parent spawner based on pipe info
            GameObject[] spawners = GameObject.FindGameObjectsWithTag("Hamster Spawner");
            HamsterSpawner hS;
            foreach (GameObject s in spawners) {
                hS = s.GetComponent<HamsterSpawner>();
                if (hS.team == _hamster.team && hS.rightSidePipe == _hamster.inRightPipe) {
                    _hamster.ParentSpawner = hS;
                    hS.HamsterLine.Add(_hamster);
                }
            }
        // If it was dropped by player or something
        } else {
            _hamster.exitedPipe = true;

            // Set a parent spawner based on team
            GameObject[] spawners = GameObject.FindGameObjectsWithTag("Hamster Spawner");
            HamsterSpawner hS;
            foreach (GameObject s in spawners) {
                hS = s.GetComponent<HamsterSpawner>();
                if (hS.team == _hamster.team) {
                    _hamster.ParentSpawner = hS;
                    //hS.HamsterLine.Add(_hamster);
                }
            }
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            stream.SendNext(_hamster.type);
            stream.SendNext(_hamster.curFacing);
            stream.SendNext(_hamster.CurState);
            //stream.SendNext(_hamster.exitedPipe);
        } else {
            _hamsterType = (int)stream.ReceiveNext();
            if(_hamsterType != (int)_hamster.type) {
                if(_hamster.isPlasma) {
                    _hamster.SetType(HAMSTER_TYPES.PLASMA, (HAMSTER_TYPES)_hamsterType);
                } else {
                    _hamster.SetType(_hamsterType);
                }
            }

            corFacing = (int)stream.ReceiveNext();

            // synch the state
            _hamsterState = (int)stream.ReceiveNext();
            if(_hamsterState != _hamster.CurState) {
                _hamster.SetState(_hamsterState);
            }
        }
    }

    void CorrectFacing(int facing) {
        switch(facing) {
            case 0: // Right
                _hamster.FaceRight();
                break;
            case 1: // Down
                _hamster.FaceDown();
                break;
            case 2: // Left
                _hamster.FaceLeft();
                break;
            case 3: // Up
                _hamster.FaceUp();
                break;
        }
    }

    // Update is called once per frame
    void Update() {
        if (_hamster.curFacing != corFacing && !PhotonNetwork.isMasterClient) {
            timer += Time.deltaTime;
            if (timer >= time) {
                CorrectFacing(corFacing);
            }
        } else {
            timer = 0f;
        }
    }
}
