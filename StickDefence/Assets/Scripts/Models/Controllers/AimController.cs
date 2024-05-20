using System.Collections.Generic;
using Enums;
using Models.Controllers.Skills;
using Models.Player;
using UnityEngine;
using VContainer;

namespace Models.Controllers
{
    public class AimController : MonoBehaviour
    {
        [SerializeField] private Skill[] _skills;
        public bool aiming = false; // Track if we are in aiming mode.
        public BoxCollider2D restrictedAreaCollider; // Assign the BoxCollider2D from the restricted area GameObject.
        public Camera _cameraMain;
        public SkillTypesEnum _skillType = SkillTypesEnum.None;
        public Skill _currentSkill;
        public Dictionary<SkillTypesEnum, Skill> SkillDictioary = new Dictionary<SkillTypesEnum, Skill>();
        [Inject] private IPlayer _player;

        private void Start()
        {
            _cameraMain = Camera.main;
            SkillDictioary.Clear();
            SkillDictioary.Add(SkillTypesEnum.Grenade, _skills[0]);
            SkillDictioary.Add(SkillTypesEnum.Rocket, _skills[1]);
            SkillDictioary.Add(SkillTypesEnum.Gas, _skills[2]);
           
        }

        private void Update()
        {
            if (aiming)
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

            // Check for left mouse button click to launch missile.
            if (aiming && UnityEngine.Input.GetMouseButtonDown(0))
            {
                
                LaunchMissile(_cameraMain.ScreenToWorldPoint(UnityEngine.Input.mousePosition));
                aiming = false; 
                _currentSkill.AimView.gameObject.SetActive(false);// Exit aiming mode.
            }
            if(aiming && UnityEngine.Input.GetMouseButton(1))
            {
                aiming = false;
                _currentSkill.AimView.gameObject.SetActive(false);
            }
        }

        public void StartAiming(SkillTypesEnum skillTypesEnum)
        {
            _skillType = skillTypesEnum;
            _currentSkill = SkillDictioary[_skillType];
            aiming = true;
            _currentSkill.AimView.transform.position = _cameraMain.ScreenToWorldPoint(UnityEngine.Input.mousePosition);
            _currentSkill.AimView.gameObject.SetActive(true);
        }

        void LaunchMissile(Vector3 mousePosition)
        {
            // Add missile launching animations or effects here.
            _currentSkill.LaunchMissile(mousePosition);
        }

       
    }
}