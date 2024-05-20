using Models.Battle;
using Models.Battle.Boosters;
using Models.Merge;
using Tools.GameTools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Models.Controllers
{
    public class SceneInstances : Singleton<SceneInstances>
    {
        [SerializeField] private MergeController _mergeController;
        [SerializeField] private PlayerFortressInstantiate _plaerBuilder;
        [FormerlySerializedAs("_aimController")] [SerializeField] private SkillLifetimeController skillLifetimeController;
        [SerializeField] private BoosterManager _boosterManager;
        public PlayerFortressInstantiate PlayerBuilder=> _plaerBuilder;
        public SkillLifetimeController SkillLifetimeController => skillLifetimeController;
        public BoosterManager BoosterManager => _boosterManager;
        public MergeController MergeController => _mergeController;
    }
}