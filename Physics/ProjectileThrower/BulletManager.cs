using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UPDB.CoreHelper.UsableMethods;

public class BulletManager : UPDBBehaviour
{
    [SerializeField]
    private float _speed = 1;

    [SerializeField]
    private float _rangeMax = Mathf.Infinity;

    [SerializeField]
    private float _lifeTime = 60;

    [SerializeField]
    private LayerMask _collisionIncludedLayers = -1;


    private List<Collider> _colliderExcludedList = new List<Collider>();
    private float _lifeTimer = 0;
    private Rigidbody _rb;
    private GameObject _thrower;

    #region Public API

    public float Speed
    {
        get => _speed;
        set => _speed = value;
    }

    public Rigidbody Rb
    {
        get
        {
            return MakeNonNullable(ref _rb, gameObject);
        }

        set => _rb = value;
    }

    public GameObject Thrower
    {
        get => _thrower;
        set => _thrower = value;
    }

    public List<Collider> ColliderExcludedList
    {
        get => _colliderExcludedList;
        set => _colliderExcludedList = value;
    }

    #endregion

    void Awake()
    {
        MakeNonNullable(ref _rb, gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        if(_lifeTimer >= _lifeTime || Vector3.Distance(transform.position, _thrower.transform.position) >= _rangeMax)
        {
            Destroy(gameObject);
        }

        _lifeTimer += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_collisionIncludedLayers == (_collisionIncludedLayers | (1 << other.gameObject.layer)) && !_colliderExcludedList.Contains(other))
        {
            Destroy(gameObject);
        }
    }
}
