using UnityEngine;

namespace UIMetaballs.Runtime
{
    public struct MetaballStruct
    {
        [ColorUsage(true, true)]
        public Color color;
        public Vector2 position;
        public Vector2 size;
        public float blending;
        public Vector4 roundness;
        public float angle;
        public int round;
        public float outlineWidth;
    }
}