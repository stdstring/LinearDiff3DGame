using System;

namespace LinearDiff3DGame.Common
{
    public class Tuple<T1, T2>
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
            Int32 hashCode = 0;
            if(Item1 != null) hashCode ^= Item1.GetHashCode();
            if(Item2 != null) hashCode ^= Item2.GetHashCode();
            return hashCode;
        }
    }

    public class Tuple<T1, T2, T3>
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
            Int32 hashCode = 0;
            if(Item1 != null) hashCode ^= Item1.GetHashCode();
            if(Item2 != null) hashCode ^= Item2.GetHashCode();
            if(Item3 != null) hashCode ^= Item3.GetHashCode();
            return hashCode;
        }
    }

    public class Tuple<T1, T2, T3, T4>
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
            Int32 hashCode = 0;
            if(Item1 != null) hashCode ^= Item1.GetHashCode();
            if(Item2 != null) hashCode ^= Item2.GetHashCode();
            if(Item3 != null) hashCode ^= Item3.GetHashCode();
            if(Item4 != null) hashCode ^= Item4.GetHashCode();
            return hashCode;
        }
    }
}