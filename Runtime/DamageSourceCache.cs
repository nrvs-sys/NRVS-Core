using System.Collections.Generic;
using UnityAtoms.BaseAtoms;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class DamageSourceCache : MonoBehaviour
{
    [SerializeField]
    StringConstantList damageSources;

    StringCache stringCache;

    // map from hash → StringConstant for the reverse lookup
    Dictionary<int, StringConstant> scByHash;

    void Awake()
    {
        stringCache = new StringCache();
        scByHash = new Dictionary<int, StringConstant>(damageSources.list.Count);

        foreach (var sc in damageSources.list)
        {
            if (sc == null || string.IsNullOrEmpty(sc.Value))
                continue;

            // feed the generic cache
            var hash = stringCache.Add(sc.Value);

            // and store in the StringConstant-specific reverse map
            scByHash[hash] = sc;
        }

        Ref.Register<DamageSourceCache>(this);
    }

    void OnDestroy()
    {
        Ref.Unregister<DamageSourceCache>(this);
    }

    /// <summary>
    /// Given a StringConstants, return its hash (0 if not pre-cached).
    /// </summary>
    public bool TryGetHash(StringConstant damageSource, out int hash)
    {
        hash = 0;

        if (damageSource == null)
            return false;

        if (stringCache.TryGetHash(damageSource.Value, out hash))
            return true;

        Debug.LogWarning($"DamageSourceCache - \"{damageSource.Value}\" ({damageSource.name}) was not cached. Returning 0.");

        return false;
    }

    /// <summary>
    /// Given a hash, return the original StringConstant (or null + warning).
    /// </summary>
    public bool TryGetByHash(int hash, out StringConstant stringConstant)
    {
        stringConstant = null;

        if (scByHash.TryGetValue(hash, out var sc))
        {
            stringConstant = sc;
            return true;
        }

        Debug.LogWarning($"DamageSourceCache - Hash {hash} was not cached. Returning null.");

        return false;
    }
}
