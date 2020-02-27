using System;
using System.Collections.Generic;
using System.IO;
using LinearDiff3DGame.Common;

namespace LinearDiff3DGame.Serialization.Common
{
    public class EnumerableBinarySerializer<TSerializedObject> : ISerializer<Pair<Int32, IEnumerable<TSerializedObject>>>
    {
        public EnumerableBinarySerializer(ISerializer<TSerializedObject> itemSerializer)
        {
            ItemSerializer = itemSerializer;
        }

        public void Serialize(Stream storage, Pair<int, IEnumerable<TSerializedObject>> serializableObject)
        {
            int32BinarySerializer.Serialize(storage, serializableObject.Item1);
            foreach(TSerializedObject item in serializableObject.Item2)
                ItemSerializer.Serialize(storage, item);
        }

        public Pair<int, IEnumerable<TSerializedObject>> Deserialize(Stream storage)
        {
            Int32 count = int32BinarySerializer.Deserialize(storage);
            IEnumerable<TSerializedObject> items = DeserializeItems(storage, count);
            return new Pair<Int32, IEnumerable<TSerializedObject>>(count, items);
        }

        protected virtual IEnumerable<TSerializedObject> DeserializeItems(Stream storage, Int32 itemCount)
        {
            IList<TSerializedObject> items = new List<TSerializedObject>();
            for(Int32 itemIndex = 0; itemIndex < itemCount; ++itemIndex)
                items.Add(ItemSerializer.Deserialize(storage));
            return items;
        }

        protected ISerializer<TSerializedObject> ItemSerializer { get; private set; }
        private readonly Int32BinarySerializer int32BinarySerializer = new Int32BinarySerializer();
    }
}