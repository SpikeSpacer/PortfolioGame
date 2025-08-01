using UnityEngine;

public class PlayerPush : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField, Min(0f)] float _pushDistance = 1.2f;
    [SerializeField] LayerMask _pushableMask;
    
    private Animator _animator;

    static readonly int _hashIsPushing = Animator.StringToHash("IsPushing");
    Transform _tr;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _tr = transform;
        if (_animator == null)
            Debug.LogError($"{nameof(PlayerPush)}: Animator not assigned.", this);
    }

    void Update()
    {
        if (TryGetPushable(out var target))
            StartPushing(target);
        else
            StopPushing();
    }

    bool TryGetPushable(out IPushable target)
    {
        Vector3 origin = _tr.position + Vector3.up * 0.5f;
        Vector3 direction = _tr.forward;

        if (Physics.Raycast(origin, direction, out var hit, _pushDistance, _pushableMask)
         && hit.collider.TryGetComponent<IPushable>(out target))
        {
            return true;
        }

        target = null;
        return false;
    }

    void StartPushing(IPushable pushable)
    {
        pushable.Push(_tr.forward);
        _animator.SetBool(_hashIsPushing, true);
    }

    void StopPushing()
    {
        _animator.SetBool(_hashIsPushing, false);
    }
}
