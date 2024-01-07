namespace Game.Shared.Interfaces {
    public interface IEntityId {
        int ID { get; set; }
        int CalculateId(bool setName = true);
    }
}
