using UnityEngine;
using System.Collections;

public class ShiftPortal : MonoBehaviour {
    Animator _animator;
    Animator _exitPortal;
    PlayerController _playerController;

    void Awake() {
        _animator = GetComponent<Animator>();
        _exitPortal = transform.GetChild(0).GetComponent<Animator>();
        _playerController = transform.parent.GetComponent<PlayerController>();
    }

    // Use this for initialization
    void Start () {
        _exitPortal.transform.parent = null;
    }

    // Update is called once per frame
    void Update () {
	
	}

    public void Activate(int dir) {
        // Setup portal on player and on shift location
        gameObject.SetActive(true);

        // Detach from the parent to prevent transformations
        transform.parent = null;
        _exitPortal.gameObject.SetActive(true);


        // TODO: Do whichever shift is appropriate for the current stage
        //_exitPortal.transform.Translate(12.5f * dir, 0f, 0f, Space.World);
        float shiftDistance = Mathf.Abs(_playerController.transform.position.x) * 2;
        _exitPortal.transform.position = transform.position;
        _exitPortal.transform.Translate(shiftDistance * dir, 0f, 0f, Space.World);

        // Animate both portals
        //_animator.Play("Portal_CW");
        //_exitPortal.Play("Portal_CCW");
        _animator.Play("PortalSpinCCW");
        _exitPortal.Play("PortalSpinCW");
    }

    public void Deactivate() {
        // Reattach to parent
        transform.parent = _playerController.transform;

        transform.localPosition = new Vector3(0, 0, 1);
        _exitPortal.transform.localPosition = new Vector3(0, 0, 1);

        // Stop animating both portals
        _animator.Play("Portal_Idle");
        _exitPortal.Play("Portal_Idle");
        //_animator.Stop();
        //_exitPortal.Stop();

        // Deactivate objects
        gameObject.SetActive(false);
        _exitPortal.gameObject.SetActive(false);
    }
}
