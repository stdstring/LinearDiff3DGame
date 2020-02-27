using System;
using System.Collections.Generic;
using System.Linq;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.RobustControl.Algorithms
{
    public interface IWaveProcessing<TItem>
    {
        void Process(TItem startItem,
                     TItem startItemParent,
                     Func<TItem, IEnumerable<TItem>> neighborsSelector,
                     Action<TItem, TItem> processItem);

        void Process(TItem startItem,
                     TItem startItemParent,
                     Func<TItem, IEnumerable<TItem>> neighborsSelector,
                     IEqualityComparer<TItem> equalityComparer,
                     Action<TItem, TItem> processItem);
    }

    public class WaveProcessing<TItem> : IWaveProcessing<TItem>
    {
        public void Process(TItem startItem,
                            TItem startItemParent,
                            Func<TItem, IEnumerable<TItem>> neighborsSelector,
                            Action<TItem, TItem> processItem)
        {
            Process(startItem,
                    startItemParent,
                    neighborsSelector,
                    EqualityComparer<TItem>.Default,
                    processItem);
        }

        public void Process(TItem startItem,
                            TItem startItemParent,
                            Func<TItem, IEnumerable<TItem>> neighborsSelector,
                            IEqualityComparer<TItem> equalityComparer,
                            Action<TItem, TItem> processItem)
        {
            List<TItem> prevWaveFront = new List<TItem>();
            List<TItem> currentWaveFront = new List<TItem>();
            ItemPairEqualityComparer itemPairEqualityComparer = new ItemPairEqualityComparer(equalityComparer);
            List<Pair<TItem>> nextWaveFront = new List<Pair<TItem>>();
            // формируем стартовый фронт волны
            nextWaveFront.Add(new Pair<TItem>(startItemParent, startItem));
            do
            {
                nextWaveFront.ForEach(pair => processItem(pair.Item1, pair.Item2));
                // после обработки
                prevWaveFront.Clear();
                prevWaveFront.AddRange(currentWaveFront);
                currentWaveFront.Clear();
                currentWaveFront.AddRange(nextWaveFront.Select(pair => pair.Item2));
                nextWaveFront.Clear();
                // в обрабатываемый фронт добавляются элементы, которых нет в текущем и предыдущем обработанных фронтах
                // учитываем также тот факт, что разные родители могут давать одни и те же дочерние элементы
                nextWaveFront.AddRange(currentWaveFront
                                           .SelectMany(parent => neighborsSelector(parent).Select(item => new Pair<TItem>(parent, item)))
                                           .Where(pair => prevWaveFront.Count(other => equalityComparer.Equals(pair.Item2, other)) == 0 &&
                                                          currentWaveFront.Count(other => equalityComparer.Equals(pair.Item2, other)) == 0)
                                           .Distinct(itemPairEqualityComparer));
            } while(nextWaveFront.Count > 0);
        }

        // Мы храним пару элементов, где первый элемент - родительский.
        // Разные родители могут дать одного и того же потомка.
        // Поэтому нам интересно уметь сравнивать на эквивалентность только по потомку (чтобы уметь получить набор всех потомков без повторений)
        private class ItemPairEqualityComparer : IEqualityComparer<Pair<TItem>>
        {
            public ItemPairEqualityComparer(IEqualityComparer<TItem> itemEqualityComparer)
            {
                this.itemEqualityComparer = itemEqualityComparer;
            }

            public Boolean Equals(Pair<TItem> x, Pair<TItem> y)
            {
                return itemEqualityComparer.Equals(x.Item2, y.Item2);
            }

            public Int32 GetHashCode(Pair<TItem> obj)
            {
                return itemEqualityComparer.GetHashCode(obj.Item2);
            }

            private readonly IEqualityComparer<TItem> itemEqualityComparer;
        }
    }
}