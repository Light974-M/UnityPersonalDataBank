using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;
using UPDB.Sound.AmbianceMixer;

public class FollowingMultipleSourceManager : UPDBBehaviour
{
    [SerializeField]
    private Transform _playerObj;

    [SerializeField]
    private AudioClip _audio;

    [SerializeField]
    private bool _followPos = true;

    [SerializeField]
    private bool _followRot = false;

    [SerializeField]
    private Vector3 _direction = Vector3.one;

    [SerializeField, Range(0,1)]
    private float _directionWheight = 1;

    [SerializeField]
    private AxisMode _axisUsed;

    private AudioSource[] _sources = new AudioSource[6];
    private Vector3[] _sourcesDirections = new Vector3[]
    {
        Vector3.left, Vector3.right, Vector3.forward, Vector3.back, Vector3.up, Vector3.down
    };

    #region Public API

    [System.Flags]
    public enum AxisMode
    {
        Side = 2,
        Front = 4,
        Height = 8,
    }

    #endregion

    private void Awake()
    {
        TestAudioSourceNumbers();
    }

    private void Update()
    {
        if (_followPos)
            transform.position = _playerObj.position;
        if (_followRot)
            transform.rotation = _playerObj.rotation;

        UpdateSourcesValue();
    }

    private void TestAudioSourceNumbers()
    {
        if ((_axisUsed & AxisMode.Side) == AxisMode.Side)
        {
            if (!_sources[0])
            {
                GameObject newObj = new GameObject("AudioSource_Left");
                newObj.transform.position = transform.TransformPoint(Vector3.left);
                newObj.transform.parent = transform;
                _sources[0] = newObj.AddComponent<AudioSource>();
                _sources[0].spatialBlend = 1;
                _sources[0].clip = _audio;
                _sources[0].loop = true;
                _sources[0].Play();
            }

            if (!_sources[1])
            {
                GameObject newObj = new GameObject("AudioSource_Right");
                newObj.transform.position = transform.TransformPoint(Vector3.right);
                newObj.transform.parent = transform;
                _sources[1] = newObj.AddComponent<AudioSource>();
                _sources[1].spatialBlend = 1;
                _sources[1].clip = _audio;
                _sources[1].loop = true;
                _sources[1].Play();
            }
        }
        else
        {
            if (_sources[0])
                Destroy(_sources[0].gameObject);

            if (_sources[1])
                Destroy(_sources[1].gameObject);
        }

        if ((_axisUsed & AxisMode.Front) == AxisMode.Front)
        {
            if (!_sources[2])
            {
                GameObject newObj = new GameObject("AudioSource_Forward");
                newObj.transform.position = transform.TransformPoint(Vector3.forward);
                newObj.transform.parent = transform;
                _sources[2] = newObj.AddComponent<AudioSource>();
                _sources[2].spatialBlend = 1;
                _sources[2].clip = _audio;
                _sources[2].loop = true;
                _sources[2].Play();
            }

            if (!_sources[3])
            {
                GameObject newObj = new GameObject("AudioSource_Backward");
                newObj.transform.position = transform.TransformPoint(Vector3.back);
                newObj.transform.parent = transform;
                _sources[3] = newObj.AddComponent<AudioSource>();
                _sources[3].spatialBlend = 1;
                _sources[3].clip = _audio;
                _sources[3].loop = true;
                _sources[3].Play();
            }
        }
        else
        {
            if (_sources[2])
                Destroy(_sources[2].gameObject);

            if (_sources[3])
                Destroy(_sources[3].gameObject);
        }

        if ((_axisUsed & AxisMode.Height) == AxisMode.Height)
        {
            if (!_sources[4])
            {
                GameObject newObj = new GameObject("AudioSource_Up");
                newObj.transform.position = transform.TransformPoint(Vector3.up);
                newObj.transform.parent = transform;
                _sources[4] = newObj.AddComponent<AudioSource>();
                _sources[4].spatialBlend = 1;
                _sources[4].clip = _audio;
                _sources[4].loop = true;
                _sources[4].Play();
            }

            if (!_sources[5])
            {
                GameObject newObj = new GameObject("AudioSource_Down");
                newObj.transform.position = transform.TransformPoint(Vector3.down);
                newObj.transform.parent = transform;
                _sources[5] = newObj.AddComponent<AudioSource>();
                _sources[5].spatialBlend = 1;
                _sources[5].clip = _audio;
                _sources[5].loop = true;
                _sources[5].Play();
            }
        }
        else
        {
            if (_sources[4])
                Destroy(_sources[4].gameObject);

            if (_sources[5])
                Destroy(_sources[5].gameObject);
        }
    }

    private void UpdateSourcesValue()
    {
        float magnitude = _direction.magnitude;
        Vector3 normalizedDirection = magnitude > 0 ? _direction.normalized : Vector3.zero;

        for (int i = 0; i < _sources.Length; i++)
        {
            float dirValue = Mathf.Max(0, Vector3.Dot(normalizedDirection, _sourcesDirections[i])) * magnitude;
            float equalValue = magnitude / 6f;
            _sources[i].volume = Mathf.Lerp(equalValue, dirValue, _directionWheight);
        }
    }

    protected override void OnSceneSelected()
    {
        Debug.DrawRay(transform.position, _direction, Color.yellow);

        for (int i = 0; i < _sources.Length; i++)
        {
            if (!_sources[i])
                continue;

            Gizmos.DrawWireSphere(transform.TransformPoint(_sourcesDirections[i]), (_sources[i].volume) / 2f); 
        }
    }
}
