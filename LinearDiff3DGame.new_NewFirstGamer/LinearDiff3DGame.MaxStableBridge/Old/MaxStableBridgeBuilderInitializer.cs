using System;
using System.Collections.Generic;
using LinearDiff3DGame.AdvMath;
using LinearDiff3DGame.Geometry3D.Common;

namespace LinearDiff3DGame.MaxStableBridge.Old
{
    internal interface IMaxStableBridgeBuilderInitializer
    {
        Object GetDataByKey(String key);
    }

    internal class MaxStableBridgeBuilderInitializer : IMaxStableBridgeBuilderInitializer
    {
        public Object GetDataByKey(String key)
        {
            Object data;
            dataDictionary.TryGetValue(key, out data);
            return data;
        }

        public static IMaxStableBridgeBuilderInitializer CreateMaterialPointData()
        {
            const Double k = 0;
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = 0;
            matrixA[1, 2] = 1;
            matrixA[1, 3] = 0;
            matrixA[2, 1] = k;
            matrixA[2, 2] = 0;
            matrixA[2, 3] = 0;
            matrixA[3, 1] = 0;
            matrixA[3, 2] = 0;
            matrixA[3, 3] = 0;
            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = 0;
            matrixB[2, 1] = 1;
            matrixB[3, 1] = 0;
            Matrix matrixC = new Matrix(3, 1);
            matrixC[1, 1] = 1;
            matrixC[2, 1] = 0;
            matrixC[3, 1] = 0;

            const Double deltaT = 0.1;

            const Double mpMax = 1.0;
            const Double mpMin = -1.0;
            const Double mqMax = 1.0;
            const Double mqMin = -1.0;

            const Double maxCValue = 2.5;
            const Int32 vertexCount = 6;
            Point3D[] terminalSetVertexes = new Point3D[vertexCount];
            terminalSetVertexes[0] = new Point3D(0, 0, maxCValue);
            terminalSetVertexes[1] = new Point3D(maxCValue, maxCValue, 0);
            terminalSetVertexes[2] = new Point3D(-maxCValue, maxCValue, 0);
            terminalSetVertexes[3] = new Point3D(-maxCValue, -maxCValue, 0);
            terminalSetVertexes[4] = new Point3D(maxCValue, -maxCValue, 0);
            terminalSetVertexes[5] = new Point3D(0, 0, -maxCValue);

            Int32[] y1y2y3Indexes = new[] {1, 2, 3};

            const Double scalingMaxMinThresholdValue = 2.0;

            MaxStableBridgeBuilderInitializer initializer = new MaxStableBridgeBuilderInitializer();
            initializer.dataDictionary.Add("MatrixA", matrixA);
            initializer.dataDictionary.Add("MatrixB", matrixB);
            initializer.dataDictionary.Add("MatrixC", matrixC);
            initializer.dataDictionary.Add("DeltaT", deltaT);
            initializer.dataDictionary.Add("MpMax", mpMax);
            initializer.dataDictionary.Add("MpMin", mpMin);
            initializer.dataDictionary.Add("MqMax", mqMax);
            initializer.dataDictionary.Add("MqMin", mqMin);
            initializer.dataDictionary.Add("TerminalSet", terminalSetVertexes);
            initializer.dataDictionary.Add("Y1Y2Y3Indexes", y1y2y3Indexes);
            initializer.dataDictionary.Add("ScalingMaxMinThresholdValue", scalingMaxMinThresholdValue);

            return initializer;
        }

