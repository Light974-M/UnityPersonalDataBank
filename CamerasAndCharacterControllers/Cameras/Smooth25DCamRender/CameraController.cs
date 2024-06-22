using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.CamerasAndCharacterControllers.Cameras.Smooth25DCameraController
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [AddComponentMenu("UPDB/CamerasAndCharacterControllers/Cameras/Smooth25DCameraController/2.5D Camera Controller")]
    public class CameraController : UPDBBehaviour
    {
        #region SerializedVariable

        #region Camera

        [Header(""), Header("CAMERA"), SerializeField, Tooltip("list of camera detected by game")]
        private List<Camera> cameraList;

        [SerializeField, Tooltip("Camera Offset (0 = center)")]
        private Vector3 cameraOffset;

        [SerializeField, Tooltip("Camera speed to center")]
        private float cameraDragSpeed = 4;

        [SerializeField, Tooltip("camera angle clamp multiplier(this is made for reset cam speed after changing maxAngleSpeed)")]
        private float camMaxAngleMultiplier = 1;

        [SerializeField, Tooltip("does Camera look at player")]
        private bool followPlayer;

        #endregion

        #endregion

        #region PrivateVariable

        #region Camera

        #endregion

        #endregion

        private void Awake()
        {
            if (cameraList.Count == 0)
                cameraList.Add(FindObjectOfType<Camera>());
        }

        private void Update()
        {
            CameraMove();
        }

        private void CameraMove()
        {
            int i = 0;

            #region Calculs
            Vector3 positionToReach = new Vector3(transform.position.x + cameraOffset.x, transform.position.y + cameraOffset.y, cameraList[i].transform.position.z + cameraOffset.z);
            float distanceMultiplier = Vector3.Distance(positionToReach, cameraList[i].transform.position) * camMaxAngleMultiplier;
            Vector3 direction = positionToReach - cameraList[i].transform.position;
            if (direction.magnitude > 1)
                direction = direction.normalized;

            Vector3 directionToReach = ((direction * Time.deltaTime) * cameraDragSpeed) * distanceMultiplier;
            #endregion

            cameraList[i].transform.Translate(directionToReach, Space.World);

            if (followPlayer)
                cameraList[i].transform.LookAt(transform.position);
        }
    } 
}
