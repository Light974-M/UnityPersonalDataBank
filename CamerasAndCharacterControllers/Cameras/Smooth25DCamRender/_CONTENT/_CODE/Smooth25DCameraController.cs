using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smooth25DCameraController : MonoBehaviour
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

    #region AwakesAndStarts
    private void Awake()
    {
        if (cameraList.Count == 0)
            cameraList.Add(FindObjectOfType<Camera>());
    } 
    #endregion

    #region Updates
    private void Update()
    {
        CameraMove();
    }
    #endregion

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