        public static IMaxStableBridgeBuilderInitializer CreateOscillatorData()
        {
            const Double k = -1;
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = 0;
            matrixA[1, 2] = 1;
            matrixA[1, 3] = 0;
            matrixA[2, 1] = k;
            matrixA[2, 2] = 0;
            matrixA[2, 3] = 0;
            matrixA[3, 1] = 0;
            matrixA[3, 2] = 0;
            matrixA[3, 3] = 0;
            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = 0;
            matrixB[2, 1] = 1;
            matrixB[3, 1] = 0;
            Matrix matrixC = new Matrix(3, 1);
            matrixC[1, 1] = 1;
            matrixC[2, 1] = 0;
            matrixC[3, 1] = 0;

            const Double deltaT = 0.1;

            const Double mpMax = 1.0;
            const Double mpMin = -1.0;
            const Double mqMax = 1.0;
            const Double mqMin = -1.0;

            const Double maxCValue = 2.5;
            const Int32 vertexCount = 6;
            Point3D[] terminalSetVertexes = new Point3D[vertexCount];
            terminalSetVertexes[0] = new Point3D(0, 0, maxCValue);
            terminalSetVertexes[1] = new Point3D(maxCValue, maxCValue, 0);
            terminalSetVertexes[2] = new Point3D(-maxCValue, maxCValue, 0);
            terminalSetVertexes[3] = new Point3D(-maxCValue, -maxCValue, 0);
            terminalSetVertexes[4] = new Point3D(maxCValue, -maxCValue, 0);
            terminalSetVertexes[5] = new Point3D(0, 0, -maxCValue);

            Int32[] y1y2y3Indexes = new[] {1, 2, 3};

            const Double scalingMaxMinThresholdValue = 2.0;

            MaxStableBridgeBuilderInitializer initializer = new MaxStableBridgeBuilderInitializer();
            initializer.dataDictionary.Add("MatrixA", matrixA);
            initializer.dataDictionary.Add("MatrixB", matrixB);
            initializer.dataDictionary.Add("MatrixC", matrixC);
            initializer.dataDictionary.Add("DeltaT", deltaT);
            initializer.dataDictionary.Add("MpMax", mpMax);
            initializer.dataDictionary.Add("MpMin", mpMin);
            initializer.dataDictionary.Add("MqMax", mqMax);
            initializer.dataDictionary.Add("MqMin", mqMin);
            initializer.dataDictionary.Add("TerminalSet", terminalSetVertexes);
            initializer.dataDictionary.Add("Y1Y2Y3Indexes", y1y2y3Indexes);
            initializer.dataDictionary.Add("ScalingMaxMinThresholdValue", scalingMaxMinThresholdValue);

            return initializer;
        }

        public static IMaxStableBridgeBuilderInitializer CreateKnobData()
        {
            Matrix matrixA = new Matrix(3, 3);
            matrixA[1, 1] = 1;
            matrixA[1, 2] = 2;
            matrixA[1, 3] = 0;
            matrixA[2, 1] = 0;
            matrixA[2, 2] = 1;
            matrixA[2, 3] = 0;
            matrixA[3, 1] = 0;
            matrixA[3, 2] = 0;
            matrixA[3, 3] = 0;
            Matrix matrixB = new Matrix(3, 1);
            matrixB[1, 1] = 0;
            matrixB[2, 1] = 1;
            matrixB[3, 1] = 0;
            Matrix matrixC = new Matrix(3, 1);
            matrixC[1, 1] = 1;
            matrixC[2, 1] = 0;
            matrixC[3, 1] = 0;

            const Double deltaT = 0.1;

            const Double mpMax = 1.0;
            const Double mpMin = -1.0;
            const Double mqMax = 0.9;
            const Double mqMin = -0.9;

            const Double maxCValue = 7.0;
            const Int32 vertexCount = 40;
            List<Point3D> vertexList = new List<Point3D>(vertexCount + 2);
            for (Int32 pointIndex = 0; pointIndex < vertexCount; ++pointIndex)
            {
                Double x = maxCValue*Math.Cos(pointIndex*2*Math.PI/vertexCount);
                Double y = maxCValue*Math.Sin(pointIndex*2*Math.PI/vertexCount);
                vertexList.Add(new Point3D(x, y, 0));
            }
            vertexList.Add(new Point3D(0, 0, maxCValue));
            vertexList.Add(new Point3D(0, 0, -maxCValue));
            Point3D[] terminalSetVertexes = vertexList.ToArray();

            Int32[] y1y2y3Indexes = new[] {1, 2, 3};

            const Double scalingMaxMinThresholdValue = 2.0;

            MaxStableBridgeBuilderInitializer initializer = new MaxStableBridgeBuilderInitializer();
            initializer.dataDictionary.Add("MatrixA", matrixA);
            initializer.dataDictionary.Add("MatrixB", matrixB);
            initializer.dataDictionary.Add("MatrixC", matrixC);
            initializer.dataDictionary.Add("DeltaT", deltaT);
            initializer.dataDictionary.Add("MpMax", mpMax);
            initializer.dataDictionary.Add("MpMin", mpMin);
            initializer.dataDictionary.Add("MqMax", mqMax);
            initializer.dataDictionary.Add("MqMin", mqMin);
            initializer.dataDictionary.Add("TerminalSet", terminalSetVertexes);
            initializer.dataDictionary.Add("Y1Y2Y3Indexes", y1y2y3Indexes);
            initializer.dataDictionary.Add("ScalingMaxMinThresholdValue", scalingMaxMinThresholdValue);

            return initializer;
        }

