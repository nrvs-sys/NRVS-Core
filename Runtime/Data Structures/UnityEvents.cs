// jlink, 1/19/2019 11:13:38 AM

using System;
using UnityEngine;
using UnityEngine.Events;


[Serializable]
public class UnityEventInt : UnityEvent<int> { }

[Serializable]
public class UnityEventUInt : UnityEvent<uint> { }

[Serializable]
public class UnityEventLong : UnityEvent<long> { }

[Serializable]
public class UnityEventBool : UnityEvent<bool> { }

[Serializable]
public class UnityEventFloat : UnityEvent<float> { }

[Serializable]
public class UnityEventString : UnityEvent<string> { }

[Serializable]
public class DamageEvent : UnityEvent<DamageInfo> { }

[Serializable]
public class GameObjectEvent : UnityEvent<GameObject> { }

[Serializable]
public class TransformEvent : UnityEvent<Transform> { }

[Serializable]
public class PointEvent : UnityEvent<GameObject, Vector3> { }

[Serializable]
public class ContactEvent : UnityEvent<ContactInfo> { }

[Serializable]
public class InteractionEvent : UnityEvent<InteractionInfo> { }

/// <summary>
/// Triggering collider, Triggered collider
/// </summary>
[Serializable]
public class TriggerEvent : UnityEvent<Collider, Collider> { }
