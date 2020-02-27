using System;
using System.Collections.Generic;
using System.IO;

namespace LinearDiff3DGame.Serialization.Common
{
    public class LazyEnumerableBinarySerializer<TSerializedObject> : EnumerableBinarySerializer<TSerializedObject>
    {
        public LazyEnumerableBinarySerializer(ISerializer<TSerializedObject> itemSerializer)
            : base(itemSerializer)
        {
        }

        protected override IEnumerable<TSerializedObject> DeserializeItems(Stream storage, Int32 itemCount)
        {
            for(Int32 itemIndex = 0; itemIndex < itemCount; ++itemIndex)
                yield return ItemSerializer.Deserialize(storage);
        }
    }
}