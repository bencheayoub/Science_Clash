/// <summary>The win rule: a player loses when they have no active scientist left.</summary>
public static class WinChecker
{
    public static bool TryGetWinner(PlayerState[] players, out int winnerIndex)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].ActiveScientist == null)
            {
                winnerIndex = 1 - i;
                return true;
            }
        }

        winnerIndex = -1;
        return false;
    }
}
