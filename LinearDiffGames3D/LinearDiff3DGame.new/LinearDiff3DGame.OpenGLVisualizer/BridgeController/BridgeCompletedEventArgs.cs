using System;

namespace LinearDiff3DGame.OpenGLVisualizer.BridgeController
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