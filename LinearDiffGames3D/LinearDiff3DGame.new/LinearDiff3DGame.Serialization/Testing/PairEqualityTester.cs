using System;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Serialization.Testing
{
    internal static class PairEqualityTester
    {
        public static Boolean TestEquality<TItem1, TItem2>(Pair<TItem1, TItem2> pair1,
                                                           Pair<TItem1, TItem2> pair2)
        {
            return TestEquality(pair1,
                                pair2,
                                (item11, item21) => Equals(item11, item21),
                                (item12, item22) => Equals(item12, item22));
        }

        public static Boolean TestEquality<TItem1, TItem2>(Pair<TItem1, TItem2> pair1,
                                                           Pair<TItem1, TItem2> pair2,
                                                           Func<TItem1, TItem1, Boolean> item1EqualityTester,
                                                           Func<TItem2, TItem2, Boolean> item2EqualityTester)
        {
            return item1EqualityTester(pair1.Item1, pair2.Item1) &&
                   item2EqualityTester(pair1.Item2, pair2.Item2);
        }
    }
}