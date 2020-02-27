using System;
using System.Collections.Generic;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.AdvMath.Algorithms
{
    public class ConvexSystemFinder
    {
        public TItem Search<TItem>(TItem startItem,
                                   Func<TItem, Boolean> searchCriteria,
                                   Func<TItem, TItem> nextItemSelector)
        {
            Boolean completed = false;
            TItem currentItem = startItem;
            while(!completed)
            {
                if(searchCriteria(currentItem))
                    completed = true;
                else
                    currentItem = nextItemSelector(currentItem);
            }
            return currentItem;
        }

        public Pair<TItem, TCriteria> Search<TItem, TCriteria>(TItem startItem,
                                                               Func<TItem, TCriteria> searchCriteriaCalc,
                                                               Func<TItem, TCriteria, Boolean> searchCriteria,
                                                               Func<TItem, TCriteria, TItem> nextItemSelector)
        {
            Boolean completed = false;
            TItem currentItem = startItem;
            TCriteria currentCriteria = default(TCriteria);
            while(!completed)
            {
                currentCriteria = searchCriteriaCalc(currentItem);
                if(searchCriteria(currentItem, currentCriteria))
                    completed = true;
                else
                    currentItem = nextItemSelector(currentItem, currentCriteria);
            }
            return new Pair<TItem, TCriteria>(currentItem, currentCriteria);
        }

        public TItem SafeSearch<TItem>(TItem startItem,
                                       Func<TItem, Boolean> searchCriteria,
                                       Func<TItem, TItem> nextItemSelector)
        {
            return SafeSearch(startItem, EqualityComparer<TItem>.Default, searchCriteria, nextItemSelector);
        }

        public TItem SafeSearch<TItem>(TItem startItem,
                                       IEqualityComparer<TItem> itemEqualityComparer,
                                       Func<TItem, Boolean> searchCriteria,
                                       Func<TItem, TItem> nextItemSelector)
        {
            IDictionary<TItem, Object> visitedItems = new Dictionary<TItem, Object>(itemEqualityComparer);
            Boolean completed = false;
            TItem currentItem = startItem;
            visitedItems.Add(startItem, null);
            while(!completed)
            {
                if(searchCriteria(currentItem))
                    completed = true;
                else
                {
                    currentItem = nextItemSelector(currentItem);
                    if(visitedItems.ContainsKey(currentItem))
                        throw new AlgorithmException("Loop in search");
                    visitedItems.Add(currentItem, null);
                }
            }
            return currentItem;
        }

        public Pair<TItem, TCriteria> SafeSearch<TItem, TCriteria>(TItem startItem,
                                                                   Func<TItem, TCriteria> searchCriteriaCalc,
                                                                   Func<TItem, TCriteria, Boolean> searchCriteria,
                                                                   Func<TItem, TCriteria, TItem> nextItemSelector)
        {
            return SafeSearch(startItem,
                              EqualityComparer<TItem>.Default,
                              searchCriteriaCalc,
                              searchCriteria,
                              nextItemSelector);
        }

        public Pair<TItem, TCriteria> SafeSearch<TItem, TCriteria>(TItem startItem,
                                                                   IEqualityComparer<TItem> itemEqualityComparer,
                                                                   Func<TItem, TCriteria> searchCriteriaCalc,
                                                                   Func<TItem, TCriteria, Boolean> searchCriteria,
                                                                   Func<TItem, TCriteria, TItem> nextItemSelector)
        {
            IDictionary<TItem, Object> visitedItems = new Dictionary<TItem, Object>(itemEqualityComparer);
            Boolean completed = false;
            TItem currentItem = startItem;
            TCriteria currentCriteria = default(TCriteria);
            visitedItems.Add(startItem, null);
            while(!completed)
            {
                currentCriteria = searchCriteriaCalc(currentItem);
                if(searchCriteria(currentItem, currentCriteria))
                    completed = true;
                else
                {
                    currentItem = nextItemSelector(currentItem, currentCriteria);
                    if(visitedItems.ContainsKey(currentItem))
                        throw new AlgorithmException("Loop in search");
                    visitedItems.Add(currentItem, null);
                }
            }
            return new Pair<TItem, TCriteria>(currentItem, currentCriteria);
        }
    }
}