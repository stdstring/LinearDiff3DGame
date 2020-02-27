using System;
using System.Collections.Generic;
using System.Globalization;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.AdvMath.MatrixUtils;
using LinearDiff3DGame.Geometry3D.Common;
using LinearDiff3DGame.MaxStableBridge.Gamers;
using LinearDiff3DGame.MaxStableBridge.Input;
using LinearDiff3DGame.MaxStableBridge.Tools;

namespace LinearDiff3DGame.MaxStableBridge
{
    public class BridgeBuilderData
    {
        public BridgeBuilderData(InputParams inputParams, ApproxComp approxComp)
        {
            Int32 matrixARowCount = inputParams.MatrixA.RowCount;
            Int32 matrixAColumnCount = inputParams.MatrixA.ColumnCount;
            Double[] matrixARawData = StringConvertHelper.ToDoubleArray(inputParams.MatrixA.RawData,
                                                                        CultureInfo.InvariantCulture);
            MatrixA = new MatrixFactory().CreateFromRawData(matrixARowCount,
                                                            matrixAColumnCount,
                                                            matrixARawData);
            DeltaT = inputParams.DeltaT;
            Y1Y2Y3Indexes = StringConvertHelper.ToInt32Array(inputParams.Y1Y2Y3IndexesRawData,
                                                             CultureInfo.InvariantCulture);
            ScalingMaxMinThreshold = inputParams.ScalingMaxMinThreshold;
            ApproxComp = approxComp;
            TerminalSet = CreateTerminalSet(inputParams.TerminalSetRawData);
            FirstGamers = CreateFirstGamers(inputParams.FirstGamers,
                                            DeltaT,
                                            approxComp,
                                            inputParams.SeparateNodeValue);
            SecondGamers = CreateSecondGamers(inputParams.SecondGamers,
                                              DeltaT,
                                              approxComp);
        }

        private static Point3D[] CreateTerminalSet(IEnumerable<String> terminalSetRawData)
        {
            List<Point3D> terminalSet = new List<Point3D>();
            foreach(String pointRaw in terminalSetRawData)
            {
                Double[] pointCoords = StringConvertHelper.ToDoubleArray(pointRaw,
                                                                         CultureInfo.InvariantCulture);
                if(pointCoords.Length != 3) throw new ArgumentException();
                terminalSet.Add(new Point3D(pointCoords[0], pointCoords[1], pointCoords[2]));
            }
            return terminalSet.ToArray();
        }

        private static IEnumerable<FirstGamerInitData> CreateFirstGamers(IEnumerable<GamerParams> gamersList, Double deltaT, ApproxComp approxComp, Double separateNodeValue)
        {
            IList<FirstGamerInitData> firstGamers = new List<FirstGamerInitData>();
            foreach(GamerParams gamerParams in gamersList)
                firstGamers.Add(new FirstGamerInitData(gamerParams, deltaT, approxComp, separateNodeValue));
            return firstGamers;
        }

        private static IEnumerable<GamerInitData> CreateSecondGamers(IEnumerable<GamerParams> gamersList, Double deltaT, ApproxComp approxComp)
        {
            IList<GamerInitData> secondGamers = new List<GamerInitData>();
            foreach(GamerParams gamerParams in gamersList)
                secondGamers.Add(new GamerInitData(gamerParams, deltaT, approxComp));
            return secondGamers;
        }

        public Matrix MatrixA { get; private set; }
        public Point3D[] TerminalSet { get; private set; }
        public Double DeltaT { get; private set; }
        public Int32[] Y1Y2Y3Indexes { get; private set; }
        public IEnumerable<FirstGamerInitData> FirstGamers { get; private set; }
        public IEnumerable<GamerInitData> SecondGamers { get; private set; }
        public Double ScalingMaxMinThreshold { get; private set; }
        public ApproxComp ApproxComp { get; private set; }
    }
}