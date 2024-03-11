using UnityEngine;

namespace SO.Iaps
{
    [CreateAssetMenu(fileName = "RewardsConstSO", menuName = "MyAssets/Config/RewardsConstSO", order = 3)]
    public class RewardsConstSO : ScriptableObject
    {
        [field: SerializeField] public int MaxSortMergeBoardCount { get; private set; }
    }
}