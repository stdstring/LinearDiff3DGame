using System;
using System.Collections.Generic;

namespace LinearDiff3DGame.Serialization.Testing
{
    internal static class EnumerableEqualityTester
    {
        public static Boolean TestEquality<TItem>(IEnumerable<TItem> enumerable1,
                                                  IEnumerable<TItem> enumerable2,
                                                  Func<TItem, TItem, Boolean> itemEqualityTester)
        {
            IEnumerator<TItem> enum1 = enumerable1.GetEnumerator();
            IEnumerator<TItem> enum2 = enumerable2.GetEnumerator();
            enum1.Reset();
            enum2.Reset();
            while(enum1.MoveNext())
            {
                if(!enum2.MoveNext()) return false;
                if(!itemEqualityTester(enum1.Current, enum2.Current)) return false;
            }
            return !enum2.MoveNext();
        }
    }
}