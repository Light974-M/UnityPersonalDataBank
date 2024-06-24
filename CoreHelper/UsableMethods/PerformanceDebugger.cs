using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.CoreHelper.UsableMethods
{
    public class PerformanceDebugger : MonoBehaviour
    {
        [SerializeField]
        private float _averageFpsUpdateTime = 1;

        [SerializeField]
        private bool _reinitCount = false;

        private float _timer = 0;
        private List<float> _fpsList = new List<float>();

        // Update is called once per frame
        void Update()
        {
            if (_reinitCount)
            {
                _timer = 0;
                _fpsList.Clear();
                _reinitCount = false;
            }

            if (_timer >= _averageFpsUpdateTime)
            {
                float averageFps = 0;

                foreach (float fps in _fpsList)
                    averageFps += fps;

                averageFps /= _fpsList.Count;

                Debug.Log(averageFps);

                _fpsList.Clear();
                _timer = 0;
            }

            if (Time.deltaTime != 0)
                _fpsList.Add(1 / Time.deltaTime);

            _timer += Time.deltaTime;
        }
    } 
}
