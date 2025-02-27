using System;
using Timberborn.Coordinates;

namespace ChooChoo
{
    public static class OrientationExtensions
    {
        public static Direction2D ToDirection(this Orientation orientation)
        {
            switch (orientation)
            {
                case Orientation.Cw0:
                    return Direction2D.Up;
                case Orientation.Cw90:
                    return Direction2D.Right;
                case Orientation.Cw180:
                    return Direction2D.Down;
                case Orientation.Cw270:
                    return Direction2D.Left;
                default:
                    throw new ArgumentOutOfRangeException(nameof (orientation), orientation, null);
            }
        }
        
        public static Direction2D CorrectedTransform(
            this Orientation orientation,
            Direction2D direction2D)
        {
            switch (orientation)
            {
                case Orientation.Cw0:
                    return direction2D;
                case Orientation.Cw90:
                    return direction2D.CorrectedNext();
                case Orientation.Cw180:
                    return direction2D.CorrectedNext().CorrectedNext();
                case Orientation.Cw270:
                    return direction2D.CorrectedNext().CorrectedNext().CorrectedNext();
                default:
                    throw new ArgumentException(string.Format("Unexpected {0}: {1}", (object) "Orientation", (object) orientation));
            }
        }
    }
}
