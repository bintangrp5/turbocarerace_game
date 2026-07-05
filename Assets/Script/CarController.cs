using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public enum DrivetrainType { FWD, RWD, AWD }

    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentbreakForce;
    private bool isBreaking;
    public bool isAutoDrive = false;
    public float autoSpeed = 10f;

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

    [Header("Auto Start")]
    [SerializeField] private float autoStartSpeed = 12f; // ≈ 43 km/h
    private bool autoStart = false;

    [Header("Physics & Stability")]
    [SerializeField] private Vector3 centerOfMassOffset = new Vector3(0f, -0.8f, 0f); // Pusat massa diturunkan agar sangat stabil
    [SerializeField] private float downforce = 50f; // Gaya tekan ke bawah ekstra agar ban menempel di jalan

    [Header("UI Control")]
    public GameObject pedalGas;

    [Header("Nitro Settings")]
    [SerializeField] private float nitroMultiplier = 2f;
    private bool isNitroActive = false;

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

        // 🔥 CUKUP INI SAJA
        ResetInput();

        int kontrol = PlayerPrefs.GetInt("kontrol", 0);

        if (kontrol == 0)
        {
            Debug.Log("Mode Manual");
            isAutoDrive = false;

            autoStart = false;

            if (pedalGas != null)
                pedalGas.SetActive(true);
        }
        else
        {
            Debug.Log("Mode Otomatis");
            isAutoDrive = true;

            if (pedalGas != null)
                pedalGas.SetActive(false);
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
        // 🔥 AUTO MODE
        if (isAutoDrive)
        {
            horizontalInput = mobileSteer;
            verticalInput = 0f;
            isBreaking = mobileBrake;
            return;
        }

        // 🔥 MANUAL MODE (UTAMA DARI MOBILE)
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

        // 🔥 KEYBOARD (TIDAK PAKAI += BIAR GA NIMPA)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                verticalInput = 1f;
            else if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                verticalInput = -1f;

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                horizontalInput = 1f;
            else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                horizontalInput = -1f;

            if (Keyboard.current.spaceKey.isPressed)
                isBreaking = true;

            if (Keyboard.current.rKey.wasPressedThisFrame)
                ResetCarRotation();
        }

        // 🔥 GAMEPAD (SAMA, TIDAK PAKAI +=)
        if (Gamepad.current != null)
        {
            float gpVertical = Gamepad.current.leftStick.y.ReadValue();
            float gpHorizontal = Gamepad.current.leftStick.x.ReadValue();

            if (Mathf.Abs(gpVertical) > 0.1f)
                verticalInput = gpVertical;

            if (Mathf.Abs(gpHorizontal) > 0.1f)
                horizontalInput = gpHorizontal;

            if (Gamepad.current.buttonSouth.isPressed)
                isBreaking = true;

            if (Gamepad.current.buttonNorth.wasPressedThisFrame)
                ResetCarRotation();
        }

        // Clamp biar aman
        horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);
        verticalInput = Mathf.Clamp(verticalInput, -1f, 1f);
    }
    
    public void StartAutoDrive()
    {
        if (!isAutoDrive) return; // 🔥 cegah di manual

        autoStart = true;
        Debug.Log("AUTO DRIVE AKTIF!");
    }

    private void HandleMotor()
    {
        if (!canDrive)
        {
            // Hentikan semua
            frontLeftWheelCollider.motorTorque = 0f;
            frontRightWheelCollider.motorTorque = 0f;
            rearLeftWheelCollider.motorTorque = 0f;
            rearRightWheelCollider.motorTorque = 0f;
            return;
        }
        
        float currentMotorForce = motorForce;

        if (isNitroActive)
        {
            currentMotorForce *= nitroMultiplier;
        }

        float input = verticalInput;

        // 🔥 AUTO MODE
        if (isAutoDrive && autoStart)
        {
            input = 0.5f;
        }

        // 🔥 MANUAL MODE (ANTI NGESOT)
        if (!isAutoDrive && Mathf.Abs(verticalInput) < 0.01f)
        {
            input = 0f;
        }

        float motorTorqueValue = input * currentMotorForce;

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
        Debug.Log("GAS TEKAN");
        mobileThrottle = 1f;
    }

    public void GasUp()
    {
        Debug.Log("GAS LEPAS");
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

    public void NitroDown()
    {
        Debug.Log("NITRO ON");
        isNitroActive = true;
    }

    public void NitroUp()
    {
        Debug.Log("NITRO OFF");
        isNitroActive = false;
    }

    public void ResetInput()
    {
        mobileThrottle = 0f;
        mobileBrake = false;
        mobileSteer = 0f;
    }
}
