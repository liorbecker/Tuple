
namespace Tuple.Logic.Interfaces
{
    public interface IBoard
    {
        ICard this[int row, int col]
        {
            get;
            set;
        }
    }
}
