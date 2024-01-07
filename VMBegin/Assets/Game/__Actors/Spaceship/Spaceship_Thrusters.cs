using DG.Tweening;
using UnityEngine;

namespace Game.Actors {
    public class Spaceship_Thrusters : MonoBehaviour {
        [SerializeField]
        private ParticleSystem _mainThrustersPS;

        [SerializeField]
        private Transform[] _rocketFlamesT;

        [SerializeField]
        private float _rocketFlameMin;
        [SerializeField]
        private float _rocketFlameMax;

        [SerializeField]
        private Vector3 _rocketFlamesOffsetInitialRotation;
        private Tweener _rotateFlameTweener;
        private Vector3 _rocketFlameRotationStart;
        private Vector3 _rocketFlameRotationEnd;
        private Tweener _scaleFlameTweener;

        private readonly float START_THRUSTERS_TIME = 1.5f;
        private readonly Vector3 JET_FLAME_SIZE_END = Vector3.one * 1.2f;

        private readonly float PULSATING_THRUSTERS_TIME_START = 0.07f;
        private readonly float PULSATING_THRUSTERS_TIME_END = 0.23f;

        private Tweener _thrusterIntensityTweener;
        private Color _colorThrusters;
        private float _startIntensityThrusters = 0.0125f;
        private float _endIntensityThrusters = 0.0275f;
        private Material _thrustersMaterial;

        private void Awake() {
            _thrustersMaterial = GetComponent<MeshRenderer>().material;

            _colorThrusters = new Color(130, 10, 0);

            _rocketFlameRotationStart = new Vector3(0, _rocketFlamesOffsetInitialRotation.y, _rocketFlamesOffsetInitialRotation.z);
            _rocketFlameRotationEnd = new Vector3(359, _rocketFlamesOffsetInitialRotation.y, _rocketFlamesOffsetInitialRotation.z);
        }

        public void StartThrusters() {

            DOVirtual
                .Float(0, _endIntensityThrusters, START_THRUSTERS_TIME, (value) => {
                    _thrustersMaterial.SetColor("_EmissionColor", _colorThrusters * value);
                });

            rotateFlame();

            DOVirtual
                .Vector3(Vector3.zero, JET_FLAME_SIZE_END, START_THRUSTERS_TIME, (value) => {
                    foreach (var rocketFlame in _rocketFlamesT) {
                        rocketFlame.localScale = value;
                    }
                })
                .OnComplete(() => {
                    _mainThrustersPS?.Play();
                    scaleFlame(false);
                    changeThrustersIntensity(false);
                });
        }

        public void StopThrusters(bool instant = false) {

            _mainThrustersPS?.Stop();

            if (instant) {

                _thrustersMaterial.SetColor("_EmissionColor", new Color(0, 0, 0));
                foreach (var rocketFlame in _rocketFlamesT) {
                    rocketFlame.localScale = Vector3.zero;
                }
                _rotateFlameTweener.Kill();
                _scaleFlameTweener.Kill();
            } else {

                _thrusterIntensityTweener.Kill();
                DOVirtual
                    .Float(_endIntensityThrusters, 0, START_THRUSTERS_TIME, (value) => {
                        _thrustersMaterial.SetColor("_EmissionColor", _colorThrusters * value);
                    });
                DOVirtual
                    .Vector3(JET_FLAME_SIZE_END, Vector3.zero, START_THRUSTERS_TIME, (value) => {
                        foreach (var rocketFlame in _rocketFlamesT) {
                            rocketFlame.localScale = value;
                        }
                    })
                    .OnComplete(() => {
                        _rotateFlameTweener.Kill();
                        _scaleFlameTweener.Kill();
                    });
            }
        }

        private void rotateFlame() {
            var randTime = Random.Range(PULSATING_THRUSTERS_TIME_START, PULSATING_THRUSTERS_TIME_END);
            _rotateFlameTweener.Kill();
            _rotateFlameTweener = DOVirtual
                .Vector3(_rocketFlameRotationStart, _rocketFlameRotationEnd, randTime, (value) => {
                    foreach (var rocketFlame in _rocketFlamesT) {
                        rocketFlame.localEulerAngles = value;
                    }
                })
                .OnComplete(() => {
                    rotateFlame();
                });

        }

        private void scaleFlame(bool odd) {
            var randTime = Random.Range(PULSATING_THRUSTERS_TIME_START, PULSATING_THRUSTERS_TIME_END);
            var from = odd ? _rocketFlameMax : _rocketFlameMin;
            var to = odd ? _rocketFlameMin : _rocketFlameMax;
            var diff = _rocketFlameMax - _rocketFlameMin;
            _scaleFlameTweener.Kill();
            _scaleFlameTweener = DOVirtual
                .Float(from, to, randTime, (value) => {
                    foreach (var rocketFlame in _rocketFlamesT) {
                        var rand = Random.Range(diff, diff * 2);
                        rocketFlame.localScale = new Vector3(rocketFlame.localScale.x, value + rand, rocketFlame.localScale.z);
                    }
                })
                .OnComplete(() => {
                    scaleFlame(!odd);
                });
        }

        private void changeThrustersIntensity(bool odd) {
            var randTime = Random.Range(PULSATING_THRUSTERS_TIME_START, PULSATING_THRUSTERS_TIME_END);

            var start = odd ? _startIntensityThrusters : _endIntensityThrusters;
            var end = odd ? _endIntensityThrusters : _startIntensityThrusters;

            _thrusterIntensityTweener = DOVirtual
                .Float(start, end, randTime, (value) => {
                    _thrustersMaterial.SetColor("_EmissionColor", _colorThrusters * value);
                })
                .OnComplete(() => {
                    changeThrustersIntensity(!odd);
                });
        }
    }
}
