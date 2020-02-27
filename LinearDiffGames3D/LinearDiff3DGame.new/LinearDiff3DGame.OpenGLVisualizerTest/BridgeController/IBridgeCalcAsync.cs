using System;

namespace LinearDiff3DGame.OpenGLVisualizerTest.BridgeController
{
    internal interface IBridgeCalcAsync : IBridgeControllerAsync
    {
        Int32 PrepareCalculation(String inputDataFile, Double finishTime);
    }
}