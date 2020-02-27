using System;
using System.Globalization;
using LinearDiff3DGame.AdvMath.Common;
using LinearDiff3DGame.AdvMath.Matrix;
using LinearDiff3DGame.AdvMath.MatrixUtils;
using LinearDiff3DGame.MaxStableBridge.Input;
using LinearDiff3DGame.MaxStableBridge.Tools;

namespace LinearDiff3DGame.MaxStableBridge.Gamers
{
    public class GamerInitData
    {
        public GamerInitData(GamerParams gamerParams, Double deltaT, ApproxComp approxComp)
        {
            Int32 matrixRowCount = gamerParams.Matrix.RowCount;
            Int32 matrixColumnCount = gamerParams.Matrix.ColumnCount;
            Double[] matrixRawData = StringConvertHelper.ToDoubleArray(gamerParams.Matrix.RawData,
                                                                       CultureInfo.InvariantCulture);
            Matrix = new MatrixFactory().CreateFromRawData(matrixRowCount,
                                                           matrixColumnCount,
                                                           matrixRawData);
            MaxSection = gamerParams.MaxSection;
            MinSection = gamerParams.MinSection;
            DeltaT = deltaT;
            ApproxComp = approxComp;
        }

        public Matrix Matrix { get; private set; }
        public Double MaxSection { get; private set; }
        public Double MinSection { get; private set; }
        public Double DeltaT { get; private set; }
        public ApproxComp ApproxComp { get; private set; }
    }
}