using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(Collider2D))]
public class EntityPhysics : MonoBehaviour {
    public LayerMask collisionMaskX;
    public LayerMask collisionMaskY;
    public LayerMask collisionMaskSlope;
    public float wallCheckDist;

    private Entity entity;
    private Collider2D _myCollider;

    private float _skin = 0.005f;

    private Ray2D _ray;
    private RaycastHit2D _hit;

    private float _stuckTimer = 0;
    private float _stuckTime = 0.25f;

    private int ceilingHitCount; // this is a counter for how much of the top of the player is colliding
    private bool isTouchingFloor;
    private int floorHitCount; // this is a counter for how much of the top of the player is colliding
    private bool isTouchingWallLeft;
    private int leftHitCount; // this is a counter for how much of the top of the player is colliding
    private bool isTouchingWallRight;
    private int rightHitCount; // this is a counter for how much of the top of the player is colliding

    public bool snappedToSlope;

    public bool IsTouchingFloor {
        get { return isTouchingFloor; }
    }
    public bool IsTouchingWallLeft {
        get { return isTouchingWallLeft; }
    }
    public bool IsTouchingWallRight {
        get { return isTouchingWallRight; }
    }

    private void Awake() {
        _myCollider = GetComponent<CircleCollider2D>();
        if (_myCollider == null) {
            _myCollider = GetComponent<BoxCollider2D>();
        }
    }
    void Start() {
        entity = GetComponent<Entity>();

        wallCheckDist = 0.1f;
    }

    void Update() {
    }

    private void FixedUpdate() {
        CheckBelow();
        WallCheck();
    }

    public void MoveX(float deltaX) {
        if (deltaX == 0) {
            return;
        }

        for (int i = 0; i < 10; ++i) {
            float dir = Mathf.Sign(deltaX);
            //float x = _pos.x + _offset.x + _scaledRadiusX * dir;
            float x = _myCollider.bounds.center.x + _myCollider.bounds.extents.x*dir;
            //float y = (_pos.y + _offset.y - _scaledRadiusY/1.1f) + _scaledRadiusY / 2.1f * i;
            float y = (_myCollider.bounds.center.y - _myCollider.bounds.extents.y / 1.05f) + _myCollider.bounds.extents.y / 4.7f * i;

            _ray = new Ray2D(new Vector2(x, y), Vector2.right * dir);
            // Draw last so it matches position
            //Debug.DrawRay(new Vector2(_ray.origin.x+deltaX, _ray.origin.y), _ray.direction * Mathf.Abs(deltaX));
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, Mathf.Abs(deltaX), collisionMaskX);
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

        if (!snappedToSlope) {
            isTouchingFloor = false;
        }

        // If we are moving upward
        if (inDeltaY > 0) {
            collisionMaskY = collisionMaskY & ~(1 << 18); // Remove the "Passthrough" layer from the mask
        } else {
            collisionMaskY = collisionMaskY | (1 << 18); // Add the "Passthrough" layer to the mask
        }

        for (int i = 0; i < 3; ++i) {
            float dir = Mathf.Sign(inDeltaY);
            //float x = (_pos.x + _offset.x - _scaledRadiusX/1.1f) + (_scaledRadiusX/1.1f) * i;
            float x = (_myCollider.bounds.center.x - _myCollider.bounds.extents.x / 1.2f) + _myCollider.bounds.extents.x / 1.2f * i;

            //float y = _pos.y + offsetY + _scaledRadiusY * dir;
            float y = _myCollider.bounds.center.y + _myCollider.bounds.extents.y*dir;

            _ray = new Ray2D(new Vector2(x, y), Vector2.up * dir);
            // Add the delta so it lines up with where the player will move
            //Debug.DrawLine(new Vector3(_ray.origin.x - 0.1f, _ray.origin.y + deltaY), new Vector3(_ray.origin.x + 0.1f, _ray.origin.y + deltaY), Color.green);
            //Debug.DrawRay(new Vector2(_ray.origin.x, _ray.origin.y + deltaY), _ray.direction * Mathf.Abs(inDeltaY));
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, Mathf.Abs(inDeltaY), collisionMaskY);
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

        if (!snappedToSlope) {
            transform.Translate(0.0f, deltaY, 0.0f);
        }
    }

