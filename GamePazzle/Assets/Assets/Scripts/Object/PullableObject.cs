using UnityEngine;



[RequireComponent(typeof(Rigidbody))]

public class PullableObject : MonoBehaviour, IInteractable

{

    // -------------------------------------------------------------------

    // 1. ПОЛЯ (Fields)

    // -------------------------------------------------------------------



    [Header("Mass Settings")]

    [SerializeField] private float _defaultMass = 1000.0f;

    [SerializeField] private float _pulledMass = 50.0f;



    private Rigidbody _rb;

    private FixedJoint _fixedJoint;

    private readonly int _animatorSpeedHash = Animator.StringToHash("speed");



    // <--- НОВОЕ: Поле для сохранения исходных ограничений Rigidbody

    private RigidbodyConstraints _originalConstraints;



    // -------------------------------------------------------------------

    // 2. UNITY LIFECYCLE METHODS

    // -------------------------------------------------------------------



    private void Awake()

    {

        _rb = GetComponent<Rigidbody>();

        _rb.mass = _defaultMass;

        // <--- НОВОЕ: Сохраняем исходные ограничения, чтобы вернуть их в конце

        _originalConstraints = _rb.constraints;

    }



    // -------------------------------------------------------------------

    // 3. IINTERACTABLE IMPLEMENTATION

    // -------------------------------------------------------------------

    public string InteractionPrompt => "Тянуть";
    public string InfoText => string.Empty;
    public bool IsContinuous => true; // Это длительное взаимодействие
    public bool IsInfoPanelActive { get; set; }

    public void BeginInteraction(PlayerController player)

    {

        if (_fixedJoint != null) return;



        // Создаем и настраиваем сустав

        _fixedJoint = gameObject.AddComponent<FixedJoint>();

        _fixedJoint.connectedBody = player.PlayerRigidbody;

        _fixedJoint.breakForce = Mathf.Infinity;

        _fixedJoint.breakTorque = Mathf.Infinity;

        _fixedJoint.enableCollision = false;



        _rb.mass = _pulledMass;



        // <--- НОВОЕ: Замораживаем вращение самого камня по всем осям

        _rb.constraints = RigidbodyConstraints.FreezeRotation;



        player.EnableStandardAnimations(false);

    }



    public void UpdateInteraction(PlayerController player)

    {

        // Эта логика остается без изменений

        Vector3 playerFacingDir = player.transform.forward;

        Vector3 playerMoveDir = player.WorldMovementVector;

        float dot = Vector3.Dot(playerFacingDir, playerMoveDir);

        float animSpeed = 0f;



        if (playerMoveDir.sqrMagnitude > 0.01f)

        {

            const float deadZone = 0.1f;

            if (dot > deadZone)

                animSpeed = 2f;

            else if (dot < -deadZone)

                animSpeed = -2f;

        }



        player.SetAnimatorFloat(_animatorSpeedHash, animSpeed);

    }



    public void EndInteraction(PlayerController player)

    {

        if (_fixedJoint != null)

        {

            Destroy(_fixedJoint);

            _fixedJoint = null;

        }

        _rb.mass = _defaultMass;



        // <--- НОВОЕ: Снимаем заморозку, возвращая исходные ограничения

        _rb.constraints = _originalConstraints;



        player.EnableStandardAnimations(true);

    }

}

