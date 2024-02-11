namespace Game.Shared.Interfaces {
    public interface IPlayer {
        IStoryData GetStoryData();

        object CinemachineBrain { get; }
    }
}
