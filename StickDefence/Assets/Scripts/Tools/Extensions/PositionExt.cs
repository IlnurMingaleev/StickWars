
using UnityEngine;

namespace Tools.Extensions
{
    public static class PositionExt
    {
        public static Vector3 CalculatePredictCollision(this Vector3 positionOrigin, float speedOrigin, Vector3 positionTarget, float speedTarget)
        {
            if (speedTarget == 0)
            {
                return positionTarget;
            }

            Vector3 velocityOrigin = (positionTarget - positionOrigin).normalized * speedOrigin;
            Vector3 velocityTarget = (positionOrigin - positionTarget).normalized * speedTarget;
            
            float timeToCollision = Vector3.Distance(positionOrigin, positionTarget) / (speedOrigin + speedTarget);
            Vector3 collisionPoint = velocityTarget + velocityTarget * timeToCollision;
            collisionPoint *= -1;

            Debug.Log("asdf " + collisionPoint + " " + positionOrigin + " " + speedOrigin + " " + positionTarget + " " +
                      speedTarget);
            return collisionPoint;
        }
    }
}