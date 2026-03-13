using System;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

[Serializable]
public struct WeaponTriggerZone
{
    public Vector3 position;
    public float radius;
}

public class MeleeWeaponController : MonoBehaviour, IWeaponObservable<GameObject>
{
    [SerializeField] private WeaponTriggerZone[] triggerZones;
    [SerializeField] private LayerMask targetLayerMask;

    private HashSet<Collider> _hitColliders;
    private Vector3[] _previousTriggerPositions;
    
    private List<IWeaponObserver<GameObject>> _observers =
        new List<IWeaponObserver<GameObject>>();
    
    RaycastHit[] _hits = new RaycastHit[10];

    private bool _isTriggering;

    private void Awake()
    {
        _previousTriggerPositions = new Vector3[triggerZones.Length];
        _hitColliders = new HashSet<Collider>();
        
        _isTriggering = false;
    }

    // 무기의 주인이 무기에게 트리거 작동을 시작하라고 전달 함수
    public void StartTrigger()
    {
        _hitColliders.Clear();
        for (int i = 0; i < triggerZones.Length; i++)
        {
            _previousTriggerPositions[i] = transform.TransformPoint(triggerZones[i].position);
        }
        _isTriggering = true;
        // Time.timeScale = 0.1f;
    }

    // 무기의 주인이 무기에게 트리거 작동을 중단하라고 전달 함수
    public void EndTrigger()
    {
        foreach (var hitCollider in _hitColliders)
        {
            Notify(hitCollider.gameObject);
        }
        _isTriggering = false;
        // Time.timeScale = 1f;
    }

    private void FixedUpdate()
    {
        if (!_isTriggering) return;

        for (int i = 0; i < triggerZones.Length; i++)
        {
            var worldPosition = transform.TransformPoint(triggerZones[i].position);
            var direction = (worldPosition - _previousTriggerPositions[i]).normalized;
            var maxDistance = Vector3.Distance(worldPosition, _previousTriggerPositions[i]);

            Ray ray = new Ray(_previousTriggerPositions[i], direction);
            var hitCount = Physics.SphereCastNonAlloc(ray, triggerZones[i].radius, _hits, 
                maxDistance, targetLayerMask);
            
            for (int j = 0; j < hitCount; j++)
            {
                var hitCollider = _hits[j].collider;
                if (hitCollider) _hitColliders.Add(hitCollider);
            }
            _previousTriggerPositions[i] = transform.TransformPoint(triggerZones[i].position);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        
        for (int i = 0; i < triggerZones.Length; i++)
        {
            var triggerZonePosition = transform.TransformPoint(triggerZones[i].position);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(triggerZonePosition, triggerZones[i].radius);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_previousTriggerPositions[i], triggerZones[i].radius);
        }
    }

    #region Observer Pattern 코드
    
    public void Subscribe(IWeaponObserver<GameObject> observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void Unsubscribe(IWeaponObserver<GameObject> observer)
    {
        _observers.Remove(observer);
    }

    public void Notify(GameObject value)
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(value);
        }
    }
    
    #endregion
}
