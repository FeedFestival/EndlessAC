using System.Collections.Generic;
using UnityEngine;

namespace GameScrypt.GSUtils
{
    public static class GSPercent
    {
#pragma warning disable 0414
        public static readonly string _version = "3.0.0";
#pragma warning restore 0414

        public static float Find(float _percent, float _of)
        {
            return (_of / 100f) * _percent;
        }
        public static float What(float _is, float _of)
        {
            return (_is * 100f) / _of;
        }

        public static int PennyToss(int _from = 0, int _to = 100)
        {
            var randomNumber = Random.Range(_from, _to);
            return (randomNumber > 50) ? 1 : 0;
        }
    }
}
