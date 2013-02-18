
namespace Tuple.Logic.Interfaces
{
    public interface IDeck
    {
        public ICard GetNextCard();

        public bool IsEmpty();
    }
}
