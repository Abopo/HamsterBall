using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

[RequireComponent(typeof(PhotonView))]
public class NetworkedHamster : Photon.MonoBehaviour {
    Hamster _hamster;
    PhotonView _photonView;

    Vector3 _hamsterPos;
    int _nextHamsterType = -1;
    Quaternion _correctRot;
    int _hamsterType;
    int _hamsterState;
    bool _exitedPipe;
    bool _hamsterInLine;

    public int corFacing;

    float timer;
    float time = 1f;

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
            stream.SendNext(_hamster.transform.position);
            stream.SendNext(_hamster.type);
            stream.SendNext(_hamster.curFacing);
            stream.SendNext(_hamster.CurState);
            stream.SendNext(_hamster.exitedPipe);
        } else {
            _hamsterPos = (Vector3)stream.ReceiveNext();

            _hamsterType = (int)stream.ReceiveNext();
            if(_hamsterType != (int)_hamster.type) {
                if(_hamster.isPlasma) {
                    _hamster.SetType(HAMSTER_TYPES.PLASMA, (HAMSTER_TYPES)_hamsterType);
                } else {
                    _hamster.SetType(_hamsterType);
                }
            }

            corFacing = (int)stream.ReceiveNext();
            _hamsterState = (int)stream.ReceiveNext();
            _exitedPipe = (bool)stream.ReceiveNext();
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
        if (!_hamster.inLine && !PhotonNetwork.isMasterClient) {
            timer += Time.deltaTime;
            if (timer >= time) {
                // If we are too far away from our master hamster
                if(Vector2.Distance(_hamsterPos, _hamster.transform.position) > 1) {
                    // Teleport to the position
                    // or maybe lerp there?
                    Debug.Log("Hamster too far, teleporting.");
                    _hamster.transform.position = _hamsterPos;

                    // If we teleport, we might miss pip stuff, so sync that too
                    if(_hamster.exitedPipe != _exitedPipe) {
                        _hamster.exitedPipe = _exitedPipe;
                    }
                }

                if (_hamster.curFacing != corFacing) {
                    //Debug.Log("Correcting hamster facing from " + _hamster.curFacing + " to " + corFacing + ".");
                    CorrectFacing(corFacing);
                }

                // synch the state
                if (_hamsterState != _hamster.CurState) {
                    //Debug.Log("Correcting hamster state from " + _hamster.CurState + " to " + _hamsterState + ".");
                    _hamster.SetState(_hamsterState);
                }
            }
        } else {
            timer = 0f;
        }
    }
}
