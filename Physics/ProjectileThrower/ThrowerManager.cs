using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UPDB.CoreHelper.UsableMethods;

public class ThrowerManager : UPDBBehaviour
{
    [SerializeField, Tooltip("active target to shoot")]
    private Transform _activeTarget = null;

    [SerializeField, Tooltip("bullet prefab")]
    private GameObject _bulletPrefab = null;

    [SerializeField]
    private Transform _bulletSpawnPoint = null;

    [SerializeField]
    private List<Collider> _bulletColliderExcludedList;

    [SerializeField]
    private float _imprecisionRadius = 1f;

    [SerializeField]
    private float _imprecisionDistanceMultiplier = 1f;

    [SerializeField, Range(0, 1)]
    private float _imprecisionDistancePercentage = 1f;

    private Vector3 _lastShootDirection = Vector3.zero;
    private Vector3 _lastIntersectionPoint = Vector3.zero;
    private Vector3 _lastAimedPoint = Vector3.zero;
    private float _lastImprecisionRadius = 0;

    #region Public API

    public Transform ActiveTarget
    {
        get => _activeTarget;
        set => _activeTarget = value;
    }

    public GameObject BulletPrefab
    {
        get => _bulletPrefab;
        set => _bulletPrefab = value;
    }

    public Transform BulletSpawnPoint
    {
        get => _bulletSpawnPoint;
        set => _bulletSpawnPoint = value;
    }

    public float ImprecisionRadius
    {
        get => _imprecisionRadius;
        set => _imprecisionRadius = value;
    }

    public float ImprecisionDistanceMultiplier
    {
        get => _imprecisionDistanceMultiplier;
        set => _imprecisionDistanceMultiplier = value;
    }

    public float ImprecisionDistancePercentage
    {
        get => _imprecisionDistancePercentage;
        set => _imprecisionDistancePercentage = value;
    }

    #endregion

    protected override void OnScene()
    {
        base.OnScene();

        Debug.DrawRay(_bulletSpawnPoint.position, _lastShootDirection, Color.blue);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(_lastIntersectionPoint, _lastImprecisionRadius);
        Gizmos.color = Color.white;

        DebugDrawPoint(_lastAimedPoint, Color.red);
    }

    public void ShootBullet()
    {
        if (!_activeTarget)
            return;

        GameObject bullet = Instantiate(_bulletPrefab, _bulletSpawnPoint.position, Quaternion.identity);

        BulletManager bulletManager = null;
        MakeNonNullable(ref bulletManager, bullet);

        bulletManager.ColliderExcludedList = _bulletColliderExcludedList;
        bulletManager.Thrower = gameObject;

        Rigidbody targetRb = null;
        MakeNonNullable(ref targetRb, _activeTarget.gameObject);

        _lastIntersectionPoint = CalculateInterceptionPoint(_bulletSpawnPoint.position, _activeTarget.position, targetRb.velocity, bulletManager.Speed);

        _lastImprecisionRadius = Mathf.Lerp(_imprecisionRadius, _imprecisionRadius * (Vector3.Distance(_bulletSpawnPoint.position, _activeTarget.position) * _imprecisionDistanceMultiplier), _imprecisionDistancePercentage);
        _lastAimedPoint = _lastIntersectionPoint + (Random.insideUnitSphere * _lastImprecisionRadius);

        if (!bulletManager.Rb.useGravity)
            _lastShootDirection = (_lastAimedPoint - _bulletSpawnPoint.position).normalized * bulletManager.Speed;
        else
            _lastShootDirection = CalculateLaunchVelocity(_bulletSpawnPoint.position, _lastAimedPoint, Vector3.Distance(_bulletSpawnPoint.position, _lastAimedPoint) / bulletManager.Speed);

        bulletManager.Rb.velocity = _lastShootDirection;
    }

    /// <summary>
    /// Calcule la direction à tirer pour intercepter une cible en mouvement.
    /// </summary>
    /// <param name="A">Point de départ du projectile</param>
    /// <param name="B">Position actuelle de la cible</param>
    /// <param name="vB">Vitesse de la cible (direction * vitesse en m/s)</param>
    /// <param name="projectileSpeed">Vitesse constante du projectile (en m/s)</param>
    /// <returns>Vecteur direction normalisé de la trajectoire du projectile, ou Vector3.zero si pas de solution</returns>
    private Vector3 CalculateInterceptionPoint(Vector3 A, Vector3 B, Vector3 vB, float projectileSpeed)
    {
        Vector3 D = B - A;

        float a = Vector3.Dot(vB, vB) - projectileSpeed * projectileSpeed;
        float b = 2f * Vector3.Dot(vB, D);
        float c = Vector3.Dot(D, D);

        float discriminant = b * b - 4f * a * c;

        if (discriminant < 0f)
        {
            // Pas de solution : le projectile ne peut pas rattraper la cible
            return Vector3.zero;
        }

        float sqrtDiscriminant = Mathf.Sqrt(discriminant);

        // Deux solutions possibles pour le temps d'interception
        float t1 = (-b + sqrtDiscriminant) / (2f * a);
        float t2 = (-b - sqrtDiscriminant) / (2f * a);

        // On garde le temps positif le plus petit
        float t = Mathf.Min(t1, t2);
        if (t < 0f) t = Mathf.Max(t1, t2);
        if (t < 0f) return Vector3.zero;

        // Position future de B
        return B + vB * t;
    }

    private Vector3 CalculateLaunchVelocity(Vector3 start, Vector3 end, float timeToTarget)
    {
        Vector3 displacement = end - start;
        Vector3 horizontalDisplacement = new Vector3(displacement.x, 0, displacement.z);
        Vector3 horizontalVelocity = horizontalDisplacement / timeToTarget;
        float verticalVelocity = (displacement.y + 0.5f * -Physics.gravity.y * timeToTarget * timeToTarget) / timeToTarget;

        return horizontalVelocity + Vector3.up * verticalVelocity;
    }
}
