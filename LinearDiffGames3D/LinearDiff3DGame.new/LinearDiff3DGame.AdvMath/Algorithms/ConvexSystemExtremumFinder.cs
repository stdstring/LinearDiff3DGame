using System;
using System.Collections.Generic;
using System.Linq;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.AdvMath.Algorithms
{
    public class ConvexSystemExtremumFinder
    {
        public TItem SearchMax<TItem>(TItem initialItem,
            Func<TItem, IEnumerable<TItem>> neighbourSelector,
            Func<TItem, Double> searchCriteria)
        {
            return Search(initialItem,
                          neighbourSelector,
                          searchCriteria,
                          Max,
                          (currentValue, value) => currentValue < value);
        }

        public TItem SearchMin<TItem>(TItem initialItem,
            Func<TItem, IEnumerable<TItem>> neighbourSelector,
            Func<TItem, Double> searchCriteria)
        {
            return Search(initialItem,
                          neighbourSelector,
                          searchCriteria,
                          Min,
                          (currentValue, value) => currentValue > value);
        }

        private static TItem Search<TItem>(TItem initialItem,
            Func<TItem, IEnumerable<TItem>> neighbourSelector,
            Func<TItem, Double> searchCriteria,
            Func<IEnumerable<TItem>, Func<TItem, Double>, Tuple<TItem, Double>> elementSelector,
            Func<Double, Double, Boolean> decision)
        {
            TItem currentItem = initialItem;
            Double currentValue = searchCriteria(currentItem);
            Boolean proceedSearch = true;
            while(proceedSearch)
            {
                proceedSearch = false;
                IEnumerable<TItem> neighbours = neighbourSelector(currentItem);
                Tuple<TItem, Double> item = elementSelector(neighbours, searchCriteria);
                if(decision(currentValue, item.Item2))
                {
                    currentItem = item.Item1;
                    currentValue = item.Item2;
                    proceedSearch = true;
                }
            }
            return currentItem;
        }

        private static Tuple<TItem, Double> Min<TItem>(IEnumerable<TItem> sequence, Func<TItem, Double> itemValue)
        {
            return sequence.Aggregate(new Tuple<TItem, Double>(default(TItem), Double.MaxValue),
                                      (accumulator, source) =>
                                      {
                                          Double value = itemValue(source);
                                          return value < accumulator.Item2 ? new Tuple<TItem, Double>(source, value) : accumulator;
                                      });
        }

        private static Tuple<TItem, Double> Max<TItem>(IEnumerable<TItem> sequence, Func<TItem, Double> itemValue)
        {
            return sequence.Aggregate(new Tuple<TItem, Double>(default(TItem), Double.MinValue),
                                      (accumulator, source) =>
                                      {
                                          Double value = itemValue(source);
                                          return value > accumulator.Item2 ? new Tuple<TItem, Double>(source, value) : accumulator;
                                      });
        }
    }
}
