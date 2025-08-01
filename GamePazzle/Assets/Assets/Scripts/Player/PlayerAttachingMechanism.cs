using UnityEngine;



public class PlayerAttachingMechanism : MonoBehaviour, IInteractable // Реализуем контракт IInteractable

{

    // -------------------------------------------------------------------

    // 1. ПОЛЯ (Fields)

    // -------------------------------------------------------------------



    [Header("Связанный Механизм")]

    [Tooltip("Ссылка на объект с RotaryMechanism, который будет вращаться.")]

    [SerializeField] private RotaryMechanism _connectedRotaryMechanism;



    [Header("Настройки прикрепления")]

    [Tooltip("Место (локальные координаты), куда игрок должен 'встать' при взаимодействии.")]

    [SerializeField] private Vector3 _playerAttachOffset = new Vector3(0, 0, -0.5f);

    [Tooltip("Поворот игрока (локальные координаты) при взаимодействии.")]

    [SerializeField] private Quaternion _playerAttachLocalRotation = Quaternion.identity;



    private Animator _playerAnimatorCache;

    private readonly int _animatorSpeedHash = Animator.StringToHash("speed");



    // -------------------------------------------------------------------

    // 2. IINTERACTABLE IMPLEMENTATION

    // -------------------------------------------------------------------

    public string InteractionPrompt => "Тянуть";
    public string InfoText => string.Empty;
    public bool IsContinuous => true; // Это длительное взаимодействие
    public bool IsInfoPanelActive { get; set; }

    /// <summary>

    /// Логика, которая выполняется при начале взаимодействия.

    /// </summary>

    public void BeginInteraction(PlayerController player)

    {

        _playerAnimatorCache = player.GetComponent<Animator>();

        // Вызываем публичный метод игрока, чтобы он сам себя "прикрепил" к нам.

        // Мы передаем ему, куда именно нужно встать и как повернуться.

        player.AttachTo(this.transform, _playerAttachOffset, _playerAttachLocalRotation);

    }



    /// <summary>

    /// Логика, которая выполняется каждый кадр во время взаимодействия.

    /// </summary>

    public void UpdateInteraction(PlayerController player)

    {

        if (_connectedRotaryMechanism != null)

        {

            // Получаем "сырой" ввод по оси Z (W/S -> 1/-1)

            // ИНВЕРТИРУЕМ ВВОД: W будет положительным (+1), S отрицательным (-1)

            float verticalInput = player.RawInputDirection.z * -1f;



            // Передаем это значение в скрипт RotaryMechanism

            _connectedRotaryMechanism.RotateBy(verticalInput);



            // --- Управление анимацией Blend Tree ---

            if (_playerAnimatorCache != null)

            {

                // Если есть значимый ввод (W/S активно)

                if (Mathf.Abs(verticalInput) > 0.1f)

                {

                    // Масштабируем ввод, чтобы он соответствовал вашему диапазону Blend Tree (-2 до 2)

                    // verticalInput уже -1 или 1, умножаем на 2, чтобы получить -2 или 2

                    float blendValue = verticalInput * -2f;

                    _playerAnimatorCache.SetFloat(_animatorSpeedHash, blendValue);

                }

                else // Если ввода нет или он слишком мал

                {

                    // Возвращаем Blend Tree параметр в 0, чтобы анимация остановилась

                    _playerAnimatorCache.SetFloat(_animatorSpeedHash, 0f);

                }



            }

        }

    }



    /// <summary>

    /// Логика, которая выполняется при завершении взаимодействия.

    /// </summary>

    public void EndInteraction(PlayerController player)

    {

        // Вызываем публичный метод игрока, чтобы он сам себя "открепил".

        player.Detach();

    }



    private void OnDrawGizmosSelected()

    {

        // Убедитесь, что _connectedRotaryMechanism назначен, чтобы Gizmo был виден

        // только при выборе объекта в редакторе.

        if (_connectedRotaryMechanism != null)

        {

            // Вычисляем глобальную позицию, куда будет перемещен игрок

            // transform.TransformPoint() преобразует локальные координаты в глобальные

            Vector3 globalAttachPosition = transform.TransformPoint(_playerAttachOffset);



            // Вычисляем глобальное вращение, которое будет применено к игроку

            // Умножаем вращение родителя на локальное смещение вращения

            Quaternion globalAttachRotation = transform.rotation * _playerAttachLocalRotation;



            Gizmos.color = Color.green; // Цвет для сферы позиции

            Gizmos.DrawWireSphere(globalAttachPosition, 0.15f); // Маленькая сфера, показывающая центр игрока



            // Рисуем оси, чтобы показать ориентацию игрока:

            // Синяя линия: направление вперед (Z) игрока

            // Красная линия: направление вправо (X) игрока

            // Зеленая линия: направление вверх (Y) игрока

            Gizmos.color = Color.blue;

            Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.forward * 0.5f); // "Нос" игрока

            Gizmos.color = Color.red;

            Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.right * 0.3f);  // "Правое плечо" игрока

            Gizmos.color = Color.green;

            Gizmos.DrawRay(globalAttachPosition, globalAttachRotation * Vector3.up * 0.3f);     // "Голова" игрока

        }

    }

}