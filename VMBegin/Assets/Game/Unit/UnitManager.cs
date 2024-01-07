using Game.Shared.Interfaces;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Unit {
    public class UnitManager : MonoBehaviour, IUnitManager {
        public Dictionary<string, IUnit> Units { get; set; }
        private Dictionary<int, string> _unitsNames;

        public void Init() {
            Units = new Dictionary<string, IUnit>();
            _unitsNames = new Dictionary<int, string>();

            foreach (Transform ct in transform) {
                var unit = ct.GetComponent<IUnit>();
                unit?.Init();
                Units.Add(unit.Name, unit);
                _unitsNames.Add(unit.ID, unit.Name);
            }
        }

        public IUnit GetUnitById(int id) {
            return Units[_unitsNames[id]];
        }

        public IUnit GetUnit(string name) {
            return Units[name];
        }
    }
}
