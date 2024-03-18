using System;
using UnityEngine;

namespace Views.Units.Animations
{
    public class UnitAnimationCallbacks : MonoBehaviour
    {
        public event Action AttackAction;
        public event Action StartCooldownAttackAction;
        
        public void OnAttackAnim()
        {
            AttackAction?.Invoke();
        }
        
        public void OnStartCooldownAttackAnim()
        {
            StartCooldownAttackAction?.Invoke();
        }
    }
}