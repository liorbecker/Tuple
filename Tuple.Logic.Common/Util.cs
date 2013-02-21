using System.Collections.Generic;
using Tuple.Logic.Interfaces;
using System.Linq;

namespace Tuple.Logic.Common
{
    public static class Util
    {
        public static bool isLegalSet(ICard firstCard, ICard secondCard, ICard thirdCard)
        {
            return
                ((firstCard.Symbol != secondCard.Symbol && firstCard.Symbol != thirdCard.Symbol && secondCard.Symbol != thirdCard.Symbol) ||
                (firstCard.Symbol == secondCard.Symbol && firstCard.Symbol == thirdCard.Symbol)) &&
                ((firstCard.Color != secondCard.Color && firstCard.Color != thirdCard.Color && secondCard.Color != thirdCard.Color) ||
                (firstCard.Color == secondCard.Color && firstCard.Color == thirdCard.Color)) &&
                ((firstCard.Number != secondCard.Number && firstCard.Number != thirdCard.Number && secondCard.Number != thirdCard.Number) ||
                (firstCard.Number == secondCard.Number && firstCard.Number == thirdCard.Number)) &&
                ((firstCard.Shading != secondCard.Shading && firstCard.Shading != thirdCard.Shading && secondCard.Shading != thirdCard.Shading) ||
                (firstCard.Shading == secondCard.Shading && firstCard.Shading == thirdCard.Shading));
        }

        public static bool isThereSet(IEnumerable<ICard> cards)
        {
            var cs = cards.ToArray();
            var count = cs.Count();

            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    for (int k = j + 1; k < count; k++)
                    {
                        if (isLegalSet(cs[i], cs[j], cs[k]))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
