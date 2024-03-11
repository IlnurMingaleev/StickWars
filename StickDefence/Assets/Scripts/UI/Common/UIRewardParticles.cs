using UnityEngine;

namespace UI.Common
{
    public class UIRewardParticles : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particlesStreached;
        [SerializeField] private ParticleSystem _particlesSphere;
      //  [SerializeField] private RewardType _type = RewardType.None;

        // private void Start()
        // {
        //     if (_type != RewardType.None) UpdateColors(_type);
        // }

        // private void UpdateColors(RewardParticlesConstants.Colors colors)
        // {
        //     ParticleSystem.ColorOverLifetimeModule colorModuleParticlesStreached = _particlesStreached.colorOverLifetime;
        //     ParticleSystem.ColorOverLifetimeModule colorModuleParticlesSphere = _particlesSphere.colorOverLifetime;
        //
        //     colorModuleParticlesStreached.color = colors.StreachedParticlesGradient;
        //     colorModuleParticlesSphere.color = colors.SphereParticlesGradient;
        // }
        //
        // public void UpdateColors(RewardType rewardType)
        // {
        //     UpdateColors(RewardParticlesConstants.GetColors(rewardType));
        // }
    }
}