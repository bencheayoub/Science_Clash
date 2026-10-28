/// <summary>What a controller (AI, later maybe network/replay) may do to a running match.</summary>
public interface IMatchActions
{
    bool IsGameOver { get; }
    PlayerState GetPlayer(int index);
    bool TryPlayCard(int playerIndex, CardData card);
    void EndTurn();
}
