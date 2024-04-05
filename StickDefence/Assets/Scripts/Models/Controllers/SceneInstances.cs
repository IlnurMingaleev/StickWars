using Models.Battle;
using Tools.GameTools;
using UnityEngine;

namespace Models.Controllers
{
    public class SceneInstances: Singleton<SceneInstances>
    {
        [SerializeField] private PlayerFortressInstantiate _plaerBuilder;
        [SerializeField] private AimController _aimController;
        public PlayerFortressInstantiate PlayerBuilder=> _plaerBuilder;
        public AimController AimController => _aimController;
    }
}