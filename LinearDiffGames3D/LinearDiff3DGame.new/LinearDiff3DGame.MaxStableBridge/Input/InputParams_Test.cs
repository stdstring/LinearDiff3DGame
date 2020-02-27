using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace LinearDiff3DGame.MaxStableBridge.Input
{
    [TestFixture]
    public class InputParams_Test
    {
        [Test]
        public void Serialization()
        {
            InputParams inputParams = new InputParams();
            inputParams.Description = "sample";
            inputParams.MatrixA = new MatrixParams
                                      {RowCount = 3, ColumnCount = 3, RawData = "1.0 0.0 3.1 2.1 1.2 0.0 9.9 8.8 7.7"};
            inputParams.FirstGamers =
                new[]
                    {
                        new GamerParams {Matrix = new MatrixParams {RowCount = 3, ColumnCount = 1, RawData = "1.0 2.3 4.0"}, MaxSection = 1.0, MinSection = -1.0},
                        new GamerParams {Matrix = new MatrixParams {RowCount = 3, ColumnCount = 1, RawData = "-2.0 0.0 0.11"}, MaxSection = 3.0, MinSection = -3.0}
                    };
            inputParams.SecondGamers =
                new[]
                    {
                        new GamerParams {Matrix = new MatrixParams {RowCount = 3, ColumnCount = 1, RawData = "-3.5 -1.4 2.0"}, MaxSection = 11.0, MinSection = -11.0},
                        new GamerParams {Matrix = new MatrixParams {RowCount = 3, ColumnCount = 1, RawData = "6.0 10.0 3.11"}, MaxSection = 6.0, MinSection = -6.0}
                    };
            inputParams.TerminalSetRawData =
                new[] { "1.0 0.0 0.0", "0.0 1.0 0.0", "-1.0 0.0 0.0", "0.0 -1.0 0.0", "0.0 0.0 1.0" };
            inputParams.Y1Y2Y3IndexesRawData = "1 2 3";
            inputParams.DeltaT = 0.1;
            inputParams.ScalingMaxMinThreshold = 2.0;
            inputParams.SeparateNodeValue = 0.0001;
            String dest;
            using(StringWriter sw = new StringWriter())
            {
                new InputParamsSerializer().Serialize(sw, inputParams);
                dest = sw.GetStringBuilder().ToString();
            }
            String expected;
            using(StreamReader sr = new StreamReader(GetInputDataSample()))
                expected = sr.ReadToEnd();
            Assert.AreEqual(expected, dest);
        }

        [Test]
        public void Deserialization()
        {
            InputParams inputParams = new InputParamsSerializer().Deserialize(GetInputDataSample());
            Assert.AreEqual("sample", inputParams.Description);
            Assert.IsNotNull(inputParams.MatrixA);
            Assert.AreEqual(3, inputParams.MatrixA.RowCount);
            Assert.AreEqual(3, inputParams.MatrixA.ColumnCount);
            Assert.AreEqual("1.0 0.0 3.1 2.1 1.2 0.0 9.9 8.8 7.7", inputParams.MatrixA.RawData);
            Assert.IsNotNull(inputParams.FirstGamers);
            Assert.AreEqual(2, inputParams.FirstGamers.Length);
            Assert.IsNotNull(inputParams.FirstGamers[0].Matrix);
            Assert.AreEqual(3, inputParams.FirstGamers[0].Matrix.RowCount);
            Assert.AreEqual(1, inputParams.FirstGamers[0].Matrix.ColumnCount);
            Assert.AreEqual("1.0 2.3 4.0", inputParams.FirstGamers[0].Matrix.RawData);
            Assert.AreEqual(-1.0, inputParams.FirstGamers[0].MinSection);
            Assert.AreEqual(1.0, inputParams.FirstGamers[0].MaxSection);
            Assert.IsNotNull(inputParams.FirstGamers[1].Matrix);
            Assert.AreEqual(3, inputParams.FirstGamers[1].Matrix.RowCount);
            Assert.AreEqual(1, inputParams.FirstGamers[1].Matrix.ColumnCount);
            Assert.AreEqual("-2.0 0.0 0.11", inputParams.FirstGamers[1].Matrix.RawData);
            Assert.AreEqual(-3.0, inputParams.FirstGamers[1].MinSection);
            Assert.AreEqual(3.0, inputParams.FirstGamers[1].MaxSection);
            Assert.IsNotNull(inputParams.SecondGamers);
            Assert.AreEqual(2, inputParams.SecondGamers.Length);
            Assert.IsNotNull(inputParams.SecondGamers[0].Matrix);
            Assert.AreEqual(3, inputParams.SecondGamers[0].Matrix.RowCount);
            Assert.AreEqual(1, inputParams.SecondGamers[0].Matrix.ColumnCount);
            Assert.AreEqual("-3.5 -1.4 2.0", inputParams.SecondGamers[0].Matrix.RawData);
            Assert.AreEqual(-11.0, inputParams.SecondGamers[0].MinSection);
            Assert.AreEqual(11.0, inputParams.SecondGamers[0].MaxSection);
            Assert.IsNotNull(inputParams.SecondGamers[1].Matrix);
            Assert.AreEqual(3, inputParams.SecondGamers[1].Matrix.RowCount);
            Assert.AreEqual(1, inputParams.SecondGamers[1].Matrix.ColumnCount);
            Assert.AreEqual("6.0 10.0 3.11", inputParams.SecondGamers[1].Matrix.RawData);
            Assert.AreEqual(-6.0, inputParams.SecondGamers[1].MinSection);
            Assert.AreEqual(6.0, inputParams.SecondGamers[1].MaxSection);
            Assert.IsNotNull(inputParams.TerminalSetRawData);
            Assert.AreEqual(5, inputParams.TerminalSetRawData.Length);
            Assert.AreEqual("1.0 0.0 0.0", inputParams.TerminalSetRawData[0]);
            Assert.AreEqual("0.0 1.0 0.0", inputParams.TerminalSetRawData[1]);
            Assert.AreEqual("-1.0 0.0 0.0", inputParams.TerminalSetRawData[2]);
            Assert.AreEqual("0.0 -1.0 0.0", inputParams.TerminalSetRawData[3]);
            Assert.AreEqual("0.0 0.0 1.0", inputParams.TerminalSetRawData[4]);
            Assert.AreEqual("1 2 3", inputParams.Y1Y2Y3IndexesRawData);
            Assert.AreEqual(0.1, inputParams.DeltaT);
            Assert.AreEqual(2, inputParams.ScalingMaxMinThreshold);
            Assert.AreEqual(0.0001, inputParams.SeparateNodeValue);
        }

        private Stream GetInputDataSample()
        {
            Assembly currentAssembly = Assembly.GetAssembly(GetType());
            return currentAssembly.GetManifestResourceStream(GetType(), "InputDataSample.xml");
        }
    }
}