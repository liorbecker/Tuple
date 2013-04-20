
namespace Tuple.Logic.Interfaces
{
    public enum Number
    {
        One,
        Two,
        Three
    }

    public static class NumberExtensions
    {
        public static int GetValue(this Number number)
        {
            switch (number)
            {
                case Number.One:
                    return 1;
                case Number.Two:
                    return 2;
                case Number.Three:
                    return 3;
                default:
                    return -1;
            }
        }
    }
}
