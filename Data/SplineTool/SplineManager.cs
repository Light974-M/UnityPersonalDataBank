using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper;
using UPDB.CoreHelper.ExtensionMethods;
using UPDB.CoreHelper.UsableMethods;

namespace UPDB.Data.SplineTool
{
    /// <summary>
    /// manage spline system for every kind of object
    /// </summary>
    [AddComponentMenu(NamespaceID.UPDB + "/" + NamespaceID.Data + "/" + NamespaceID.SplineTool + "/Spline Manager"), ExecuteAlways]
    public class SplineManager : UPDBBehaviour
    {
        #region Serialized API

        /*****************************************************BASE REFERENCES**************************************************************/
        [Space, Header("BASE REFERENCES"), Space]

        [SerializeField, Tooltip("preset to apply for spline keys")]
        private SplinePreset _usedSpline;

        [Space]

        [SerializeField, Tooltip("list of all target affected by this spline")]
        private SplineTarget[] _targetList;


        /*****************************************************DEBUG**************************************************************/
        [Space, Header("DEBUG"), Space]

        [SerializeField, Tooltip("number of vertices in line drawn per meter")]
        private float _lineVertices = 4;

        [SerializeField, Tooltip("used to determine wich target will be used to draw spline")]
        private List<bool> _targetToDrawDebug;

        #endregion

        #region Public API

        public SplineTarget[] TargetList
        {
            get { return _targetList; }
            set { _targetList = value; }
        }

        public List<bool> TargetToDrawDebug
        {
            get
            {
                while (_targetToDrawDebug.Count < _targetList.Length)
                {
                    _targetToDrawDebug.Add(false);
                }
                while (_targetToDrawDebug.Count > _targetList.Length)
                {
                    _targetToDrawDebug.RemoveAt(_targetToDrawDebug.Count - 1);
                }
                return _targetToDrawDebug;
            }
        }

        #endregion


        /********************************************************BUILT-IN FUNCTIONS*************************************************************/

