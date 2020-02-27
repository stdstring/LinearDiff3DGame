using System.IO;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Serialization.Common
{
    public class PairBinarySerializer<T1, T2> : ISerializer<Pair<T1, T2>>
    {
        public PairBinarySerializer(ISerializer<T1> item1Serializer, ISerializer<T2> item2Serializer)
        {
            this.item1Serializer = item1Serializer;
            this.item2Serializer = item2Serializer;
        }

        public void Serialize(Stream storage, Pair<T1, T2> serializableObject)
        {
            item1Serializer.Serialize(storage, serializableObject.Item1);
            item2Serializer.Serialize(storage, serializableObject.Item2);
        }

        public Pair<T1, T2> Deserialize(Stream storage)
        {
            T1 item1 = item1Serializer.Deserialize(storage);
            T2 item2 = item2Serializer.Deserialize(storage);
            return new Pair<T1, T2>(item1, item2);
        }

        private readonly ISerializer<T1> item1Serializer;
        private readonly ISerializer<T2> item2Serializer;
    }
}