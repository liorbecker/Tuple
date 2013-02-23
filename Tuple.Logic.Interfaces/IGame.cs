
using System.Collections.Generic;
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
        /// <param name="firstCard">The first card in the set to check</param>
        /// <param name="secondCard">The second card in the set to check</param>
        /// <param name="thirdCard">The third card in the set to check</param>
        /// <returns>true if the provided set is a legal set</returns>
        bool RemoveSet(ICardWithPosition firstCard, ICardWithPosition secondCard, ICardWithPosition thirdCard);

        /// <summary>
        /// Indicates if the current board need to call <see cref="OpenCard"/>
        /// </summary>
        /// <returns>true if you need to call <see cref="OpenCard"/></returns>
        bool ShouldOpenCard();

        /// <summary>
        /// Opens the next card from the deck
        /// </summary>
        /// <returns>The card to place on the board</returns>
        ICardWithPosition OpenCard();

        /// <summary>
        /// Get all the opened cards on the board
        /// </summary>
        /// <returns>A collection with all the opened cards on the board</returns>
        IEnumerable<ICardWithPosition> GetAllOpenedCards(); 
    }
}
