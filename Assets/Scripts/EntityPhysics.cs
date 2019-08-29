using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(Collider2D))]
public class EntityPhysics : MonoBehaviour {
    public LayerMask collisionMask1;
    public LayerMask collisionMask2;
    public float wallCheckDist;

    private Entity entity;
    private Vector2 _pos;
    private Collider2D _collider;
    private float _radius;
    private float _scaledRadius;
    private float _scaledRadiusX;
    private float _scaledRadiusY;
    private Vector2 _offset;

    private float _skin = 0.005f;

    private Ray2D _ray;
    private RaycastHit2D _hit;

    private float _stuckTimer = 0;
    private float _stuckTime = 0.5f;

    private int ceilingHitCount; // this is a counter for how much of the top of the player is colliding
    private bool isTouchingFloor;
    private int floorHitCount; // this is a counter for how much of the top of the player is colliding
    private bool isTouchingWallLeft;
    private int leftHitCount; // this is a counter for how much of the top of the player is colliding
    private bool isTouchingWallRight;
    private int rightHitCount; // this is a counter for how much of the top of the player is colliding

    public bool IsTouchingFloor {
        get { return isTouchingFloor; }
    }
    public bool IsTouchingWallLeft {
        get { return isTouchingWallLeft; }
    }
    public bool IsTouchingWallRight {
        get { return isTouchingWallRight; }
    }

    void Start() {
        entity = GetComponent<Entity>();

        UpdateCollisionData();

        wallCheckDist = 0.1f;
    }

    // Gets the collision data from the entity
    public void UpdateCollisionData() {
        _collider = GetComponent<CircleCollider2D>();
        if (_collider != null) {
            _radius = ((CircleCollider2D)_collider).radius;
            _scaledRadius = Mathf.Abs(_radius * transform.localScale.x);
            _scaledRadiusX = _scaledRadius;
            _scaledRadiusY = _scaledRadius;
        } else {
            _collider = GetComponent<BoxCollider2D>();
            _scaledRadiusX = Mathf.Abs(((BoxCollider2D)_collider).size.x / 2.2f * transform.lossyScale.x);
            _scaledRadiusY = Mathf.Abs(((BoxCollider2D)_collider).size.y / 2f * transform.lossyScale.y);
        }

        _offset = _collider.offset;
    }

    void Update() {
        CheckBelow();
        WallCheck();
    }

    public void MoveX(float deltaX) {
        if (deltaX == 0) {
            return;
        }

        for (int i = 0; i < 5; ++i) {
            float dir = Mathf.Sign(deltaX);
            float x = _pos.x + _offset.x + _scaledRadiusX * dir;
            float y = (_pos.y + _offset.y - _scaledRadiusY/1.1f) + _scaledRadiusY / 2.1f * i;

            _ray = new Ray2D(new Vector2(x, y), Vector2.right * dir);
            Debug.DrawRay(_ray.origin, _ray.direction * Mathf.Abs(deltaX));
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, Mathf.Abs(deltaX), collisionMask1);
            if (_hit) {
                float dst = Vector2.Distance(_ray.origin, _hit.point);

                if (dst > _skin) {
                    deltaX = dst * dir + _skin;
                } else {
                    deltaX = 0;
                }

                entity.CollisionResponseX(_hit.collider);
                break;
            }
        }

