using System;

namespace LinearDiff3DGame.OpenGLVisualizerTest.BridgeController
{
    public class BridgeCompletedEventArgs : EventArgs
    {
        public BridgeCompletedEventArgs(Boolean success)
        {
            Success = success;
        }

        public Boolean Success { get; private set; }
    }
}