    // Checks if there is anything below the player
    public void CheckBelow() {
        if(snappedToSlope) {
            return;
        }

        isTouchingFloor = false;
        //floorHitCount = 0;

        for (int i = 0; i < 3; ++i) {
            //float x = (_pos.x + _offset.x - _scaledRadiusX / 1.1f) + _scaledRadiusX / 1.1f * i;
            float x = (_myCollider.bounds.center.x - _myCollider.bounds.extents.x / 1.2f) + _myCollider.bounds.extents.x / 1.2f * i;
            //float y = _pos.y + _offset.y + _scaledRadiusY * -1;
            float y = _myCollider.bounds.center.y - _myCollider.bounds.extents.y;

            _ray = new Ray2D(new Vector2(x, y), Vector2.up * -1);
            Debug.DrawRay(_ray.origin, _ray.direction * 0.01f);
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, 0.05f, collisionMaskY);
            if (_hit) {
                isTouchingFloor = true;
                entity.Grounded = true;
                //floorHitCount++;
                //entity.CollisionResponseY(_hit.collider);
            }
        }
    }

    public void WallCheck() {
        isTouchingWallLeft = false;
        isTouchingWallRight = false;
        leftHitCount = 0;
        rightHitCount = 0;

        // Check right first
        for (int i = 0; i < 3; ++i) {
            //float x = _pos.x + _offset.x + _scaledRadiusX;
            float x = _myCollider.bounds.center.x + _myCollider.bounds.extents.x;
            //float y = (_pos.y + _offset.y - _scaledRadiusY / 1.25f) + _scaledRadiusY / 1.25f * i;
            float y = (_myCollider.bounds.center.y - _myCollider.bounds.extents.y / 1.05f) + _myCollider.bounds.extents.y / 2.2f * i;

            _ray = new Ray2D(new Vector2(x, y), Vector2.right);
            //Debug.DrawRay(_ray.origin, _ray.direction * wallCheckDist);
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, wallCheckDist, collisionMaskX);
            if (_hit) {
                rightHitCount++;
                isTouchingWallRight = true;
            }
        }

        // Check left second
        for (int i = 0; i < 3; ++i) {
            //float x = _pos.x + _offset.x + _scaledRadiusX * -1;
            float x = _myCollider.bounds.center.x - _myCollider.bounds.extents.x;
            //float y = (_pos.y + _offset.y - _scaledRadiusY / 1.25f) + _scaledRadiusY / 1.25f * i;
            float y = (_myCollider.bounds.center.y - _myCollider.bounds.extents.y / 1.05f) + _myCollider.bounds.extents.y / 2.2f * i;

            _ray = new Ray2D(new Vector2(x, y), Vector2.right * -1);
            //Debug.DrawRay(_ray.origin, _ray.direction * wallCheckDist);
            _hit = Physics2D.Raycast(_ray.origin, _ray.direction, wallCheckDist, collisionMaskX);
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
        if ((collider.gameObject.layer == 9 || collider.gameObject.layer == 21 || collider.gameObject.layer == 23) && collider.GetComponent<BoxCollider2D>() != null) {
            _stuckTimer += Time.deltaTime;
            if(_stuckTimer < _stuckTime) {
                return;
            }

            // Figure out from which direction we are colliding
            int tempLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            // Only search for the collided object
            int layerMask = 1 << collider.gameObject.layer;

            int dir = -1;
            int hitCount = -1;
            int sideCount = 0;

            // Bottom check
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(_myCollider.bounds.center.x, _myCollider.bounds.center.y - _myCollider.bounds.extents.y), new Vector2(0, -1), 0.1f, layerMask);
            //Debug.DrawRay(new Vector2(_myCollider.bounds.center.x, _myCollider.bounds.center.y - _myCollider.bounds.extents.y), new Vector2(0, -1), Color.red);
            if (hit && hit.collider == collider && hitCount <= floorHitCount) {
                dir = 2;
                hitCount = floorHitCount;
                sideCount++;
            }
            // Top check
            hit = Physics2D.Raycast(new Vector2(_myCollider.bounds.center.x, _myCollider.bounds.center.y + _myCollider.bounds.extents.y), new Vector2(0, 1), 0.1f, layerMask);
            //Debug.DrawRay(new Vector2(_myCollider.bounds.center.x, _myCollider.bounds.center.y + _myCollider.bounds.extents.y), new Vector2(0, 1), Color.red);
            if (hit && hit.collider == collider && hitCount <= ceilingHitCount) {
                dir = 3;
                hitCount = ceilingHitCount;
                sideCount++;
            }
            // Right side check
            hit = Physics2D.Raycast(new Vector2(_myCollider.bounds.center.x + _myCollider.bounds.extents.x, _myCollider.bounds.center.y), new Vector2(1, 0), 0.1f, layerMask);
            //Debug.DrawRay(new Vector2(_myCollider.bounds.center.x + _myCollider.bounds.extents.x, _myCollider.bounds.center.y), new Vector2(1, 0), Color.red);
            if(hit && hit.collider == collider && hitCount <= rightHitCount) {
                dir = 0;
                hitCount = rightHitCount;
                sideCount++;
            }
            // left side check
            hit = Physics2D.Raycast(new Vector2(_myCollider.bounds.center.x - _myCollider.bounds.extents.x, _myCollider.bounds.center.y), new Vector2(-1, 0), 0.1f, layerMask);
            //Debug.DrawRay(new Vector2(_myCollider.bounds.center.x - _myCollider.bounds.extents.x, _myCollider.bounds.center.y), new Vector2(-1, 0), Color.red);
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
                    case 0: // Right
                        float curXPos = _myCollider.bounds.center.x + _myCollider.bounds.extents.x;
                        float wantXPos = collider.bounds.center.x - collider.bounds.extents.x;
                        float xMove = wantXPos - curXPos;
                        //if (Mathf.Abs(xMove) > _scaledRadiusX / 4) {
                            MoveX(xMove);
                        //}
                        break;
                    case 1: // Left
                        curXPos = _myCollider.bounds.center.x - _myCollider.bounds.extents.x;
                        wantXPos = collider.bounds.center.x + collider.bounds.extents.x;
                        xMove = wantXPos - curXPos;
                        //if (Mathf.Abs(xMove) > _scaledRadiusX / 4) {
                            MoveX(xMove);
                        //}
                        break;
                    case 2: // Below
                        float curYPos = _myCollider.bounds.center.y - _myCollider.bounds.extents.y;
                        float wantYPos = collider.bounds.center.y + collider.bounds.extents.y;
                        float yMove = wantYPos - curYPos;
                        //if (Mathf.Abs(yMove) > _scaledRadiusY / 4) {
                            MoveY(yMove);
                        //}
                        break;
                    case 3: // Above
                        curYPos = _myCollider.bounds.center.y + _myCollider.bounds.extents.y;
                        wantYPos = collider.bounds.center.y - collider.bounds.extents.y;
                        yMove = wantYPos - curYPos;
                        //if (Mathf.Abs(yMove) > _scaledRadiusY / 4) {
                            MoveY(yMove);
                        //}
                        break;
                }
            }

            _stuckTimer = 0f;
        }
    }

    public void SnapToSlope() {
        snappedToSlope = false;
        float curYPos = _myCollider.bounds.center.y - _myCollider.bounds.extents.y;

        _ray = new Ray2D(new Vector2(_myCollider.bounds.center.x, transform.position.y), Vector2.up * -1);
        Debug.DrawRay(_ray.origin, _ray.direction * 1f);
        _hit = Physics2D.Raycast(_ray.origin, _ray.direction, 1f, collisionMaskSlope);
        if(_hit && _hit.distance-_myCollider.bounds.extents.y < 0.3f) {
            //Debug.Log("Snap to Slope");

            isTouchingFloor = true;
            entity.Grounded = true;
            snappedToSlope = true;

            float wantYPos = _hit.point.y+0.02f;
            float yMove = wantYPos - curYPos;
            transform.Translate(0.0f, yMove, 0.0f);
            entity.CollisionResponseY(_hit.collider);
        }
    }
}
