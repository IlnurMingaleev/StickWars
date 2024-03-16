using System.Collections.Generic;
using UnityEngine;

namespace Models.Merge
{
    [CreateAssetMenu]
    public class GameResources : ScriptableObject 
    {
        public List<GameObject> items = new List<GameObject>();
    }
}