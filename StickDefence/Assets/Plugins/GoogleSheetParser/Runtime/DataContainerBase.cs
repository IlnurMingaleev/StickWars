using System.Collections.Generic;
using UnityEngine;

namespace NorskaLib.GoogleSheetsDatabase
{
    public abstract class DataContainerBase : ScriptableObject
    {
        [SerializeField] [HideInInspector]
        public string documentID;
    }
}