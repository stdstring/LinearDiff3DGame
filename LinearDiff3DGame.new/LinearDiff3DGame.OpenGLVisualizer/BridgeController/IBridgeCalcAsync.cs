using System;

namespace LinearDiff3DGame.OpenGLVisualizer.BridgeController
{
    internal interface IBridgeCalcAsync : IBridgeControllerAsync
    {
        Int32 PrepareCalculation(String inputDataFile, Double finishTime);
    }
}