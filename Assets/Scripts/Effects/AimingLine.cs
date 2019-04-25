using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimingLine : MonoBehaviour {
    public LayerMask collisionMask1;

    public GameObject _aimDotObj;

    List<AimDot> _aimDots = new List<AimDot>();

    public bool _generating;
    float _generateTime = 0.1f;
    float _generateTimer = 0f;

    Ray2D _ray;
    Vector2 _dir;
    RaycastHit2D _hit;
    Ray2D _ray2;
    Vector2 _dir2;
    RaycastHit2D _hit2;

    // Use this for initialization
    void Start () {
	}
	
    // Update is called once per frame
    void Update () {
        Raycasts();

        if (_generating) {
            _generateTimer += Time.deltaTime;
            if (_generateTimer >= _generateTime) {
                GenerateAimDot();
                _generateTimer = 0f;
            }

            UpdateAimDots();
        }
		SoundManager.mainAudio.ThrowAngleEvent.setParameterValue("LaunchAngle", Mathf.Abs(transform.rotation.z));
    }

    void Raycasts() {
        _dir = transform.GetChild(0).position - transform.position;
        _ray = new Ray2D(transform.position, _dir.normalized);
        //_hit = Physics2D.Raycast(_ray.origin, _ray.direction, 20, collisionMask1);
        _hit = Physics2D.CircleCast(_ray.origin, 0.34f, _ray.direction, 20, collisionMask1);
        Debug.DrawRay(_ray.origin, _ray.direction * _hit.distance, Color.black);

        if (_hit && _hit.collider.tag == "Wall") {
            // Bounce off the wall

            // For some reason the relfected ray is getting stuck on the same collider
            // So disable it before the cast, then enable it after
            //_hit.collider.enabled = false;

            _dir2 = Vector2.Reflect(_dir, _hit.normal);
            _ray2 = new Ray2D(_hit.centroid, _dir2);
            _hit2 = Physics2D.Raycast(_ray2.origin, _ray2.direction, 20, collisionMask1);
            Debug.DrawRay(_ray2.origin, _ray2.direction * _hit2.distance, Color.blue);

            //_hit.collider.enabled = true;
        } else {
            _hit2.distance = 0;
        }
    }

    void GenerateAimDot() {
        GameObject aimDot = Instantiate(_aimDotObj, transform);
        aimDot.transform.localPosition = new Vector3(0.5f, 0f, -1f);
        aimDot.transform.localScale = new Vector3(1f, 1f, 1f);
        _aimDots.Add(aimDot.GetComponent<AimDot>());
    }

    void UpdateAimDots() {
        foreach (AimDot aimDot in _aimDots) {
            if (aimDot == null) {
                continue;
            }

            if (!aimDot.Reflected && aimDot.RayPos > _hit.distance) {
                aimDot.Reflect();
            }

            if (!aimDot.Reflected) {
                aimDot.transform.position = new Vector3(_ray.GetPoint(aimDot.RayPos).x,
                                                        _ray.GetPoint(aimDot.RayPos).y,
                                                        aimDot.transform.position.z);
            } else if (_hit2.distance > 0) {
                aimDot.transform.position = new Vector3(_ray2.GetPoint(aimDot.RayPos).x,
                                                        _ray2.GetPoint(aimDot.RayPos).y,
                                                        aimDot.transform.position.z);
            } else {
                DestroyObject(aimDot.gameObject);
            }
        }
    }

    public void Begin() {
        _generating = true;
    }

    public void Stop() {
        // Destroy all aim dots
        foreach(AimDot aimDot in _aimDots) {
            if (aimDot != null) {
                DestroyObject(aimDot.gameObject);
            }
        }
        _aimDots.Clear();

        _generating = false;
		
    }

    public void RemoveAimDot(AimDot aimDot) {
        _aimDots.Remove(aimDot);
    }
}
