using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public enum DrivetrainType { FWD, RWD, AWD }

    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;

    private float mobileSteer = 0f;
    private float mobileThrottle = 0f;
    private bool mobileBrake = false;
    private CarStatus carStatus;
    private Rigidbody rb;
    public bool canDrive = true;

    //settings
    [Header("Drivetrain Settings")]
    [SerializeField] private DrivetrainType drivetrain = DrivetrainType.RWD; // Default RWD agar lebih mudah dikendalikan
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;

    [Header("Physics & Stability")]
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0f, -0.8f, 0f); // Pusat massa diturunkan agar sangat stabil
    [SerializeField] private float downforce = 50f; // Gaya tekan ke bawah ekstra agar ban menempel di jalan

    //wheel coliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    //wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private void Start()
    {
        carStatus = GetComponent<CarStatus>();
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.centerOfMass = centerOfMassOffset;
        }
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        ApplyDownforce();
    }

    private void GetInput()
    {
        horizontalInput = mobileSteer;
        verticalInput = mobileThrottle;
        isBreaking = mobileBrake;

        if (!canDrive)
            return;

        if (carStatus != null && carStatus.IsDead)
        {
            isBreaking = true;
            return;
        }

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                verticalInput += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                verticalInput -= 1f;

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontalInput += 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontalInput -= 1f;

            if (Keyboard.current.spaceKey.isPressed)
                isBreaking = true;

            if (Keyboard.current.rKey.wasPressedThisFrame)
                ResetCarRotation();
        }

        if (Gamepad.current != null)
        {
            horizontalInput += Gamepad.current.leftStick.x.ReadValue();
            verticalInput += Gamepad.current.leftStick.y.ReadValue();
            
            if (Gamepad.current.buttonSouth.isPressed)
                isBreaking = true;

            if (Gamepad.current.buttonNorth.wasPressedThisFrame)
                ResetCarRotation();
        }

        horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);
        verticalInput = Mathf.Clamp(verticalInput, -1f, 1f);
    }

    private void HandleMotor()
    {
        float motorTorqueValue = verticalInput * motorForce;
        
        // Reset torsi untuk semua roda terlebih dahulu
        frontLeftWheelCollider.motorTorque = 0f;
        frontRightWheelCollider.motorTorque = 0f;
        rearLeftWheelCollider.motorTorque = 0f;
        rearRightWheelCollider.motorTorque = 0f;

        switch (drivetrain)
        {
            case DrivetrainType.FWD:
                frontLeftWheelCollider.motorTorque = motorTorqueValue;
                frontRightWheelCollider.motorTorque = motorTorqueValue;
                break;
            case DrivetrainType.RWD:
                rearLeftWheelCollider.motorTorque = motorTorqueValue;
                rearRightWheelCollider.motorTorque = motorTorqueValue;
                break;
            case DrivetrainType.AWD:
                frontLeftWheelCollider.motorTorque = motorTorqueValue;
                frontRightWheelCollider.motorTorque = motorTorqueValue;
                rearLeftWheelCollider.motorTorque = motorTorqueValue;
                rearRightWheelCollider.motorTorque = motorTorqueValue;
                break;
        }

        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    private void ApplyDownforce()
    {
        if (rb != null)
        {
            // Gunakan rb.velocity agar kompatibel dengan seluruh versi Unity
            rb.AddForce(-transform.up * downforce * rb.linearVelocity.magnitude);
        }
    }

    public void ResetCarRotation()
    {
        if (rb != null)
        {
            // Angkat sedikit ke atas dan posisikan tegak lurus
            transform.position += Vector3.up * 1.5f;
            transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // Steering
    public void TurnLeftDown()
    {
        Debug.Log("LEFT BUTTON");
        mobileSteer = -1f;
    }

    public void TurnRightDown()
    {
        Debug.Log("RIGHT BUTTON");
        mobileSteer = 1f;
    }

    public void TurnUp()
    {
        mobileSteer = 0f;
    }

    // Gas
    public void GasDown()
    {
        mobileThrottle = 1f;
    }

    public void GasUp()
    {
        mobileThrottle = 0f;
    }

    // Brake
    public void BrakeDown()
    {
        mobileBrake = true;
    }

    public void BrakeUp()
    {
        mobileBrake = false;
    }
}
