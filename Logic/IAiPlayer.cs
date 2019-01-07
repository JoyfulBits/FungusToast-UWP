namespace Logic
{
    public interface IAiPlayer : IPlayer
    {
        AiType AiType { get; }
    }
}