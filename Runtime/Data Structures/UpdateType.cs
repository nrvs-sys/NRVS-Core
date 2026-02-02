using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    [System.Flags]
    public enum UpdateType
    {
        Update = (1 << 0),
        FixedUpdate = (1 << 1),
        LateUpdate = (1 << 2),
        PreTick = (1 << 3),
        Tick = (1 << 4),
        PostTick = (1 << 5),
        PreReconcile = (1 << 6),
        PostReconcile = (1 << 7)
    }
