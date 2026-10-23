public class TurnManager
{
    private readonly PlayerState[] players;
    private readonly int cardsPerTurn;

    public int CurrentPlayerIndex { get; private set; }
    public PlayerState CurrentPlayer => players[CurrentPlayerIndex];

    public TurnManager(PlayerState[] players, int cardsPerTurn = 1)
    {
        this.players = players;
        this.cardsPerTurn = cardsPerTurn;
    }

    public void StartFirstTurn(int firstPlayer = 0)
    {
        CurrentPlayerIndex = firstPlayer;
        BeginTurn();
    }

    public void EndTurn()
    {
        GameEvents.RaiseTurnEnded(CurrentPlayerIndex);
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % players.Length;
        BeginTurn();
    }

    private void BeginTurn()
    {
        CurrentPlayer.RefillEnergy();
        CurrentPlayer.DrawCards(cardsPerTurn);
        GameEvents.RaiseTurnStarted(CurrentPlayerIndex);
    }
}