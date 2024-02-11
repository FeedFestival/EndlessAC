using DG.Tweening;
using Game.Shared.Interfaces;
using UnityEngine;

namespace Game.Actors {
    public class Spaceship_Actor : MonoBehaviour, IActor {

        public Animator Animator { get => null; }
        public GameObject go => gameObject;

        [SerializeField]
        private Transform _livingAreaT;
        [SerializeField]
        private GameObject _nuclearSunRoom;
        [SerializeField]
        private Spaceship_Thrusters _engine_Thrusters;
        [SerializeField]
        private Spaceship_Thrusters _livingAreaSmall_Thrusters;
        [SerializeField]
        private Spaceship_Thrusters _livingAreaBig_Thrusters;

        private Tweener _rotateLivingAreaTweener;
        private readonly Vector3 START_ROTATION = new Vector3(0, 90, 0);
        private readonly Vector3 END_ROTATION = new Vector3(0, 90, 360);
        private readonly float ROTATION_TIME = 7.5f;
        private readonly float NUCLEAR_START_TIME = 3.33f;
        private readonly float PULSATING_TIME = 0.33f;
        private Tweener _nuclearSunColorTweener;
        private Color _nuclear_offColor;
        private float _nuclear_emissionValue;
        private const float _nuclear_startEmissionValue = 0.001f;
        private const float _nuclear_endEmissionValue = 0.15f;
        private Color _nuclear_startColor;
        private Color _nuclear_endColor;
        private Material _nuclearMaterial;

        private Vector3? _stopAtRotation;
        private bool _stopRotateLivingArea;

        private void Awake() {
            _nuclearMaterial = _nuclearSunRoom.GetComponent<MeshRenderer>().material;

            _nuclear_offColor = new Color(0, 0, 0);
            _nuclear_startColor = new Color(190, 70, 5);
            _nuclear_endColor = new Color(190, 40, 5);
        }

        void Start() {

            Init();
        }

        public void Init() {
            StopEngine(instant: true);
            StopNuclear(instant: true);
            StopRotationEngine(instant: true);
        }

        public void SetActive(bool active) => gameObject.SetActive(active);

        public void StartRotateLivingArea() {
            _rotateLivingAreaTweener.Kill();
            _rotateLivingAreaTweener = DOTween
                .To(() => START_ROTATION, rotation => _livingAreaT.localEulerAngles = rotation, END_ROTATION, ROTATION_TIME * 2)
                .SetEase(Ease.InCubic)
                .OnComplete(() => {
                    rotateLivingArea();
                });
        }

        public void StopRotateLivingAreaAtTurnRotation() {
            var atTurnRot = new Vector3(0, 90, 270);
            StopRotateLivingArea(atTurnRot);
        }

        public void StopRotateLivingArea() {
            StopRotateLivingArea(null);
        }

        public void StopRotateLivingArea(Vector3? atRotation = null) {
            _stopAtRotation = atRotation;
            if (_rotateLivingAreaTweener.IsPlaying()) {
                _stopRotateLivingArea = true;
            } else {
                rotateLivingAreaToAStop();
            }
        }

        public void StartNuclear() {
            _nuclearSunColorTweener.Kill();

            DOVirtual
                .Float(_nuclear_startEmissionValue, _nuclear_endEmissionValue, NUCLEAR_START_TIME, (value) => {
                    _nuclear_emissionValue = value;
                });

            _nuclearSunColorTweener = DOVirtual
                .Color(_nuclear_offColor, _nuclear_startColor, NUCLEAR_START_TIME, (value) => {
                    _nuclearMaterial.SetColor("_EmissionColor", value * _nuclear_emissionValue);
                })
                .OnComplete(() => {
                    changeNuclearSunColor(false);
                });
        }

        public void StopNuclear(bool instant = false) {
            if (instant) {
                _nuclearMaterial.SetColor("_EmissionColor", _nuclear_offColor = new Color(0, 0, 0));
            } else {
                _nuclearSunColorTweener.Kill();
                _nuclearSunColorTweener = DOVirtual
                    .Color(_nuclear_endColor, _nuclear_offColor, NUCLEAR_START_TIME, (value) => {
                        _nuclearMaterial.SetColor("_EmissionColor", value * .21f);
                    });
            }
        }

        public void StartEngine() => _engine_Thrusters.StartThrusters();

        public void StopEngine(bool instant = false) => _engine_Thrusters.StopThrusters(instant);

        public void StartRotationEngine() {
            _livingAreaSmall_Thrusters.StartThrusters();
            _livingAreaBig_Thrusters.StartThrusters();
        }

        public void StopRotationEngine(bool instant = false) {
            _livingAreaSmall_Thrusters.StopThrusters(instant);
            _livingAreaBig_Thrusters.StopThrusters(instant);
        }










        private void rotateLivingArea() {
            _rotateLivingAreaTweener = DOTween
                .To(() => START_ROTATION, rotation => _livingAreaT.localEulerAngles = rotation, END_ROTATION, ROTATION_TIME)
                .SetEase(Ease.Linear)
                .OnComplete(() => {
                    if (_stopRotateLivingArea) {
                        rotateLivingAreaToAStop();
                    } else {
                        rotateLivingArea();
                    }
                });
        }

        private void rotateLivingAreaToAStop() {
            _stopRotateLivingArea = false;
            var endRotation = _stopAtRotation.HasValue ? _stopAtRotation.Value : END_ROTATION;
            _rotateLivingAreaTweener = DOTween
                .To(() => _livingAreaT.localEulerAngles, rotation => _livingAreaT.localEulerAngles = rotation, endRotation, ROTATION_TIME * 2)
                .SetEase(Ease.OutCubic);
        }

        private void changeNuclearSunColor(bool odd) {
            var start = odd ? _nuclear_startColor : _nuclear_endColor;
            var end = odd ? _nuclear_endColor : _nuclear_startColor;

            _nuclearSunColorTweener = DOVirtual
                .Color(start, end, PULSATING_TIME, (value) => {
                    _nuclearMaterial.SetColor("_EmissionColor", value * .21f);
                })
                .OnComplete(() => {
                    changeNuclearSunColor(!odd);
                });
        }
    }
}
