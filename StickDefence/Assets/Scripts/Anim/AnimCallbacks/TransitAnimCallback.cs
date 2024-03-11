using System;
using UnityEngine;

namespace AnimationsTools.AnimCallbacks
{
    [RequireComponent(typeof(Animator))]
    public class TransitAnimCallback : MonoBehaviour
    {
        public event System.Action TransitAnimeCallBacked;

        public virtual void OnTransitionCallback()
        {
            TransitAnimeCallBacked?.Invoke();
        } 
    }
}