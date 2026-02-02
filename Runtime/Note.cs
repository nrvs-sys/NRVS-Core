using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(fileName = "Note_ ", menuName = "Utilities/Note")]
    public class Note : ScriptableObject
    {
        [SerializeField]
        [TextArea(3, 6)]
        private string description;
    }
