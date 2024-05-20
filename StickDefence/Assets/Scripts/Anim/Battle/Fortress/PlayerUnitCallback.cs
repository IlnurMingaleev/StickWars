using System;
using UnityEngine;

namespace Anim.Battle.Fortress
{
    public class PlayerUnitCallback : MonoBehaviour
    {
        public event Action EndAttackAnim;

        public void OnEndLaunchAnim() => EndAttackAnim?.Invoke();
    }
}