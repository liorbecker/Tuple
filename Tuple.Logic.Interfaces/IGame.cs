
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
        /// <param name="firstCardRow">Row of the first card</param>
        /// <param name="firstCardCol">Column of the first card</param>
        /// <param name="secondCardRow">Row of the second card</param>
        /// <param name="secondCardCol">Column of the second card</param>
        /// <param name="thirdCardRow">Row of the third card</param>
        /// <param name="thirdCardCol">Column of the third card</param>
        /// <returns>true if the provided set is a legal set</returns>
        bool RemoveSet(int firstCardRow, int firstCardCol, int secondCardRow, int secondCardCol, int thirdCardRow, int thirdCardCol);

        /// <summary>
        /// Opens the next card from the deck
        /// </summary>
        /// <param name="row">The row to place the new card</param>
        /// <param name="col">The column to place the new card</param>
        /// <returns>The card to place on the board, or null if there is no need to open one</returns>
        ICard OpenCard(out int row, out int col);
    }
}
