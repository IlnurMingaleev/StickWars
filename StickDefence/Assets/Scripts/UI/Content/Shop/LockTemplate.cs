using TMPro;
using UnityEngine;

namespace UI.Content.Shop
{
    public class LockTemplate:MonoBehaviour
    {
        [SerializeField] private TMP_Text _lockLabel;
        public TMP_Text LockLabel => _lockLabel;

    }
}