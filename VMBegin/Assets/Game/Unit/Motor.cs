using DG.Tweening;
using Game.Shared.Interfaces;
using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Unit {
    public class Motor : MonoBehaviour {

        NavMeshAgent _agent;
        [SerializeField]
        float _agentSpeed = 3.5f;
        [SerializeField]
        float _speedMultiplier = 1f;
        [Tooltip("Acceleration and deceleration")]
        [SerializeField]
        float _speedChangeRate = 10.0f;
        [SerializeField]
        float _sprintMultiplier = 2f;
        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        [SerializeField]
        float _rotationSmoothTime = 0.12f;


        float _targetRotation = 0.0f;
        float _rotationVelocity;


        Vector2 _movementInput;
        float _cameraRotationY = 0f;
        bool _analogMovement;
        bool _sprint = false;


        ITrigger _movementTargetTrigger;
        bool _movingToTarget = false;

        IDisposable _motorLateUpdateObs;
        IDisposable _agentOffMeshLinkObs;
        Tween _speedTween;
        float _animatorSpeed;
        float _animationBlend;
        Tweener _stopMovementTweener;

        Tween _rotateTween;
        Vector3? _nextCornerPos;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDMotionSpeed;

        bool _isDisabled;
        IActor _actor;

        internal void Init(IActor actor, ITrigger movementTargetTrigger) {
            _actor = actor;

            _agent = GetComponent<NavMeshAgent>();
            _agent.updateRotation = false;

            // We do this so we can make sure the agent is on the navmesh
            Teleport(transform.position);
            _agent.enabled = true;

            _movementTargetTrigger = movementTargetTrigger;

            assignAnimationIDs();
        }

        internal void Teleport(Vector3 position, bool onNavMesh = false) {
            if (onNavMesh) {

                NavMeshHit closestHit;
                if (NavMesh.SamplePosition(position, out closestHit, 500f, NavMesh.AllAreas)) {
                    transform.position = closestHit.position;
                } else {
                    Debug.LogError("Could not find position on NavMesh!");
                }
                return;
            }
            transform.position = position;
        }

        internal void SprintTogled(bool sprint) {
            _sprint = sprint;
        }

        internal void MovementTargetChanged(Vector3 targetPos) {
            _movingToTarget = true;
            _agent.speed = _agentSpeed;

            stayOnNavMesh(ref targetPos);
            moveToTarget(ref targetPos);
        }

        internal void MoveTargetReached() {
            _motorLateUpdateObs?.Dispose();
            _agent.isStopped = true;

            changeSpeed(0, 0.3f);

            //ReachedDestination?.Invoke();
        }

        internal void MovementInputChanged(Vector2 input, float cameraRotationY, bool analogMovement) {

            if (_movingToTarget) {
                _movingToTarget = false;
                MoveTargetReached();
            }

            if (analogMovement == false) {
                _agent.speed = _agentSpeed;
            }

            _movementInput = input;
            _cameraRotationY = cameraRotationY;
            _analogMovement = analogMovement;

            if (_movementInput == Vector2.zero) {
                tweenStopMovement();
            } else {
                _stopMovementTweener.Kill();
            }
        }

        void Update() {

            if (_isDisabled || _movingToTarget || _movementInput == Vector2.zero) { return; }

            move();
        }

        void stayOnNavMesh(ref Vector3 pos) {
            NavMeshHit hit;
            bool canWalk = NavMesh.SamplePosition(pos, out hit, 3, NavMesh.AllAreas);
            if (canWalk) {
                pos = hit.position;
            } else {
                if (NavMesh.FindClosestEdge(pos, out hit, NavMesh.AllAreas)) {
                    pos = hit.position;
                }
            }
        }

        void moveToTarget(ref Vector3 pos) {

            float distance = Vector3.Distance(pos, transform.position);
            if (distance < 0.7f) { return; }

            if (_agent.isStopped) {
                _agent.isStopped = false;
            }

            _movementTargetTrigger.transform.position = pos;
            _movementTargetTrigger.Enable();
            _agent.SetDestination(pos);

            //if (_usingOtherAnimator == false) {
            _motorLateUpdateObs?.Dispose();
            _motorLateUpdateObs = this.LateUpdateAsObservable()
                .ThrottleFirst(TimeSpan.FromMilliseconds(100))
                .Do(_ => changeSpeed(_agent.velocity.magnitude, 0.1f))
                .Do(_ => rotateTowardsNextPointOnNavMesh())
                .Do(_ => checkIfReachedDestination())
                .Subscribe();
            //}
        }

        void move() {

            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = _sprint ? _sprintMultiplier : _speedMultiplier;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            //if (_movementInput == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_agent.velocity.x, 0.0f, _agent.velocity.z).magnitude;

            float speed = targetSpeed;
            float speedOffset = 0.1f;
            float inputMagnitude = _analogMovement ? _movementInput.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset) {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * _speedChangeRate);

                // round speed to 3 decimal places
                speed = Mathf.Round(speed * 1000f) / 1000f;
            }

            _agent.speed = speed;

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * _speedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            var inputDirection = new Vector3(_movementInput.x, 0.0f, _movementInput.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_movementInput != Vector2.zero) {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _cameraRotationY;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    _rotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            if (inputDirection != Vector3.zero) {
                var targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
                _agent.Move(targetDirection * _agent.speed * Time.deltaTime);
            }

            setAnimatorFloats(_animationBlend, inputMagnitude);
        }

        void changeSpeed(float toSpeed, float time) {
            killSpeedTweenIfRunning();

            //if (_usingOtherAnimator) { return; }

            _speedTween = DOTween
            .To(() => _animatorSpeed,
                speed => _animatorSpeed = speed,
                toSpeed,
                time
            )
            .SetEase(Ease.Linear)
            .OnUpdate(() => {
                _actor.Animator.SetFloat("Speed", _animatorSpeed);
            })
            .OnKill(() => { _speedTween = null; });
        }

        void rotateTowardsNextPointOnNavMesh() {
            //if (_usingOtherAnimator) { return; }

            Vector3? nextCornerPos;

            try {
                nextCornerPos = _agent.path.corners[1];
            } catch (Exception) {
#if UNITY_EDITOR
                var s = string.Empty;
                foreach (var item in _agent.path.corners) {
                    s += item + "\n";
                }
                //Debug.Log("_agent.path.corners: " + s);
#endif
                nextCornerPos = null;
            }

            Debug.Log("nextCornerPos: " + nextCornerPos);

            if (nextCornerPos.HasValue == false || _nextCornerPos == nextCornerPos) return;

            _nextCornerPos = nextCornerPos;
            Debug.Log("_nextCornerPos: " + _nextCornerPos);

            var lookAt = Quaternion.LookRotation(nextCornerPos.Value - transform.position);

            _rotateTween?.Kill();

            _rotateTween = DOVirtual
            .Float(0, 1, 0.33f, (value) => {
                transform.rotation = Quaternion.Slerp(transform.rotation, lookAt, value);
            });
        }

        void checkIfReachedDestination() {
            //if (_usingOtherAnimator) { return; }

            if (!_agent.pathPending) {
                if (_agent.remainingDistance <= _agent.stoppingDistance) {
                    if (!_agent.hasPath || _agent.velocity.sqrMagnitude == 0f) {
                        MoveTargetReached();
                    }
                }
            }
        }

        void killSpeedTweenIfRunning() {
            if (_speedTween != null && _speedTween.IsActive()) {
                _speedTween.Kill();
                _speedTween = null;
            }
        }

        void tweenStopMovement() {
            _stopMovementTweener.Kill();
            _stopMovementTweener = DOVirtual.Float(_animationBlend, 0, 0.3f, (value) => {
                _animationBlend = value;
                setAnimatorFloats(speed: _animationBlend);
            });
        }

        void assignAnimationIDs() {
            // This could be the reason Miss Johnson freezes after we switch animator , the hash changes?
            _animIDSpeed = Animator.StringToHash("Speed");
            //_animIDGrounded = Animator.StringToHash("Grounded");
            //_animIDJump = Animator.StringToHash("Jump");
            //_animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        void setAnimatorFloats(float speed, float motionSpeed = 1) {
            //if (_hasAnimator) {
            _actor.Animator.SetFloat(_animIDSpeed, speed);
            _actor.Animator.SetFloat(_animIDMotionSpeed, motionSpeed);
            //}
        }

        void OnDestroy() {
            killSpeedTweenIfRunning();
            _motorLateUpdateObs?.Dispose();
            _agentOffMeshLinkObs?.Dispose();
        }
    }
}