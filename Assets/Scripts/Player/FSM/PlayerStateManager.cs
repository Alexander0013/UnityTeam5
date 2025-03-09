using UnityEngine;
using StarterAssets;
using System.Collections;
using UnityEngine.SceneManagement;
public class PlayerStateManager : MonoBehaviour
{
    public PlayerBaseState previousState = null; // track the old state
    public PlayerBaseState currentState;

    // Timer for weapon
    public float idleWeaponTimer = 0f;
    public bool firstEntry = true;
    public bool idleWeaponHide = true;
    
    // References to your input, animator, and controller.
    public StarterAssetsInputs Input;
    public Animator Animator;
    public CharacterController Controller;

    // A simple grounded flag – replace with your actual grounded logic.
    public bool isGrounded = true;

    // Reference to a slash VFX object (a child object or prefab with a ParticleSystem).
    public GameObject slashVFX;
    // Movement and combat parameters
    public float MoveSpeed = 2.0f;
    public float SprintSpeed = 5.335f;
    public float RotationSmoothTime = 0.12f;
    public float SpeedChangeRate = 10.0f;
    public float JumpHeight = 1.2f;
    public float Gravity = -15.0f;
    public float JumpTimeout = 0.50f;
    public float FallTimeout = 0.15f;

    // Internal fields for movement calculations
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private GameObject _mainCamera;

    private void Awake()
    {
        Input = GetComponent<StarterAssetsInputs>();
        Animator = GetComponent<Animator>();
        Controller = GetComponent<CharacterController>();
        _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        //DontDestroyOnLoad(gameObject);

    }

    private void Start()
    {
        _jumpTimeoutDelta = JumpTimeout;
        _fallTimeoutDelta = FallTimeout;
        // Start in Idle state.
        currentState = new IdleState();
        currentState.EnterState(this);
        Debug.Log("[PlayerStateManager] Entering Idle State");

    }

    private void Update()
    {
        // Let the current state handle high-level behavior
        currentState.UpdateState(this);

        // If not in Attack state, handle movement/jump
        if (!(currentState is AttackState))
        {
            UpdateMovement();
            UpdateJumpAndGravity();
        }
    }

    public void SwitchState(PlayerBaseState newState)
    {
        // Set previousState before we exit the old one.
        previousState = currentState;

        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    // Blend the Attack layer weight to 0
    public IEnumerator BlendAttackLayerWeight(int layerIndex, float blendDuration)
    {
        float startWeight = Animator.GetLayerWeight(layerIndex);
        float time = 0f;
        while (time < blendDuration)
        {
            time += Time.deltaTime;
            float newWeight = Mathf.Lerp(startWeight, 0f, time / blendDuration);
            Animator.SetLayerWeight(layerIndex, newWeight);
            yield return null;
        }
        Animator.SetLayerWeight(layerIndex, 0f);
    }

    // Blend the Attack layer weight to a target (e.g. 1)
    public IEnumerator BlendAttackLayerWeightTo(int layerIndex, float targetWeight, float blendDuration)
    {
        float startWeight = Animator.GetLayerWeight(layerIndex);
        float time = 0f;
        while (time < blendDuration)
        {
            time += Time.deltaTime;
            float newWeight = Mathf.Lerp(startWeight, targetWeight, time / blendDuration);
            Animator.SetLayerWeight(layerIndex, newWeight);
            yield return null;
        }
        Animator.SetLayerWeight(layerIndex, targetWeight);
    }

    // Example check for attack animation finishing
    public bool IsAttackAnimationFinished()
    {
        if (Animator != null)
        {
            var stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
            // "Attack" must match the name of your attack state
            return stateInfo.IsName("Attack") && stateInfo.normalizedTime >= 1f;
        }
        return true;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    // Basic movement logic
    public void UpdateMovement()
    {
        float targetSpeed = Input.sprint ? SprintSpeed : MoveSpeed;
        if (Input.move == Vector2.zero)
            targetSpeed = 0f;

        float currentHorizontalSpeed = new Vector3(Controller.velocity.x, 0f, Controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = Input.analogMovement ? Input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                Time.deltaTime * SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        Vector3 inputDirection = new Vector3(Input.move.x, 0f, Input.move.y).normalized;
        if (Input.move != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation,
                ref _rotationVelocity, RotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, rotation, 0f);
        }

        Vector3 targetDirection = Quaternion.Euler(0f, _targetRotation, 0f) * Vector3.forward;
        Controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                        new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);

        if (Animator != null)
        {
            Animator.SetFloat(Animator.StringToHash("Speed"), _animationBlend);
            Animator.SetFloat(Animator.StringToHash("MotionSpeed"), inputMagnitude);
        }
    }

    public void UpdateJumpAndGravity()
    {
        if (IsGrounded())
        {
            _fallTimeoutDelta = FallTimeout;
            if (Animator != null)
            {
                Animator.SetBool(Animator.StringToHash("Jump"), false);
                Animator.SetBool(Animator.StringToHash("FreeFall"), false);
            }

            if (_verticalVelocity < 0f)
                _verticalVelocity = -2f;

            if (Input.jump && _jumpTimeoutDelta <= 0f)
            {
                _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                if (Animator != null)
                    Animator.SetBool(Animator.StringToHash("Jump"), true);
            }
            if (_jumpTimeoutDelta >= 0f)
                _jumpTimeoutDelta -= Time.deltaTime;
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;
            if (_fallTimeoutDelta >= 0f)
                _fallTimeoutDelta -= Time.deltaTime;
            else if (Animator != null)
                Animator.SetBool(Animator.StringToHash("FreeFall"), true);

            Input.jump = false;
        }

        // Apply gravity
        if (_verticalVelocity < 53f)
            _verticalVelocity += Gravity * Time.deltaTime;
    }
}
