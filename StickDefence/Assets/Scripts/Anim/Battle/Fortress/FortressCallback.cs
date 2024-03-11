using System;
using UnityEngine;

namespace Anim.Battle.Fortress
{
    public class FortressCallback : MonoBehaviour
    {
        public event Action EndLaunchAnim;

        public void OnEndLaunchAnim() => EndLaunchAnim?.Invoke();
    }
}