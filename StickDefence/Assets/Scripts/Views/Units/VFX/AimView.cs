using Models.Controllers;
using UnityEngine;
using Views.Health;

namespace Views.Units.VFX
{
    public class AimView : MonoBehaviour
    {
        [SerializeField] private GameObject _aimSprite;
        public void ActivateAimSprite()
        {
            _aimSprite.SetActive(true);
        }

        public void DeactivateAimSprite()
        {
            _aimSprite.SetActive(false);
        }
    }
}