using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple cache for strings and their Animator hashes.
/// </summary>
public class StringCache
{
    Dictionary<string, int> hashByString = new Dictionary<string, int>();
    Dictionary<int, string> stringByHash = new Dictionary<int, string>();

    /// <summary>
    /// Adds a new string to the cache (or ignores duplicates).
    /// </summary>
    public int Add(string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;

        int hash = Animator.StringToHash(str);

        // if we've already seen the same string, nothing to do
        if (hashByString.TryGetValue(str, out var existing) && existing == hash)
            return hash;

        // collision check (very unlikely but good to warn)
        if (stringByHash.TryGetValue(hash, out var other) && other != str)
            Debug.LogError($"StringCache - hash collision: \"{str}\" vs \"{other}\" → {hash}");

        hashByString[str] = hash;
        stringByHash[hash] = str;

        return hash;
    }

    /// <summary>
    /// Returns the hash for a string if it was previously added,
    /// otherwise warns and returns zero.
    /// </summary>
    public bool TryGetHash(string s, out int hash)
    {
        hash = 0;

        if (string.IsNullOrEmpty(s))
            return false;

        if (hashByString.TryGetValue(s, out var h))
        {
            hash = h;
            return true;
        }

        Debug.LogWarning($"StringCache - \"{s}\" was not cached. Returning 0.");

        return false;
    }

    /// <summary>
    /// Returns the original string for a hash if it was previously added,
    /// otherwise warns and returns null.
    /// </summary>
    public bool TryGetString(int hash, out string str)
    {
        str = null;

        if (stringByHash.TryGetValue(hash, out var s))
        {
            str = s;
            return true;
        }

        Debug.LogWarning($"StringCache - hash {hash} was not cached. Returning null.");

        return false;
    }
}
