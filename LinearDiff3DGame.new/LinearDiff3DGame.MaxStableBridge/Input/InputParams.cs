using System;
using System.IO;
using System.Xml.Serialization;

namespace LinearDiff3DGame.MaxStableBridge.Input
{
    [XmlRoot("Input")]
    public class InputParams
    {
        [XmlAttribute("Description")]
        public String Description { get; set; }

        [XmlElement("MatrixA")]
        public MatrixParams MatrixA { get; set; }

        [XmlArray("FirstGamers")]
        [XmlArrayItem("FirstGamer")]
        public GamerParams[] FirstGamers { get; set; }

        [XmlArray("SecondGamers")]
        [XmlArrayItem("SecondGamer")]
        public GamerParams[] SecondGamers { get; set; }

        [XmlArray("TerminalSet")]
        [XmlArrayItem("Point")]
        public String[] TerminalSetRawData { get; set; }

        [XmlElement("Y1Y2Y3Indexes")]
        public String Y1Y2Y3IndexesRawData { get; set; }

        [XmlElement("DeltaT")]
        public Double DeltaT { get; set; }

        [XmlElement("ScalingMaxMinThreshold")]
        public Double ScalingMaxMinThreshold { get; set; }

        [XmlElement("SeparateNodeValue")]
        public Double SeparateNodeValue { get; set; }
    }

    [XmlRoot("Gamer")]
    public class GamerParams
    {
        [XmlElement("Matrix")]
        public MatrixParams Matrix { get; set; }

        [XmlElement("MinSection")]
        public Double MinSection { get; set; }

        [XmlElement("MaxSection")]
        public Double MaxSection { get; set; }
    }

    [XmlRoot("Matrix")]
    public class MatrixParams
    {
        [XmlAttribute("RowCount")]
        public Int32 RowCount { get; set; }

        [XmlAttribute("ColumnCount")]
        public Int32 ColumnCount { get; set; }

        [XmlText]
        public String RawData;
    }

    public class InputParamsSerializer
    {
        //public void Serialize(String destFileName, InputParams inputParams)
        //{
        //    using(StreamWriter writer = new StreamWriter(destFileName))
        //        serializer.Serialize(writer, inputParams);
        //}

        public void Serialize(TextWriter dest, InputParams inputParams)
        {
            serializer.Serialize(dest, inputParams);
        }

        public void Serialize(Stream dest, InputParams inputParams)
        {
            serializer.Serialize(dest, inputParams);
        }

        //public InputParams Deserialize(String sourceFileName)
        //{
        //    using(StreamReader reader = new StreamReader(sourceFileName))
        //        return (InputParams)serializer.Deserialize(reader);
        //}

        public InputParams Deserialize(TextReader source)
        {
            return (InputParams)serializer.Deserialize(source);
        }

        public InputParams Deserialize(Stream source)
        {
            return (InputParams)serializer.Deserialize(source);
        }

        private readonly XmlSerializer serializer = new XmlSerializer(typeof(InputParams));
    }
}