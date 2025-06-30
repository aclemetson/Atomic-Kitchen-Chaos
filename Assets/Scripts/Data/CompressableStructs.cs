using System;
using UnityEngine;

namespace AtomicKitchenChaos.Data
{
    public class CompressableStructs
    {
        [Serializable]
        public struct CompressableVector3 {
            public float x, y, z;

            public CompressableVector3(Vector3 v) {
                x = v.x;
                y = v.y;
                z = v.z;
            }

            public Vector3 ToVector3() => new Vector3(x, y, z);
        }
    }
}
