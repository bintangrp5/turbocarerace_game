using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public enum DrivetrainType { FWD, RWD, AWD }

    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;
    
    [Header("Drive Settings")]
    public bool isAutoDrive = false;
    public float autoSpeed = 10f; // Kecepatan dasar auto
    public bool canDrive = true;

    private float mobileSteer = 0f;
    private float mobileThrottle = 0f;
    private bool mobileBrake = false;
    private CarStatus carStatus;
    private Rigidbody rb;
    private Rigidbody playerRigidbody;

    [Header("Drivetrain Settings")]
    [SerializeField] private DrivetrainType drivetrain = DrivetrainType.RWD;
    [SerializeField] private float motorForce, breakForce, maxSteerAngle;

    [Header("Auto Start")]
    [SerializeField] private float autoStartSpeed = 12f;
    private bool autoStart = false;

    [Header("Physics & Stability")]
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0f, -0.8f, 0f);
    [SerializeField] private float downforce = 50f;

    [Header("UI Control")]
    public GameObject pedalGas;

    [Header("Nitro Settings")]
    [SerializeField] private float nitroMultiplier = 2f;
    private bool isNitroActive = false;

    [Header("Wheel Colliders")]
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    [Header("Wheel Transforms")]
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private void Start()
    {
        carStatus = GetComponent<CarStatus>();
        rb = GetComponent<Rigidbody>();
        playerRigidbody = rb;

        if (rb != null) rb.centerOfMass = centerOfMassOffset;

        ResetInput();

        ApplySettingsFromPrefs();
    }

    public void ApplySettingsFromPrefs()
    {
        int kontrol = PlayerPrefs.GetInt("kontrol", 0);
        isAutoDrive = (kontrol != 0);

        if (isAutoDrive) autoStart = true;

        if (pedalGas != null) pedalGas.SetActive(!isAutoDrive);
        if (isAutoDrive) Debug.Log("Mode Otomatis Aktif");
    }

    private void Update() => GetInput();

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        ApplyDownforce();
    }

    private void GetInput()
    {
        if (isAutoDrive)
        {
            horizontalInput = mobileSteer;
            verticalInput = 0f; // Input manual tidak berpengaruh di auto
            isBreaking = mobileBrake;
            return;
        }

        horizontalInput = mobileSteer;
        verticalInput = mobileThrottle;
        isBreaking = mobileBrake;

        if (!canDrive || (carStatus != null && carStatus.IsDead))
        {
            isBreaking = true;
            return;
        }

        // Keyboard & Gamepad Input
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) verticalInput = 1f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) verticalInput = -1f;

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) horizontalInput = 1f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) horizontalInput = -1f;

            if (Keyboard.current.spaceKey.isPressed) isBreaking = true;
            if (Keyboard.current.rKey.wasPressedThisFrame) ResetCarRotation();
        }
    }

    private void HandleMotor()
    {
        if (!canDrive)
        {
            SetMotorTorque(0f);
            return;
        }
        
        float currentMotorForce = motorForce * (isNitroActive ? nitroMultiplier : 1f);
        float input = isAutoDrive && autoStart ? (autoSpeed / 20f) : verticalInput;

        if (!isAutoDrive && Mathf.Abs(verticalInput) < 0.01f) input = 0f;

        float motorTorqueValue = input * currentMotorForce;
        SetMotorTorque(motorTorqueValue);
        
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void SetMotorTorque(float torque)
    {
        if (drivetrain == DrivetrainType.FWD || drivetrain == DrivetrainType.AWD)
        {
            frontLeftWheelCollider.motorTorque = torque;
            frontRightWheelCollider.motorTorque = torque;
        }
        if (drivetrain == DrivetrainType.RWD || drivetrain == DrivetrainType.AWD)
        {
            rearLeftWheelCollider.motorTorque = torque;
            rearRightWheelCollider.motorTorque = torque;
        }
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = frontLeftWheelCollider.brakeTorque = 
        rearLeftWheelCollider.brakeTorque = rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wc, Transform wt)
    {
        wc.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wt.position = pos;
        wt.rotation = rot;
    }

    private void ApplyDownforce()
    {
        if (rb != null) rb.AddForce(-transform.up * downforce * rb.linearVelocity.magnitude);
    }

    public void StartAutoDrive()
    {
        if (isAutoDrive) autoStart = true;
    }

    private void Awake()
    {
        // 2. AMBIL KOMPONENNYA saat game dimulai
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public void BoostSpeed(float boostAmount)
    {
        // 1. Ubah satuan km/jam ke meter/detik (m/s)
        // 15 km/jam dibagi 3.6 = 4.16 m/s
        float boostInMPS = boostAmount / 3.6f;

        // 2. Tambahkan kecepatan secara linear (tidak menyentak)
        if (playerRigidbody != null)
        {
            // Ambil kecepatan saat ini, lalu tambahkan 15 km/jam ke arah depan
            playerRigidbody.linearVelocity += transform.forward * boostInMPS;
        }

        // 3. Update target kecepatan AI
        autoSpeed += boostAmount;

        Debug.Log("Boost Aktif! Menambah kecepatan tepat: " + boostAmount + " km/jam");
    }
    public void ResetCarRotation()
    {
        if (rb != null)
        {
            transform.position += Vector3.up * 1.5f;
            transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            rb.linearVelocity = rb.angularVelocity = Vector3.zero;
        }
    }

    // Di dalam CarController.cs
    public void StopCar()
    {
        canDrive = false; // Mematikan input (seperti yang sudah ada di HandleMotor)
        
        // Matikan mesin/kecepatan
        if (rb != null) 
        {
            rb.linearVelocity *= 0.5f; // Mobil berhenti seketika
            rb.angularVelocity = Vector3.zero;
        }
        
    }

    // Input UI Methods
    public void TurnLeftDown() => mobileSteer = -1f;
    public void TurnRightDown() => mobileSteer = 1f;
    public void TurnUp() => mobileSteer = 0f;
    public void GasDown() => mobileThrottle = 1f;
    public void GasUp() => mobileThrottle = 0f;
    public void BrakeDown() => mobileBrake = true;
    public void BrakeUp() => mobileBrake = false;
    public void NitroDown() => isNitroActive = true;
    public void NitroUp() => isNitroActive = false;
    public void ResetInput() { mobileThrottle = 0f; mobileBrake = false; mobileSteer = 0f; }
}