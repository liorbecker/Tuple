using System;
namespace Tuple.Logic.Interfaces
{
    public interface IDeck
    {
        ICard GetNextCard();

        bool IsEmpty();
    }
}
