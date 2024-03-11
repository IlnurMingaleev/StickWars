using System.Collections;
using UnityEngine;

namespace Common
{
    public class LifeTimer : MonoBehaviour
    {
        [SerializeField] private float _timer;

        private void Start()
        {
            StartCoroutine(StartTimer());
        }

        private IEnumerator StartTimer()
        {
            yield return new WaitForSeconds(_timer);
            Destroy(gameObject);
        }
    }
}