        transform.Translate(deltaX, 0.0f, 0.0f);
    }

    public void MoveY(float inDeltaY) {
        if (inDeltaY == 0) {
            return;
        }

        ceilingHitCount = 0;
        floorHitCount = 0;
        float deltaY = inDeltaY;
        _pos = transform.position;
        isTouchingFloor = false;
        float offsetY;

        // If we are moving upward
        if (inDeltaY > 0) {
            collisionMask1 = collisionMask1 & ~(1 << 18); // Remove the "Passthrough" layer from the mask
            offsetY = 0;
        } else {
            collisionMask1 = collisionMask1 | (1 << 18); // Add the "Passthrough" layer to the mask
            offsetY = _offset.y;
        }

        for (int i = 0; i < 3; ++i) {
            float dir = Mathf.Sign(inDeltaY);
            float x = (_pos.x + _offset.x - _scaledRadiusX/1.1f) + (_scaledRadiusX/1.1f) * i;
            float y = _pos.y + offsetY + _scaledRadiusY * dir;

            _ray = new Ray2D(new Vector2(x, y), Vector2.up * dir);
            Debug.DrawRay(_ray.origin, _ray.direction * Mathf.Abs(inDeltaY));
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, Mathf.Abs(inDeltaY), collisionMask1);
            if (_hit) {
                if (dir == 1) {
                    ceilingHitCount++;
                } else if (dir == -1) {
                    floorHitCount++;
                }
                if (ceilingHitCount > 1 || floorHitCount > 1) {
                    continue;
                }

                float dst = Vector2.Distance(_ray.origin, _hit.point);

                if (dst > _skin) {
                    deltaY = dst * dir + _skin;
                } else {
                    deltaY = 0;
                }

                isTouchingFloor = true;
                entity.Grounded = true;
                entity.CollisionResponseY(_hit.collider);
            }
        }

        transform.Translate(0.0f, deltaY, 0.0f);
    }

    // Checks if there is anything below the player
    public void CheckBelow() {
        _pos = transform.position;
        isTouchingFloor = false;
        //floorHitCount = 0;

        for (int i = 0; i < 3; ++i) {
            float x = (_pos.x + _offset.x - _scaledRadiusX / 1.1f) + _scaledRadiusX / 1.1f * i;
            float y = _pos.y + _offset.y + _scaledRadiusY * -1;

            _ray = new Ray2D(new Vector2(x, y), Vector2.up * -1);
            //Debug.DrawRay(_ray.origin, _ray.direction * 0.01f);
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, 0.01f, collisionMask1);
            if (_hit) {
                isTouchingFloor = true;
                entity.Grounded = true;
                //floorHitCount++;
                entity.CollisionResponseY(_hit.collider);
            }
        }
    }

    public void WallCheck() {
        _pos = transform.position;

        isTouchingWallLeft = false;
        isTouchingWallRight = false;
        leftHitCount = 0;
        rightHitCount = 0;

        // Check right first
        for (int i = 0; i < 3; ++i) {
            float x = _pos.x + _offset.x + _scaledRadiusX;
            float y = (_pos.y + _offset.y - _scaledRadiusY / 1.25f) + _scaledRadiusY / 1.25f * i;

            _ray = new Ray2D(new Vector2(x, y), Vector2.right);
            ///Debug.DrawRay(_ray.origin, _ray.direction * wallCheckDist);
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, wallCheckDist, collisionMask2);
            if (_hit) {
                rightHitCount++;
                isTouchingWallRight = true;
            }
        }

        // Check left second
        for (int i = 0; i < 3; ++i) {
            float x = _pos.x + _offset.x + _scaledRadiusX * -1;
            float y = (_pos.y + _offset.y - _scaledRadiusY / 1.25f) + _scaledRadiusY / 1.25f * i;

            _ray = new Ray2D(new Vector2(x, y), Vector2.right * -1);
            //Debug.DrawRay(_ray.origin, _ray.direction * wallCheckDist);
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, wallCheckDist, collisionMask2);
            if (_hit) {
                isTouchingWallLeft = true;
                leftHitCount++;
            }
        }
    }

    public bool IsTouchingAnything() {
        if (isTouchingFloor || isTouchingWallLeft || isTouchingWallRight) {
            return true;
        }

        return false;
    }

    void OnTriggerStay2D(Collider2D collider) {
        // If we are in a wall (layer 9) or platform (layer 21) with a box collider
        if ((collider.gameObject.layer == 9 || collider.gameObject.layer == 21) && collider.GetComponent<BoxCollider2D>() != null) {
            //_stuckTimer += Time.deltaTime;
            //if(_stuckTimer < _stuckTime) {
            //    return;
            //}

            // Figure out from which direction we are colliding
            int tempLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            int dir = -1;
            int hitCount = -1;
            int sideCount = 0;

            // Bottom check
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(_pos.x, _pos.y + _offset.y - _scaledRadiusY), new Vector2(0, -1), 0.1f);
            //Debug.DrawRay(new Vector2(_pos.x, _pos.y + _center.y - _scaledRadius), new Vector2(0, -1));
            if (hit && hit.collider == collider && hitCount <= floorHitCount) {
                dir = 2;
                hitCount = floorHitCount;
                sideCount++;
            }
            // Top check
            hit = Physics2D.Raycast(new Vector2(_pos.x, _pos.y + _offset.y + _scaledRadiusY), new Vector2(0, 1), 0.1f);
            //Debug.DrawRay(new Vector2(_pos.x, _pos.y + _center.y + _scaledRadius), new Vector2(0, 1));
            if (hit && hit.collider == collider && hitCount <= ceilingHitCount) {
                dir = 3;
                hitCount = ceilingHitCount;
                sideCount++;
            }
            // Right side check
            hit = Physics2D.Raycast(new Vector2(_pos.x + _offset.x + _scaledRadiusX, _pos.y), new Vector2(1, 0), 0.1f);
            //Debug.DrawRay(new Vector2(_pos.x + _center.x + _scaledRadius, _pos.y), new Vector2(1, 0));
            if(hit && hit.collider == collider && hitCount <= rightHitCount) {
                dir = 0;
                hitCount = rightHitCount;
                sideCount++;
            }
            // left side check
            hit = Physics2D.Raycast(new Vector2(_pos.x + _offset.x - _scaledRadiusX, _pos.y), new Vector2(-1, 0), 0.1f);
            //Debug.DrawRay(new Vector2(_pos.x + _center.x - _scaledRadius, _pos.y), new Vector2(-1, 0));
            if (hit && hit.collider == collider && hitCount <= leftHitCount) {
                dir = 1;
                hitCount = leftHitCount;
                sideCount++;
            }

            gameObject.layer = tempLayer;

            if (sideCount >= 3) {
                // The entity is probably completely inside a wall and needs to be respawned
                entity.Respawn();
               
                // TODO: instead of respawning, attempt to push out in the direction of least resistance
            } else {
                // Move out of it
                switch (dir) {
                    case 0:
                        float curXPos = transform.position.x + _scaledRadiusX;
                        float wantXPos = collider.transform.position.x - (collider.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(collider.transform.lossyScale.x)) / 2f;
                        float xMove = wantXPos - curXPos;
                        if (Mathf.Abs(xMove) > _scaledRadiusX / 4) {
                            MoveX(xMove);
                        }
                        break;
                    case 1:
                        curXPos = transform.position.x - _scaledRadiusX;
                        wantXPos = collider.transform.position.x + (collider.GetComponent<BoxCollider2D>().size.x * Mathf.Abs(collider.transform.lossyScale.x)) / 2f;
                        xMove = wantXPos - curXPos;
                        if (Mathf.Abs(xMove) > _scaledRadiusX / 4) {
                            MoveX(xMove);
                        }
                        break;
                    case 2:
                        float curYPos = transform.position.y - _scaledRadiusY;
                        float wantYPos = collider.transform.position.y + (collider.GetComponent<BoxCollider2D>().size.y * Mathf.Abs(collider.transform.lossyScale.y)) / 2f;
                        float yMove = wantYPos - curYPos;
                        if (Mathf.Abs(yMove) > _scaledRadiusY / 4) {
                            MoveY(yMove);
                        }
                        break;
                    case 3:
                        curYPos = transform.position.y + _scaledRadiusY;
                        wantYPos = collider.transform.position.y - (collider.GetComponent<BoxCollider2D>().size.y * Mathf.Abs(collider.transform.lossyScale.y)) / 2f;
                        yMove = wantYPos - curYPos;
                        if (Mathf.Abs(yMove) > _scaledRadiusY / 4) {
                            MoveY(yMove);
                        }
                        break;
                }
            }

            _stuckTimer = 0f;
        }
    }
}
