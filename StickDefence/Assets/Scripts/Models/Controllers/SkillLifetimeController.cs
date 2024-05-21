using System;
using System.Collections.Generic;
using Enums;
using Models.Controllers.Skills;
using Models.Player;
using UniRx;
using UnityEngine;
using VContainer;

namespace Models.Controllers
{
    public class SkillLifetimeController : MonoBehaviour
    {
        [SerializeField] private Skill[] _skills;
        private ReactiveProperty<bool> _aiming = new ReactiveProperty<bool>(false);
        public IReadOnlyReactiveProperty<bool> Aiming => _aiming; // Track if we are in aiming mode.
        public BoxCollider2D restrictedAreaCollider; // Assign the BoxCollider2D from the restricted area GameObject.
        private Camera _cameraMain;
        public SkillTypesEnum _skillType = SkillTypesEnum.None;
        public Skill _currentSkill;
        public Dictionary<SkillTypesEnum, Skill> SkillDictioary = new Dictionary<SkillTypesEnum, Skill>();
        [Inject] private IPlayer _player;
        private CompositeDisposable _aimDisposable = new CompositeDisposable();

        private void Start()
        {
            _cameraMain = Camera.main;
            SkillDictioary.Clear();
            SkillDictioary.Add(SkillTypesEnum.Grenade, _skills[0]);
            SkillDictioary.Add(SkillTypesEnum.Rocket, _skills[1]);
            SkillDictioary.Add(SkillTypesEnum.Gas, _skills[2]);
           
        }
        
        private void OnAimUpdate()
        {
            // Convert mouse position to a ray from camera to mouse.
            Ray ray = _cameraMain.ScreenPointToRay(UnityEngine.Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null && hit.collider == restrictedAreaCollider)
            {
                // Move aim sprite to mouse position if it is within the restricted area.
                Vector3 mousePosition = hit.point;
                mousePosition.z = 0; // Adjust for 2D.
                _currentSkill.AimView.position = mousePosition;
            }
        }

        public void OnPointerUp()
        {
            if (_aiming.Value == true)
            {
                foreach(Skill skill in _skills)
                {
                    skill.AimView.gameObject.SetActive(false);
                }
                LaunchMissile(_cameraMain.ScreenToWorldPoint(UnityEngine.Input.mousePosition));
                _aimDisposable.Clear();
                _aiming.Value = false;
            }

          
          // Exit aiming mode.
        }

        public void StartAiming(SkillTypesEnum skillTypesEnum)
        {
            foreach(Skill skill in _skills)
            {
                skill.AimView.gameObject.SetActive(false);
            }
            _skillType = skillTypesEnum;
            _currentSkill = SkillDictioary[_skillType];
            _currentSkill.AimView.transform.position = _cameraMain.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            _currentSkill.AimView.gameObject.SetActive(true);
            _aiming.Value = true;
            Observable.EveryUpdate().Subscribe(_ => OnAimUpdate()).AddTo(_aimDisposable);

        }

        void LaunchMissile(Vector3 mousePosition)
        {
            _currentSkill.LaunchMissile(mousePosition);
        }

        private void OnDisable()
        {
            _aimDisposable.Clear();
        }
    }
}