        /// <summary>
        /// Awake is called when scrit instance is loaded
        /// </summary>
        private void Awake()
        {
            if (_targetList != null && _targetList.Length != 0)
                foreach (SplineTarget target in _targetList)
                    target.ActiveKeyMemo = target.ActiveKey;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        private void Update()
        {
            if (_usedSpline)
            {
                _usedSpline.InitValuesAndCurves();

                if (_targetList != null && _targetList.Length != 0)
                {
                    for(int i = 0; i < _targetList.Length; i++)
                        if (_targetList[i].Target)
                            UpdateSplineTarget(ref _targetList[i]);
                        else
                            Debug.LogWarning("you have no target transform in your target component, add one before tool can calculate anything");
                }
                else
                    Debug.LogWarning("Warning : enter targets in order for tool to calculate positions and rotations");
            }
            else
                Debug.LogWarning("WARNING : ENTER A SPLINE PRESET IN ORDER TO USE TOOL");
        }

        /// <summary>
        /// OnDrawGizmosSelected is called at scene refresh, when inspector of class is selected
        /// </summary>
        protected override void OnDrawGizmosSelected()
        {
            base.OnDrawGizmosSelected();

            if (_targetList != null && _targetList.Length != 0)
                CallVisualDrawing();
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (_usedSpline)
                _usedSpline.CallGizmos(this);
        }


        /****************************************************CUSTOM FUNCTIONS*************************************************************/

        private void UpdateSplineTarget(ref SplineTarget target)
        {
            UpdateRotationList(ref target);

            if (target.Target)
            {
                if (Application.isPlaying)
                {
                    if (target.StartTimer >= target.StartTime)
                        UpdatePlayer(ref target);
                    else
                    {
                        target.StartTimer += Time.deltaTime;

                        if (target.TargetLinkedAnimation)
                            target.TargetLinkedAnimation.speed = 0;
                    }
                }

                for (int i = 0; i < _usedSpline.KeyPoints.Length; i++)
                    if (_usedSpline.KeyPoints[i].RotationOverwrite != RotationMode.FreeRotation)
                        OverwriteRotationKeysValues(ref target, i);

                RegisterPositionInformations(ref target);
                target.Target.position = GetTargetPos(target.TargetSplinePos);
                target.Target.rotation = GetTargetRot(target, target.TargetSplinePos);
                SetTargetOpacity(ref target);
            }
            else
                Debug.LogWarning("Warning : enter a target in slot in order for tool to calculate positions and rotations");
        }

        #region DEBUG

        public void CallVisualDrawing()
        {
            if (_usedSpline)
            {
                DrawVertices();

                if (_targetList != null && _targetList.Length != 0)
                    DrawRotations();

                DrawLines();
            }

            Gizmos.color = Color.white;
        }

        private void DrawVertices()
        {
            for (int i = 0; i < _usedSpline.KeyPoints.Length; i++)
            {
                if (i == 0)
                {
                    Gizmos.color = Color.red;
                    Vector3 pos = transform.TransformPoint(_usedSpline.KeyPoints[i].KeyPosition);
                    Gizmos.DrawSphere(pos, Vector3.Distance(Camera.current.transform.position, pos) * 0.04f);
                }
                else
                {
                    Gizmos.color = Color.green;
                    Vector3 pos = transform.TransformPoint(_usedSpline.KeyPoints[i].KeyPosition);
                    Gizmos.DrawSphere(pos, Vector3.Distance(Camera.current.transform.position, pos) * 0.015f);
                }
            }

            Gizmos.color = Color.white;
        }

        private void DrawRotations()
        {
            for (int i = 0; i < _usedSpline.KeyPoints.Length; i++)
            {
                if (_usedSpline.KeyPoints[i].RotationOverwrite == RotationMode.FollowRotation)
                {
                    for (int j = 0; j < _targetList.Length; j++)
                        if (TargetToDrawDebug[j])
                            DrawRotationGizmos(_usedSpline.KeyPoints[i].KeyPosition, _targetList[j].KeysRotationOverrideList[i], Color.white, Color.red, 1.5f);
                }
                else
                {
                    DrawRotationGizmos(_usedSpline.KeyPoints[i].KeyPosition, _targetList[0].KeysRotationOverrideList[i], Color.white, Color.red, 1.5f);
                }
            }

            if (_usedSpline.StartRotationOverwrite == RotationMode.FollowRotation)
            {
                for (int j = 0; j < _targetList.Length; j++)
                    if (TargetToDrawDebug[j])
                        DrawRotationGizmos(_usedSpline.KeyPoints[0].KeyPosition, _targetList[j].StartKeyRotationOverride, Color.yellow, Color.blue, 3);
            }
            else
            {
                DrawRotationGizmos(_usedSpline.KeyPoints[0].KeyPosition, _targetList[0].StartKeyRotationOverride, Color.yellow, Color.blue, 3);
            }
        }

        private void UpdateRotationList(ref SplineTarget target)
        {
            if (target.KeysRotationOverrideList == null || target.KeysRotationOverrideList.Length != _usedSpline.KeyPoints.Length)
                target.KeysRotationOverrideList = new Quaternion[_usedSpline.KeyPoints.Length];

            for (int i = 0; i < target.KeysRotationOverrideList.Length; i++)
                if (_usedSpline.KeyPoints[i].RotationOverwrite == RotationMode.FreeRotation)
                    target.KeysRotationOverrideList[i] = _usedSpline.KeyPoints[i].KeyRotation;

            if (_usedSpline.StartRotationOverwrite == RotationMode.FreeRotation)
                target.StartKeyRotationOverride = _usedSpline.StartRotation;
        }

        private void DrawRotationGizmos(Vector3 position, Quaternion rotation, Color southCol, Color northCol, float scale)
        {
            Vector3 pos = transform.TransformPoint(position);
            Quaternion rot = rotation;
            Vector3 rot2 = rot * new Vector3(transform.forward.x, transform.forward.y + 0.25f, transform.forward.z);
            Vector3 rot3 = rot * new Vector3(transform.forward.x, transform.forward.y - 0.25f, transform.forward.z);

            Gizmos.color = northCol;

            Gizmos.DrawRay(pos, (rot2) * scale);
            Gizmos.DrawLine(pos + (rot2 * scale), pos + ((rot * transform.forward) * (scale * 1.33333333f)));

            Gizmos.color = southCol;

            Gizmos.DrawRay(pos, (rot * transform.forward) * (scale * 1.33333333f));
            Gizmos.DrawRay(pos, (rot3) * scale);
            Gizmos.DrawLine(pos + (rot3 * scale), pos + ((rot * transform.forward) * (scale * 1.33333333f)));

            Gizmos.color = Color.white;
        }

        private void DrawLines()
        {
            for (int i = 0; i < _usedSpline.KeyPoints.Length - 1; i++)
            {
                float distanceLength = Vector3.Distance(_usedSpline.KeyPoints[i].KeyPosition, _usedSpline.KeyPoints[i + 1].KeyPosition);
                int numberOfVerticesToPut = Mathf.RoundToInt(_lineVertices * distanceLength);

                for (int j = 0; j < numberOfVerticesToPut; j++)
                {
                    float vertex1ScaledPos = Mathf.Lerp(i, i + 1, j / (float)numberOfVerticesToPut);
                    float vertex1UnscaledPos = vertex1ScaledPos / (float)(_usedSpline.KeyPoints.Length - 1);
                    float vertex2ScaledPos = Mathf.Lerp(i, i + 1, (j + 1) / (float)numberOfVerticesToPut);
                    float vertex2UnscaledPos = vertex2ScaledPos / (float)(_usedSpline.KeyPoints.Length - 1);


                    Gizmos.DrawLine(GetTargetPos(vertex1UnscaledPos), GetTargetPos(vertex2UnscaledPos));
                }
            }
        }

        #endregion

        #region SplinePrecalculation

        private void RegisterPositionInformations(ref SplineTarget target)
        {
            target.TargetSplinePosScaled = target.TargetSplinePos * (_usedSpline.KeyPoints.Length - 1);
            target.ActiveKey = Mathf.FloorToInt(target.TargetSplinePosScaled);
            target.TargetKeyPos = target.TargetSplinePosScaled - target.ActiveKey;
        }

        private Vector3 GetTargetPos(float targetSplinePos)
        {
            float targetSplinePosScaled = targetSplinePos * (_usedSpline.KeyPoints.Length - 1);
            int activeKey = Mathf.FloorToInt(targetSplinePosScaled);
            float targetKeyPos = targetSplinePosScaled - activeKey;

            Vector3 pos;

            if (targetSplinePosScaled < _usedSpline.KeyPoints.Length - 1)
            {
                Vector3 keyPos = _usedSpline.KeyPoints[activeKey].KeyPosition;
                Quaternion keyRot = Quaternion.LookRotation(transform.TransformPoint(_usedSpline.KeyPoints[activeKey + 1].KeyPosition) - transform.TransformPoint(_usedSpline.KeyPoints[activeKey].KeyPosition), Vector3.up);
                AnimationCurve Xcurve = _usedSpline.KeyPoints[activeKey].XCurve;
                AnimationCurve Ycurve = _usedSpline.KeyPoints[activeKey].YCurve;
                float scaleValue = Vector3.Distance(_usedSpline.KeyPoints[activeKey].KeyPosition, _usedSpline.KeyPoints[activeKey + 1].KeyPosition);

                Vector3 a = transform.TransformPoint(keyPos);
                Vector3 b = transform.TransformPoint(_usedSpline.KeyPoints[activeKey + 1].KeyPosition);

                Vector3 linearPos = Vector3.Lerp(a, b, targetKeyPos);
                Vector3 XOffset = keyRot.Right() * -(Xcurve.Evaluate(targetKeyPos) * scaleValue);
                Vector3 YOffset = keyRot.Up() * (Ycurve.Evaluate(targetKeyPos) * scaleValue);
                pos = linearPos + XOffset + YOffset;
            }
            else
            {
                pos = transform.TransformPoint(_usedSpline.KeyPoints[_usedSpline.KeyPoints.Length - 1].KeyPosition);
            }

            return pos;
        }

        private Quaternion GetTargetRot(SplineTarget target, float targetSplinePos)
        {
            float targetSplinePosScaled = targetSplinePos * (_usedSpline.KeyPoints.Length - 1);
            int activeKey = Mathf.FloorToInt(targetSplinePosScaled);
            float targetKeyPos = targetSplinePosScaled - activeKey;

            bool isFirstIndex = activeKey <= 0;
            bool isLastIndex = activeKey >= _usedSpline.KeyPoints.Length - 1;

            Quaternion rotation = target.KeysRotationOverrideList[activeKey];

            if (!isLastIndex && isFirstIndex)
                rotation = GetRotationLerpCalculation(target, 0, targetKeyPos, target.StartKeyRotationOverride);
            else if (!isLastIndex)
                rotation = GetRotationLerpCalculation(target, activeKey, targetKeyPos, target.KeysRotationOverrideList[activeKey - 1]);

            return rotation;
        }

        private Quaternion GetRotationLerpCalculation(SplineTarget target, int activeKey, float targetKeyPos, Quaternion lerpStartRot)
        {
            float scaleValue = Vector3.Distance(_usedSpline.KeyPoints[activeKey].KeyPosition, _usedSpline.KeyPoints[activeKey + 1].KeyPosition);
            float scaledTargetKeyPos = targetKeyPos * scaleValue;

            float time = _usedSpline.KeyPoints[activeKey].RotationLerpTime != 0 ? scaledTargetKeyPos / _usedSpline.KeyPoints[activeKey].RotationLerpTime : 1;

            Quaternion rotation = Quaternion.Lerp(lerpStartRot, target.KeysRotationOverrideList[activeKey], time);

            if (_usedSpline.KeyPoints[activeKey].RotationLerpShape.keys.Length > 1)
                rotation = CurveLerp(lerpStartRot, target.KeysRotationOverrideList[activeKey], time, _usedSpline.KeyPoints[activeKey].RotationLerpShape);

            return rotation;
        }

        private void OverwriteRotationKeysValues(ref SplineTarget target, int i)
        {
            bool isLastIndex = i >= _usedSpline.KeyPoints.Length - 1;
            bool isFollowing = _usedSpline.KeyPoints[i].RotationOverwrite == RotationMode.FollowRotation;
            bool isStraight = _usedSpline.KeyPoints[i].RotationOverwrite == RotationMode.StraightRotation;
            bool activeKeyIsLast = target.ActiveKey >= _usedSpline.KeyPoints.Length - 1;
            bool updateStartRot = i == 0;

            if (isLastIndex)
                target.KeysRotationOverrideList[i] = target.KeysRotationOverrideList[i - 1];
            else if (isStraight)
                target.KeysRotationOverrideList[i] = Quaternion.LookRotation(transform.TransformPoint(_usedSpline.KeyPoints[i + 1].KeyPosition) - transform.TransformPoint(_usedSpline.KeyPoints[i].KeyPosition), Vector3.up);
            else if (isFollowing)
            {
                bool isActiveKey = target.ActiveKey == i;
                bool nextKeyIsFollowing = _usedSpline.KeyPoints[i + 1].RotationOverwrite == RotationMode.FollowRotation;

                if (!isActiveKey && !nextKeyIsFollowing)
                    target.KeysRotationOverrideList[i] = RotationKeyFollowCalculation(target.TargetSplinePos, i, (i + 1) / (float)(_usedSpline.KeyPoints.Length - 1), true);
                else if (!activeKeyIsLast)
                    target.KeysRotationOverrideList[i] = RotationKeyFollowCalculation(target.TargetSplinePos, target.ActiveKey, target.TargetSplinePos, false);
            }


            if (updateStartRot && _usedSpline.StartRotationOverwrite == RotationMode.FollowRotation)
                target.StartKeyRotationOverride = activeKeyIsLast ? target.StartKeyRotationOverride : RotationKeyFollowCalculation(target.TargetSplinePos, target.ActiveKey, target.TargetSplinePos, false);
            else if (updateStartRot && _usedSpline.StartRotationOverwrite == RotationMode.StraightRotation)
                target.StartKeyRotationOverride = Quaternion.LookRotation(transform.TransformPoint(_usedSpline.KeyPoints[1].KeyPosition) - transform.TransformPoint(_usedSpline.KeyPoints[0].KeyPosition), Vector3.up);
        }

        private Quaternion RotationKeyFollowCalculation(float targetSplinePos, int targetIndex, float pos, bool staticFollow)
        {
            float scaleValue = 0.001f / Vector3.Distance(_usedSpline.KeyPoints[targetIndex].KeyPosition, _usedSpline.KeyPoints[targetIndex + 1].KeyPosition);
            float targetedPos = staticFollow ? (targetIndex + 1 - scaleValue) / (_usedSpline.KeyPoints.Length - 1) : targetSplinePos + scaleValue;

            Quaternion rotation = staticFollow ? Quaternion.LookRotation(GetTargetPos(pos) - GetTargetPos(targetedPos), Vector3.up) : Quaternion.LookRotation(GetTargetPos(targetedPos) - GetTargetPos(pos), Vector3.up);

            return rotation;
        }

        private float GetTargetFade(SplineTarget target, float targetSplinePos)
        {
            float targetSplinePosScaled = targetSplinePos * (_usedSpline.KeyPoints.Length - 1);
            float distance = target.ActiveKey < (_usedSpline.KeyPoints.Length - 1) ? Vector3.Distance(_usedSpline.KeyPoints[target.ActiveKey].KeyPosition, _usedSpline.KeyPoints[target.ActiveKey + 1].KeyPosition) : Vector3.Distance(_usedSpline.KeyPoints[target.ActiveKey - 1].KeyPosition, _usedSpline.KeyPoints[target.ActiveKey].KeyPosition);
            float targetSplinePosRescaled = targetSplinePosScaled * distance;

            float startFadeTime = target.StartFadeTime * (_usedSpline.KeyPoints.Length - 1) * distance;
            float endFadeTime = (1 - target.EndFadeTime) * (_usedSpline.KeyPoints.Length - 1) * distance;

            float startFade = target.StartFadeTime == 0 ? 1 : Mathf.Lerp(0, 1, targetSplinePosRescaled / startFadeTime);
            float endFade = target.EndFadeTime == 0 ? 1 : Mathf.Lerp(0, 1, (((_usedSpline.KeyPoints.Length - 1) - targetSplinePosScaled) * distance) / (endFadeTime));

            float fadeValue = endFade > startFade ? startFade : endFade;

            return fadeValue;
        }

        private void SetTargetOpacity(ref SplineTarget target)
        {
            if (!target.TargetManualFadeReference)
            {
                if (target.Target.TryGetComponent(out MeshRenderer meshRenderer))
                    meshRenderer.sharedMaterial.color = new Color(meshRenderer.sharedMaterial.color.r, meshRenderer.sharedMaterial.color.g, meshRenderer.sharedMaterial.color.b, GetTargetFade(target, target.TargetSplinePos));
            }
            else
            {
                if (target.TargetManualFadeReference.shader == Shader.Find("Universal Render Pipeline/Autodesk Interactive/AutodeskInteractiveTransparent"))
                    target.TargetManualFadeReference.color = new Color(target.TargetManualFadeReference.color.r, target.TargetManualFadeReference.color.g, target.TargetManualFadeReference.color.b, GetTargetFade(target, target.TargetSplinePos));
                else
                    target.TargetManualFadeReference.color = new Color(target.TargetManualFadeReference.color.r, target.TargetManualFadeReference.color.g, target.TargetManualFadeReference.color.b, GetTargetFade(target, target.TargetSplinePos));
            }
        }

        #endregion

        #region SplineMovements

        private void UpdatePlayer(ref SplineTarget target)
        {
            bool isAPauseTime = _usedSpline.KeyPoints[target.ActiveKey].PauseTime != Vector2.zero;
            bool hasPassedAKey = target.ActiveKey != target.ActiveKeyMemo;

            if (!isAPauseTime || !hasPassedAKey)
                MovePlayer(ref target);

            if (isAPauseTime && (hasPassedAKey || target.PauseTimer != 0))
                PauseBetweenKeys(ref target);

            target.ActiveKeyMemo = target.ActiveKey;
        }

        private void MovePlayer(ref SplineTarget target)
        {
            float distance = target.ActiveKey < (_usedSpline.KeyPoints.Length - 1) ? Vector3.Distance(_usedSpline.KeyPoints[target.ActiveKey].KeyPosition, _usedSpline.KeyPoints[target.ActiveKey + 1].KeyPosition) : Vector3.Distance(_usedSpline.KeyPoints[target.ActiveKey - 1].KeyPosition, _usedSpline.KeyPoints[target.ActiveKey].KeyPosition);
            float targetKeyPosScaled = target.TargetKeyPos * distance;
            float speedMultiplierLerp = 0;

            if (target.ActiveKey > 0)
            {
                float time = _usedSpline.KeyPoints[target.ActiveKey].SpeedLerpTime != 0 ? targetKeyPosScaled / _usedSpline.KeyPoints[target.ActiveKey].SpeedLerpTime : 1;
                speedMultiplierLerp = Mathf.Lerp(_usedSpline.KeyPoints[target.ActiveKey - 1].Speed, _usedSpline.KeyPoints[target.ActiveKey].Speed, time);
            }
            else
            {
                float time = _usedSpline.KeyPoints[0].SpeedLerpTime != 0 ? targetKeyPosScaled / _usedSpline.KeyPoints[0].SpeedLerpTime : 1;
                speedMultiplierLerp = Mathf.Lerp(_usedSpline.StartSpeed, _usedSpline.KeyPoints[0].Speed, time);
            }

            float moveDelta = speedMultiplierLerp * target.SpeedMultiplier;
            float moveDeltaScaled = moveDelta / distance;

            target.TargetSplinePos += moveDeltaScaled * Time.deltaTime;

            if (target.TargetLinkedAnimation)
                target.TargetLinkedAnimation.speed = moveDelta * target.AnimSpeedMultiplier;

            if (target.TargetSplinePos >= 1 && target.Loop)
            {
                if (target.LoopTimer < target.LoopTime)
                {
                    target.LoopTimer += Time.deltaTime;
                }
                else
                {
                    target.LoopTimer = 0;
                    target.TargetSplinePos = target.LoopPos;
                }
            }
        }

        private void PauseBetweenKeys(ref SplineTarget target)
        {
            if (target.PauseTimer == 0)
                target.PauseRandomValue = Random.Range(_usedSpline.KeyPoints[target.ActiveKey].PauseTime.x, _usedSpline.KeyPoints[target.ActiveKey].PauseTime.y);

            if (target.PauseTimer < target.PauseRandomValue)
            {
                if (target.TargetLinkedAnimation)
                    target.TargetLinkedAnimation.speed = 0;

                target.TargetSplinePos = target.ActiveKey / (float)(_usedSpline.KeyPoints.Length - 1);
                target.PauseTimer += Time.deltaTime;
            }
            else
            {
                target.PauseTimer = 0;
            }
        }

        #endregion
    }

}