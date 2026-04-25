using UnityEngine;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpHeight = 1.5f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private float minPitch = -80f;
    [SerializeField] private float maxPitch = 80f;
    [SerializeField] private bool lockCursorOnStart = true;
    [SerializeField] private bool invertMouseX = false;
    [SerializeField] private bool invertMouseY = false;

    [Header("Gun Follow")]
    [SerializeField] private Transform gunTransform;
    [SerializeField] private Vector3 gunPositionOffset = new Vector3(0.25f, -0.2f, 0.6f);
    [SerializeField] private Vector3 gunRotationOffset = Vector3.zero;
    [SerializeField] private bool useCurrentGunOffsetOnStart = true;

    private CharacterController controller;
    private float verticalVelocity;
    private float cameraPitch;
    private Vector3 baseCameraLocalEuler;

    [Header("UI")]
    public float numOfEnDest = 0;
    public TextMeshProUGUI EnNum;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void Start()
    {
        if (cameraTransform != null)
        {
            baseCameraLocalEuler = cameraTransform.localEulerAngles;
            cameraPitch = NormalizeAngle(baseCameraLocalEuler.x);

            if (gunTransform != null && useCurrentGunOffsetOnStart)
            {
                gunPositionOffset = cameraTransform.InverseTransformPoint(gunTransform.position);
                Quaternion relativeRotation = Quaternion.Inverse(cameraTransform.rotation) * gunTransform.rotation;
                gunRotationOffset = relativeRotation.eulerAngles;
            }
        }

        if (lockCursorOnStart)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void Update()
    {
        if (GameStateController.IsGamePaused)
        {
            return;
        }

        HandleMouseLook();
        HandleMovement();

        if (EnNum != null)
        {
            EnNum.text = "Num of Enemy destroyed: " + numOfEnDest;
        }
    }

    private void LateUpdate()
    {
        if (GameStateController.IsGamePaused)
        {
            return;
        }

        HandleGunFollow();
    }

    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        if (cameraTransform != null)
        {
            forward = cameraTransform.forward;
            right = cameraTransform.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
        }

        Vector3 move = right * horizontalInput + forward * verticalInput;
        controller.Move(move * moveSpeed * Time.deltaTime);

        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * Time.deltaTime;
        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        if (cameraTransform == null)
        {
            return;
        }

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        if (invertMouseX)
        {
            mouseX = -mouseX;
        }

        if (invertMouseY)
        {
            mouseY = -mouseY;
        }

        transform.Rotate(Vector3.up * mouseX);

        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, minPitch, maxPitch);
        cameraTransform.localRotation = Quaternion.Euler(
            cameraPitch,
            baseCameraLocalEuler.y,
            baseCameraLocalEuler.z
        );
    }

    private float NormalizeAngle(float angle)
    {
        if (angle > 180f)
        {
            angle -= 360f;
        }

        return angle;
    }

    private void HandleGunFollow()
    {
        if (cameraTransform == null || gunTransform == null)
        {
            return;
        }

        gunTransform.position = cameraTransform.TransformPoint(gunPositionOffset);
        gunTransform.rotation = cameraTransform.rotation * Quaternion.Euler(gunRotationOffset);
    }

}