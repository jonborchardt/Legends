using System;

namespace Legends.Data
{
    [Serializable]
    public class ReplayFrame
    {
        public float Timestamp;
        public string AthleteId;
        public float[] Position;   // [x, y, z] — no UnityEngine.Vector3
        // No AnimState, no Rotation. Frames are positional snapshots only.
        // ReplayDirector derives facing from position delta.
        // Animation state comes from Timeline events at dispatch time.
    }
}
