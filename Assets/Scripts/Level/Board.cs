using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {

    public GameObject leftCeiling;
    public GameObject rightCeiling;
    public GameObject bigCeiling;

    public GameObject upperCenterWallCollider;
    public GameObject upperCenterWallOverlay;

    public HamsterSpawner rightSpawner1;
    public HamsterSpawner rightSpawner2;

    public GameObject leftDivider;
    public GameObject rightDivider;
    public GameObject bigDivider;

    GameManager _gameManager;

    private void Awake() {
    }
    // Start is called before the first frame update
    void Start() {
        _gameManager = FindObjectOfType<GameManager>();

        switch (_gameManager.gameMode) {
            case GAME_MODE.MP_VERSUS:
            case GAME_MODE.MP_PARTY:
                if (upperCenterWallCollider != null) {
                    upperCenterWallCollider.SetActive(true);
                }
                if (upperCenterWallOverlay != null) {
                    upperCenterWallOverlay.SetActive(true);
                }
                if (bigCeiling != null) {
                    bigCeiling.SetActive(false);
                }
                if (leftDivider != null) {
                    leftDivider.SetActive(true);
                }
                if (rightDivider != null) {
                    rightDivider.SetActive(true);
                }
                if (bigDivider != null) {
                    bigDivider.SetActive(false);
                }
                break;

            case GAME_MODE.SP_CLEAR:
            case GAME_MODE.SURVIVAL:
                if (leftCeiling != null) {
                    leftCeiling.SetActive(false);
                }
                if (rightCeiling != null) {
                    rightCeiling.SetActive(false);
                }
                if (bigCeiling != null) {
                    bigCeiling.SetActive(true);
                }
                goto case GAME_MODE.SP_POINTS;
            case GAME_MODE.SP_POINTS:
                rightSpawner1.enabled = false;
                if (rightSpawner2 != null) {
                    rightSpawner2.enabled = false;
                }
                break;

            case GAME_MODE.TEAMSURVIVAL:
                if (upperCenterWallCollider != null) {
                    upperCenterWallCollider.SetActive(false);
                }
                if (upperCenterWallOverlay != null) {
                    upperCenterWallOverlay.SetActive(false);
                }
                if (bigCeiling != null) {
                    bigCeiling.SetActive(false);
                }
                if (leftDivider != null) {
                    leftDivider.SetActive(false);
                }
                if (rightDivider != null) {
                    rightDivider.SetActive(false);
                }
                if (bigDivider != null) {
                    bigDivider.SetActive(true);
                }
                break;
        }
    }

    // Update is called once per frame
    void Update() {

    }

    private void OnDestroy() {
        if (PhotonNetwork.connectedAndReady) {
            // Only the owner should try and destroy the bubble
            if (PhotonNetwork.player == GetComponent<PhotonView>().owner) {
                PhotonNetwork.Destroy(gameObject);
            } else if (PhotonNetwork.isMasterClient) {
                GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.masterClient);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

}