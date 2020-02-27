using System;
using OpenGLControlTest;

namespace LinearDiff3DGame.OpenGLVisualizerTest.VisualisationHelpers
{
    internal class ViewPointManager
    {
        public ViewPointManager()
            : this(DefaultAngleOX,
                   DefaultAngleOY,
                   DefaultDistanceOZ,
                   DefaultDeltaOX,
                   DefaultDeltaOY,
                   DefaultDeltaOZ)
        {
        }

        public ViewPointManager(Double angleOX,
                                Double angleOY,
                                Double distanceOZ,
                                Double deltaOX,
                                Double deltaOY,
                                Double deltaOZ)
        {
            InitialAngleOX = AngleOX = angleOX;
            InitialAngleOY = AngleOY = angleOY;
            InitialDistanceOZ = DistanceOZ = distanceOZ;
            DeltaOX = deltaOX;
            DeltaOY = deltaOY;
            DeltaOZ = deltaOZ;
        }

        public void ApplyViewPoint()
        {
            OpenGLControl.glTranslated(0.0, 0.0, -DistanceOZ);
            OpenGLControl.glRotated(AngleOY, 0.0, 1.0, 0.0);
            OpenGLControl.glRotated(AngleOX, 1.0, 0.0, 0.0);
        }

        public void RotateOX(SByte rotateSign)
        {
            AngleOX += DeltaOX * rotateSign;
            if(AngleOX > 360) AngleOX -= 360;
            if(AngleOX < -360) AngleOX += 360;
        }

        public void RotateOY(SByte rotateSign)
        {
            AngleOY += DeltaOY * rotateSign;
            if(AngleOY > 360) AngleOY -= 360;
            if(AngleOY < -360) AngleOY += 360;
        }

        public void MoveOZ(SByte moveSign)
        {
            DistanceOZ += DeltaOZ * moveSign;
        }

        public void ResetViewPoint()
        {
            AngleOX = InitialAngleOX;
            AngleOY = InitialAngleOY;
            DistanceOZ = InitialDistanceOZ;
        }

        public Double AngleOX { get; set; }
        public Double AngleOY { get; set; }
        public Double DistanceOZ { get; set; }
        public Double DeltaOX { get; private set; }
        public Double DeltaOY { get; private set; }
        public Double DeltaOZ { get; private set; }

        public const Double DefaultAngleOX = 0;
        public const Double DefaultAngleOY = 0;
        public const Double DefaultDistanceOZ = 20;
        public const Double DefaultDeltaOX = 5;
        public const Double DefaultDeltaOY = 5;
        public const Double DefaultDeltaOZ = 1;

        private readonly Double InitialAngleOX;
        private readonly Double InitialAngleOY;
        private readonly Double InitialDistanceOZ;
    }
}