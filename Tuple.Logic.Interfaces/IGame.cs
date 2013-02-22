
namespace Tuple.Logic.Interfaces
{
    /// <summary>
    /// Represents the entire logic of the game
    /// </summary>
    public interface IGame
    {
        /// <summary>
        /// Check wheather the game is over (no more cards to open and no more sets on board)
        /// </summary>
        /// <returns>true iff the game is over</returns>
        bool IsGameOver();

        /// <summary>
        /// Remove a set from the board
        /// </summary>
        /// <param name="firstCardPosition">Position of the first card in the set to check</param>
        /// <param name="secondCardPosition">Position of the second card in the set to check</param>
        /// <param name="thirdCardPosition">Position of the third card in the set to check</param>
        /// <returns>true if the provided set is a legal set</returns>
        bool RemoveSet(Position firstCardPosition, Position secondCardPosition, Position thirdCardPosition);

        /// <summary>
        /// Opens the next card from the deck
        /// </summary>
        /// <param name="position">The position to place the new card</param>
        /// <returns>The card to place on the board, or null if there is no need to open one</returns>
        ICard OpenCard(out Position position);
    }
}
