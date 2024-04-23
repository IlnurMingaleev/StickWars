using Models.Battle;
using Models.Battle.Boosters;
using Models.Merge;
using Tools.GameTools;
using UnityEditorInternal;
using UnityEngine;

namespace Models.Controllers
{
    public class SceneInstances:Singleton<SceneInstances>
    {
        [SerializeField] private MergeController _mergeController;
        [SerializeField] private PlayerFortressInstantiate _plaerBuilder;
        [SerializeField] private AimController _aimController;
        [SerializeField] private BoosterManager _boosterManager;
        public PlayerFortressInstantiate PlayerBuilder=> _plaerBuilder;
        public AimController AimController => _aimController;
        public BoosterManager BoosterManager => _boosterManager;
        public MergeController MergeController => _mergeController;
       
       
    }
    
}