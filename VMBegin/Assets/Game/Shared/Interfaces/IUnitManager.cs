using System.Collections.Generic;

namespace Game.Shared.Interfaces {
    public interface IUnitManager {
        Dictionary<string, IUnit> Units { get; set; }
        void Init(IBasePrefabs basePrefabs);
        IUnit GetUnitById(int id);
        IUnit GetUnit(string name);
    }
}