using System;
using System.IO;

namespace LinearDiff3DGame.OpenGLVisualizer.BridgeController
{
    internal interface IBridgeLoadAsync : IBridgeControllerAsync
    {
        Int32 Prepare(Stream dataStream);
    }
}