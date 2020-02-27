using System;

namespace LinearDiff3DGame.Common
{
    public class Tuple<T1, T2> : IEquatable<Tuple<T1, T2>>
    {
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }

        public Boolean Equals(Tuple<T1, T2> other)
        {
            return Equals(Item1, other.Item1) && Equals(Item2, other.Item2);
        }

        public override bool Equals(object obj)
        {
            Tuple<T1, T2> other = obj as Tuple<T1, T2>;
            if(other == null) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            Int32 hashCode = HashCodeHelper<T1>.Calculate(0, Item1);
            return HashCodeHelper<T2>.Calculate(hashCode, Item2);
        }
    }

    public class Pair<T> : Tuple<T, T>
    {
        public Pair(T item1, T item2)
            : base(item1, item2)
        {
        }
    }

    public class Tuple<T1, T2, T3> : IEquatable<Tuple<T1, T2, T3>>
    {
        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }

        public Boolean Equals(Tuple<T1, T2, T3> other)
        {
            return Equals(Item1, other.Item1) && Equals(Item2, other.Item2) && Equals(Item3, other.Item3);
        }

        public override bool Equals(object obj)
        {
            Tuple<T1, T2, T3> other = obj as Tuple<T1, T2, T3>;
            if(other == null) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            Int32 hashCode = HashCodeHelper<T1>.Calculate(0, Item1);
            hashCode = HashCodeHelper<T2>.Calculate(hashCode, Item2);
            return HashCodeHelper<T3>.Calculate(hashCode, Item3);
        }
    }

    public class Triple<T> : Tuple<T, T, T>
    {
        public Triple(T item1, T item2, T item3)
            : base(item1, item2, item3)
        {
        }
    }

    public class Tuple<T1, T2, T3, T4> : IEquatable<Tuple<T1, T2, T3, T4>>
    {
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
            Item4 = item4;
        }

        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
        public T3 Item3 { get; private set; }
        public T4 Item4 { get; private set; }

        public Boolean Equals(Tuple<T1, T2, T3, T4> other)
        {
            return Equals(Item1, other.Item1) &&
                   Equals(Item2, other.Item2) &&
                   Equals(Item3, other.Item3) &&
                   Equals(Item4, other.Item4);
        }

        public override bool Equals(object obj)
        {
            Tuple<T1, T2, T3, T4> other = obj as Tuple<T1, T2, T3, T4>;
            if(other == null) return false;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            Int32 hashCode = HashCodeHelper<T1>.Calculate(0, Item1);
            hashCode = HashCodeHelper<T2>.Calculate(hashCode, Item2);
            hashCode = HashCodeHelper<T3>.Calculate(hashCode, Item3);
            return HashCodeHelper<T4>.Calculate(hashCode, Item4);
        }
    }

    internal static class HashCodeHelper<T>
    {
        public static Int32 Calculate(Int32 previousValue, T item)
        {
            // ReSharper disable CompareNonConstrainedGenericWithNull
            if(item == null) return previousValue;
            // ReSharper restore CompareNonConstrainedGenericWithNull
            return previousValue == 0 ? item.GetHashCode() : (previousValue*397) ^ item.GetHashCode();
        }
    }
}