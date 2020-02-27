using System.IO;

namespace LinearDiff3DGame.Serialization
{
    public interface ISerializer<TSerializableObject>
    {
        void Serialize(Stream storage, TSerializableObject serializableObject);
        TSerializableObject Deserialize(Stream storage);
    }
}