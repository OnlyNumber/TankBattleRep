using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeCameraController : MonoBehaviour
{
    [Header("Target & Focus")]
    [SerializeField] private Transform _target;             // Танк або точка в центрі танка
    [SerializeField] private Vector3 _targetOffset = new Vector3(0f, 1.2f, 0f); // Зсув висоти прицілювання (на рівень башти)

    [Header("Rotation Settings")]
    [SerializeField] private float _rotationSensitivity = 0.2f; // Чутливість обертання
    [SerializeField] private float _minPitch = -10f;             // Мінімальний кут огляду (знизу)
    [SerializeField] private float _maxPitch = 60f;              // Максимальний кут огляду (зверху)
    [SerializeField] private float _rotationSmoothness = 10f;    // Плавність інерції

    [Header("Zoom Settings")]
    [SerializeField] private float _distance = 8f;               // Початкова відстань
    [SerializeField] private float _minDistance = 3f;            // Максимальне наближення
    [SerializeField] private float _maxDistance = 18f;           // Максимальне віддалення
    [SerializeField] private float _zoomSensitivity = 0.02f;     // Чутливість колеса миші
    [SerializeField] private float _zoomSmoothness = 8f;         // Плавність зуму

    private float _yaw;   // Поворот по горизонталі (навколо Y)
    private float _pitch; // Поворот по вертикалі (вгору/вниз)

    private float _targetYaw;
    private float _targetPitch;
    private float _targetDistance;

    private Vector2 _previousPointerPosition;

    private void Start()
    {
        // Ініціалізуємо початкові кути з поточного повороту камери
        Vector3 angles = transform.eulerAngles;
        _targetYaw = _yaw = angles.y;
        _targetPitch = _pitch = angles.x;
        _targetDistance = _distance;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        HandleRotationInput();
        HandleZoomInput();
        UpdateCameraTransform();
    }

    private void HandleRotationInput()
    {
        Vector2 currentPointerPos = Vector2.zero;
        bool isPressing = false;
        bool wasPressedThisFrame = false;

        // 1. Читання миші
        if (Mouse.current != null)
        {
            currentPointerPos = Mouse.current.position.ReadValue();
            isPressing = Mouse.current.leftButton.isPressed;
            wasPressedThisFrame = Mouse.current.leftButton.wasPressedThisFrame;
        }
        // 2. Читання тачскріна (один палець для обертання)
        else if (Touchscreen.current != null && Touchscreen.current.touches.Count == 1)
        {
            var touch = Touchscreen.current.primaryTouch;
            currentPointerPos = touch.position.ReadValue();
            isPressing = touch.press.isPressed;
            wasPressedThisFrame = touch.press.wasPressedThisFrame;
        }

        if (wasPressedThisFrame)
        {
            _previousPointerPosition = currentPointerPos;
        }
        else if (isPressing)
        {
            Vector2 delta = currentPointerPos - _previousPointerPosition;
            _previousPointerPosition = currentPointerPos;

            // Змінюємо цільові кути
            _targetYaw += delta.x * _rotationSensitivity;
            _targetPitch -= delta.y * _rotationSensitivity; // Інвертуємо Y для природного керування

            // Обмежуємо нахил по вертикалі, щоб не перевертати камеру
            _targetPitch = Mathf.Clamp(_targetPitch, _minPitch, _maxPitch);
        }
    }

    private void HandleZoomInput()
    {
        // Зум колесом миші
        if (Mouse.current != null)
        {
            float scroll = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scroll) > 0.01f)
            {
                _targetDistance -= scroll * _zoomSensitivity;
                _targetDistance = Mathf.Clamp(_targetDistance, _minDistance, _maxDistance);
            }
        }
        // Зум Pinch-to-Zoom (двома пальцями на смартфоні)
        else if (Touchscreen.current != null && Touchscreen.current.touches.Count >= 2)
        {
            var touch0 = Touchscreen.current.touches[0];
            var touch1 = Touchscreen.current.touches[1];

            Vector2 prevPos0 = touch0.position.ReadValue() - touch0.delta.ReadValue();
            Vector2 prevPos1 = touch1.position.ReadValue() - touch1.delta.ReadValue();

            float prevDistance = (prevPos0 - prevPos1).magnitude;
            float currentDistance = (touch0.position.ReadValue() - touch1.position.ReadValue()).magnitude;

            float deltaDistance = currentDistance - prevDistance;
            _targetDistance -= deltaDistance * _zoomSensitivity;
            _targetDistance = Mathf.Clamp(_targetDistance, _minDistance, _maxDistance);
        }
    }

    private void UpdateCameraTransform()
    {
        // Плавна інтерполяція (Lerp) кутів та відстані для ефекту інерції WoT
        _yaw = Mathf.Lerp(_yaw, _targetYaw, Time.deltaTime * _rotationSmoothness);
        _pitch = Mathf.Lerp(_pitch, _targetPitch, Time.deltaTime * _rotationSmoothness);
        _distance = Mathf.Lerp(_distance, _targetDistance, Time.deltaTime * _zoomSmoothness);

        // Обчислюємо поворот і позицію у 3D-просторі
        Quaternion rotation = Quaternion.Euler(_pitch, _yaw, 0f);
        Vector3 targetFocusPosition = _target.position + _targetOffset;
        Vector3 position = targetFocusPosition - (rotation * Vector3.forward * _distance);

        // Применяємо позицію та поворот до камери
        transform.rotation = rotation;
        transform.position = position;
    }
}