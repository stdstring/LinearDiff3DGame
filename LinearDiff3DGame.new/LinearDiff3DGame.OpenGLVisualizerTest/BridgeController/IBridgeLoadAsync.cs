using System;
using System.IO;

namespace LinearDiff3DGame.OpenGLVisualizerTest.BridgeController
{
    internal interface IBridgeLoadAsync : IBridgeControllerAsync
    {
        Int32 Prepare(Stream dataStream);
    }
}