        //public static IMaxStableBridgeBuilderInitializer CreateAirplaneTaskData()
        //{
        //    Matrix matrixA = new Matrix(4, 4);
        //    matrixA[1, 1] = 0;
        //    matrixA[1, 2] = 1;
        //    matrixA[1, 3] = 0;
        //    matrixA[1, 4] = 0;
        //    matrixA[2, 1] = 0;
        //    matrixA[2, 2] = -0.032;
        //    matrixA[2, 3] = 0;
        //    matrixA[2, 4] = -0.135;
        //    matrixA[3, 1] = 0;
        //    matrixA[3, 2] = 0;
        //    matrixA[3, 3] = 0;
        //    matrixA[3, 4] = 1;
        //    matrixA[4, 1] = 0;
        //    matrixA[4, 2] = 0.27;
        //    matrixA[4, 3] = 0;
        //    matrixA[4, 4] = -0.014;
        //    Matrix matrixB = new Matrix(3, 1);
        //    matrixB[1, 1] = 0;
        //    matrixB[2, 1] = 1;
        //    matrixB[3, 1] = 0;
        //    Matrix matrixC = new Matrix(3, 1);
        //    matrixC[1, 1] = 1;
        //    matrixC[2, 1] = 0;
        //    matrixC[3, 1] = 0;

        //    const Double deltaT = 0.1;

        //    const Double mpMax = 1.0;
        //    const Double mpMin = -1.0;
        //    const Double mqMax = 1.0;
        //    const Double mqMin = -1.0;

        //    const Double maxCValue = 2.5;
        //    const Int32 vertexCount = 6;
        //    Point3D[] terminalSetVertexes = new Point3D[vertexCount];
        //    terminalSetVertexes[0] = new Point3D(0, 0, maxCValue);
        //    terminalSetVertexes[1] = new Point3D(maxCValue, maxCValue, 0);
        //    terminalSetVertexes[2] = new Point3D(-maxCValue, maxCValue, 0);
        //    terminalSetVertexes[3] = new Point3D(-maxCValue, -maxCValue, 0);
        //    terminalSetVertexes[4] = new Point3D(maxCValue, -maxCValue, 0);
        //    terminalSetVertexes[5] = new Point3D(0, 0, -maxCValue);

        //    Int32[] y1y2y3Indexes = new[] { 1, 2, 3 };

        //    const Double scalingMaxMinThresholdValue = 2.0;

        //    MaxStableBridgeBuilderInitializer initializer = new MaxStableBridgeBuilderInitializer();
        //    initializer.dataDictionary.Add("MatrixA", matrixA);
        //    initializer.dataDictionary.Add("MatrixB", matrixB);
        //    initializer.dataDictionary.Add("MatrixC", matrixC);
        //    initializer.dataDictionary.Add("DeltaT", deltaT);
        //    initializer.dataDictionary.Add("MpMax", mpMax);
        //    initializer.dataDictionary.Add("MpMin", mpMin);
        //    initializer.dataDictionary.Add("MqMax", mqMax);
        //    initializer.dataDictionary.Add("MqMin", mqMin);
        //    initializer.dataDictionary.Add("TerminalSet", terminalSetVertexes);
        //    initializer.dataDictionary.Add("Y1Y2Y3Indexes", y1y2y3Indexes);
        //    initializer.dataDictionary.Add("ScalingMaxMinThresholdValue", scalingMaxMinThresholdValue);

        //    return initializer;
        //}

        private MaxStableBridgeBuilderInitializer()
        {
        }

        private readonly Dictionary<String, Object> dataDictionary = new Dictionary<String, Object>();
    }
}