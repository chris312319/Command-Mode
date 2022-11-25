using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
//using UnityEngine.InputSystem;
/*
public class ThirdPersonShooterController : MonoBehaviour
{
    Transform playerTransform;
    Animator animator;
    Transform cameraTransform;
    CharacterController characterController;
    PlayerSoundController playerSoundController;  //角色音效播放脚本

    #region 玩家姿态及相关动画参数阈值
    public enum PlayerPosture
    {
        Crouch,
        Stand,
        Falling,
        Jumping,
        Landing,
        Climbing
    };
    //[HideInInspector]
    public PlayerPosture playerPosture = PlayerPosture.Stand;

    float crouchThreshold = 0f;
    float standThreshold = 1f;
    float midairThreshold = 2.1f;
    float landingThreshold = 1f;
    #endregion

    //玩家运动状态
    public enum LocomotionState
    {
        Idle,
        Walk,
        Run
    };
    [HideInInspector]
    public LocomotionState locomotionState = LocomotionState.Idle;


    //玩家装备状态
    public enum ArmState
    {
        Normal,
        Aim
    };
    [HideInInspector]
    public ArmState armState = ArmState.Normal;

    //玩家不同状态的运动速度
    float crouchSpeed = 1.5f;
    float walkSpeed = 2.5f;
    float runSpeed = 5.5f;

    #region 输入值
    Vector2 moveInput;
    bool isRunPressed;
    bool isCrouchPressed;
    bool isAimPressed;
    bool isJumpPressed;
    #endregion

    #region 状态机参数的哈希值
    int postureHash;
    int moveSpeedHash;
    int turnSpeedHash;
    int verticalVelHash;
    int feetTweenHash;
    #endregion

    Vector3 playerMovementWorldSpace = Vector3.zero;
    Vector3 playerMovement = Vector3.zero;

    //重力
    public float gravity = -9.8f;

    //垂直方向速度
    float VerticalVelocity;

    //最大跳起高度
    public float maxHeight = 1.5f;

    //滞空左右脚状态
    float feetTween;

    #region 速度缓存池定义
    static readonly int CACHE_SIZE = 3;
    Vector3[] velCache = new Vector3[CACHE_SIZE];
    int currentChacheIndex = 0;
    Vector3 averageVel = Vector3.zero;
    #endregion

    //下落时加速度的倍数
    float fallMultiplier = 1.5f;

    //玩家是否着地
    bool isGrounded;

    //玩家是否可以跌落
    bool couldFall;

    //跌落的最小高度，小于此高度不会切换到跌落姿态
    float fallHeight = 0.5f;

    //是否处于跳跃CD状态
    bool isLanding;

    //地标检测射线的偏移量
    float groundCheckOffset = 0.5f;

    //跳跃的CD设置
    float jumpCD = 0.15f;

    //上一帧的动画nornalized时间
    float lastFootCycle = 0;

    #region 翻越相关
    PlayerSensor playerSensor;

    bool isClimbReady;

    int defaultClimbParameter = 0;
    int vaultParameter = 1;
    int lowClimbParameter = 2;
    int highClimbParameter = 3;
    int currentClimbparameter;

    Vector3 leftHandPosition;
    Vector3 rightHandPosition;
    Vector3 rightFootPosition;
    #endregion




    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform;
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        characterController = GetComponent<CharacterController>();
        playerSoundController = GetComponent<PlayerSoundController>();

        postureHash = Animator.StringToHash("玩家姿态");
        moveSpeedHash = Animator.StringToHash("移动速度");
        turnSpeedHash = Animator.StringToHash("转弯速度");
        verticalVelHash = Animator.StringToHash("垂直速度");
        feetTweenHash = Animator.StringToHash("左右脚");

        Cursor.lockState = CursorLockMode.Locked;

        playerSensor = GetComponent<PlayerSensor>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckGround();
        SwitchPlayerStates();
        CaculateGravity();
        Jump();
        CaculateInputDirection();
        SetupAnimator();
        PlayFootStep();
    }

    #region 输入相关
    public void GetMoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void GetRunInput(InputAction.CallbackContext ctx)
    {
        isRunPressed = ctx.ReadValueAsButton();
    }

    public void GetCrouchInput(InputAction.CallbackContext ctx)
    {
        isCrouchPressed = ctx.ReadValueAsButton();
    }

    public void GetAimInput(InputAction.CallbackContext ctx)
    {
        isAimPressed = ctx.ReadValueAsButton();
    }

    public void GetJumpInput(InputAction.CallbackContext ctx)
    {
        isJumpPressed = ctx.ReadValueAsButton();
    }

    #endregion


    /// <summary>
    /// 用于切换玩家的各种状态
    /// </summary>
    void SwitchPlayerStates()
    {
        switch (playerPosture)
        {
            //站立姿态，三(四)个转出
            case PlayerPosture.Stand:
                if (VerticalVelocity > 0)
                {
                    playerPosture = PlayerPosture.Jumping;
                }
                else if (!isGrounded && couldFall)
                {
                    playerPosture = PlayerPosture.Falling;
                }
                else if (isCrouchPressed)
                {
                    playerPosture = PlayerPosture.Crouch;
                }
                else if (isClimbReady)
                {
                    playerPosture = PlayerPosture.Climbing;
                }
                break;

            //下蹲姿态，两个转出
            case PlayerPosture.Crouch:
                if (!isGrounded && couldFall)
                {
                    playerPosture = PlayerPosture.Falling;
                }
                else if (!isCrouchPressed)
                {
                    playerPosture = PlayerPosture.Stand;
                }
                break;

            //摔落，一个转出
            case PlayerPosture.Falling:
                if (isGrounded)
                {
                    StartCoroutine(CoolDownJump());
                }
                if (isLanding)
                {
                    playerPosture = PlayerPosture.Landing;
                }
                break;

            //跳跃，一个转出
            case PlayerPosture.Jumping:
                if (isGrounded)
                {
                    StartCoroutine(CoolDownJump());
                }
                if (isLanding)
                {
                    playerPosture = PlayerPosture.Landing;
                }
                break;

            //着陆，一个转出
            case PlayerPosture.Landing:
                if (!isLanding)
                {
                    playerPosture = PlayerPosture.Stand;
                }
                break;

            //攀爬，一个转出
            case PlayerPosture.Climbing:

                if (!animator.GetCurrentAnimatorStateInfo(0).IsTag("攀爬") && !animator.IsInTransition(0))
                {
                    playerPosture = PlayerPosture.Stand;
                }
                break;
        }

        //现在这里统一把isClimbReady设置为false，后续添加一个专门的变量控制方法
        isClimbReady = false;

        if (moveInput.magnitude == 0)
        {
            locomotionState = LocomotionState.Idle;
        }
        else if (!isRunPressed)
        {
            locomotionState = LocomotionState.Walk;
        }
        else
        {
            locomotionState = LocomotionState.Run;
        }

        if (isAimPressed)
        {
            armState = ArmState.Aim;
        }
        else
        {
            armState = ArmState.Normal;
        }
    }


    /// <summary>
    /// 落地检测
    /// </summary>
    void CheckGround()
    {
        if (Physics.SphereCast(playerTransform.position + (Vector3.up * groundCheckOffset), characterController.radius, Vector3.down, out RaycastHit hit, groundCheckOffset - characterController.radius + 2 * characterController.skinWidth))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
            couldFall = !Physics.Raycast(playerTransform.position, Vector3.down, fallHeight);
        }
    }

    /// <summary>
    /// 计算跳跃CD
    /// </summary>
    /// <returns></returns>
    IEnumerator CoolDownJump()
    {
        landingThreshold = Mathf.Clamp(VerticalVelocity, -10, 0);
        landingThreshold /= 20f;
        landingThreshold += 1f;
        isLanding = true;
        playerPosture = PlayerPosture.Landing;
        playerSoundController.PlayLanding();
        yield return new WaitForSeconds(jumpCD);
        isLanding = false;
    }

    /// <summary>
    /// 计算下落速度
    /// </summary>
    void CaculateGravity()
    {
        if (playerPosture != PlayerPosture.Jumping && playerPosture != PlayerPosture.Falling)
        {
            if (!isGrounded)
            {
                VerticalVelocity += gravity * fallMultiplier * Time.deltaTime;
            }
            else
            {
                VerticalVelocity = gravity * Time.deltaTime;
            }
        }
        else
        {
            if (VerticalVelocity <= 0 || !isJumpPressed)
            {
                VerticalVelocity += gravity * fallMultiplier * Time.deltaTime;
            }
            else
            {
                VerticalVelocity += gravity * Time.deltaTime;
            }
        }

    }

    /// <summary>
    /// 跳跃方法
    /// </summary>
    void Jump()
    {
        if (playerPosture == PlayerPosture.Stand && isJumpPressed)
        {
            float velOffset;
            switch (locomotionState)
            {
                case LocomotionState.Run:
                    velOffset = 1f;
                    break;
                case LocomotionState.Walk:
                    velOffset = 0.5f;
                    break;

                case LocomotionState.Idle:
                    velOffset = 0f;
                    break;
                default:
                    velOffset = 0f;
                    break;
            }

            switch (playerSensor.ClimbDetection(playerTransform, playerMovementWorldSpace, velOffset))
            {
                case PlayerSensor.NextPlayerMovement.jump:
                    playerSoundController.PlayJumpEffort();
                    VerticalVelocity = Mathf.Sqrt(-2 * gravity * maxHeight);
                    feetTween = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);
                    feetTween = feetTween < 0.5f ? 1 : -1;
                    if (locomotionState == LocomotionState.Run)
                    {
                        feetTween *= 3;
                    }
                    else if (locomotionState == LocomotionState.Walk)
                    {
                        feetTween *= 2;
                    }
                    else
                    {
                        feetTween = Random.Range(0.5f, 1f) * feetTween;
                    }
                    break;

                case PlayerSensor.NextPlayerMovement.climbLow:
                    leftHandPosition = playerSensor.Ledge + Vector3.Cross(-playerSensor.ClimbHitNormal, Vector3.up) * 0.3f;   //左手的位置向左移0.3米
                    isClimbReady = true;
                    currentClimbparameter = lowClimbParameter;
                    break;

                case PlayerSensor.NextPlayerMovement.climbHigh:
                    rightHandPosition = playerSensor.Ledge + Vector3.Cross(playerSensor.ClimbHitNormal, Vector3.up) * 0.3f;        //右手的位置处于ledge右边0.3米
                    rightFootPosition = playerSensor.Ledge + Vector3.down * 1.2f;   //脚的位置在顶端以下1.2米
                    isClimbReady = true;
                    currentClimbparameter = highClimbParameter;
                    break;

                case PlayerSensor.NextPlayerMovement.vault:
                    rightHandPosition = playerSensor.Ledge;     //右手位置居中
                    isClimbReady = true;
                    currentClimbparameter = vaultParameter;
                    break;
            }
        }
    }


    /// <summary>
    /// 计算玩家输入相对于相机的方向
    /// </summary>
    void CaculateInputDirection()
    {
        Vector3 camForwardProjection = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        playerMovementWorldSpace = camForwardProjection * moveInput.y + cameraTransform.right * moveInput.x;
        playerMovement = playerTransform.InverseTransformVector(playerMovementWorldSpace);
    }


    /// <summary>
    /// 设置动画状态机的参数
    /// </summary>
    void SetupAnimator()
    {
        if (playerPosture == PlayerPosture.Stand)
        {
            animator.SetFloat(postureHash, standThreshold, 0.1f, Time.deltaTime);
            switch (locomotionState)
            {
                case LocomotionState.Idle:
                    animator.SetFloat(moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Walk:
                    animator.SetFloat(moveSpeedHash, playerMovement.magnitude * walkSpeed, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Run:
                    animator.SetFloat(moveSpeedHash, playerMovement.magnitude * runSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }
        else if (playerPosture == PlayerPosture.Crouch)
        {
            animator.SetFloat(postureHash, crouchThreshold, 0.1f, Time.deltaTime);
            switch (locomotionState)
            {
                case LocomotionState.Idle:
                    animator.SetFloat(moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                default:
                    animator.SetFloat(moveSpeedHash, playerMovement.magnitude * crouchSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }
        else if (playerPosture == PlayerPosture.Jumping)
        {
            animator.SetFloat(postureHash, midairThreshold);
            animator.SetFloat(verticalVelHash, VerticalVelocity);
            animator.SetFloat(feetTweenHash, feetTween);
        }
        else if (playerPosture == PlayerPosture.Landing)
        {
            animator.SetFloat(postureHash, landingThreshold, 0.03f, Time.deltaTime);
            switch (locomotionState)
            {
                case LocomotionState.Idle:
                    animator.SetFloat(moveSpeedHash, 0, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Walk:
                    animator.SetFloat(moveSpeedHash, playerMovement.magnitude * walkSpeed, 0.1f, Time.deltaTime);
                    break;
                case LocomotionState.Run:
                    animator.SetFloat(moveSpeedHash, playerMovement.magnitude * runSpeed, 0.1f, Time.deltaTime);
                    break;
            }
        }
        else if (playerPosture == PlayerPosture.Falling)
        {
            animator.SetFloat(postureHash, midairThreshold);
            animator.SetFloat(verticalVelHash, VerticalVelocity);
        }

        else if (playerPosture == PlayerPosture.Climbing)
        {
            animator.SetInteger("攀爬方式", currentClimbparameter);
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
            Debug.Log(info.IsName("空手运动"));
            Debug.Log(info.ToString());

            playerTransform.rotation = Quaternion.Lerp(playerTransform.rotation, Quaternion.LookRotation(-playerSensor.ClimbHitNormal), 0.5f);
            if (info.IsName("爬"))
            {
                currentClimbparameter = defaultClimbParameter;
                animator.MatchTarget(leftHandPosition, Quaternion.identity, AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.one, 0f), 0f, 0.1f);
                animator.MatchTarget(leftHandPosition + Vector3.up * 0.18f, Quaternion.identity, AvatarTarget.LeftHand, new MatchTargetWeightMask(Vector3.up, 0f), 0.1f, 0.3f);
            }
            else if (info.IsName("爬高"))
            {
                currentClimbparameter = defaultClimbParameter;
                animator.MatchTarget(rightFootPosition, Quaternion.identity, AvatarTarget.RightFoot, new MatchTargetWeightMask(Vector3.one, 0f), 0f, 0.13f);
                animator.MatchTarget(rightHandPosition, Quaternion.identity, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0f), 0.2f, 0.32f);
            }
            else if (info.IsName("翻越"))
            {
                currentClimbparameter = defaultClimbParameter;
                animator.MatchTarget(rightHandPosition, Quaternion.identity, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0f), 0.1f, 0.2f);
                animator.MatchTarget(rightHandPosition + Vector3.up * 0.1f, Quaternion.identity, AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0f), 0.35f, 0.45f);
            }
        }


        if (armState == ArmState.Normal && playerPosture != PlayerPosture.Jumping)
        {
            float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
            animator.SetFloat(turnSpeedHash, rad, 0.1f, Time.deltaTime);
            playerTransform.Rotate(0, rad * 200 * Time.deltaTime, 0f);
        }
    }


    /// <summary>
    /// 计算前三帧的速度平均值
    /// </summary>
    /// <param name="newVel">当前帧的速度平均值</param>
    /// <returns>平均速度</returns>
    Vector3 AverageVel(Vector3 newVel)
    {
        velCache[currentChacheIndex] = newVel;
        currentChacheIndex++;
        currentChacheIndex %= CACHE_SIZE;
        Vector3 average = Vector3.zero;
        foreach (Vector3 vel in velCache)
        {
            average += vel;
        }
        return average / CACHE_SIZE;
    }

    /// <summary>
    /// 播放脚步声
    /// </summary>
    void PlayFootStep()
    {
        if (playerPosture != PlayerPosture.Jumping && playerPosture != PlayerPosture.Falling)
        {
            if (locomotionState == LocomotionState.Walk || locomotionState == LocomotionState.Run)
            {
                float currentFootCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f); ;
                if ((lastFootCycle < 0.1 && currentFootCycle >= 0.1) || (currentFootCycle >= 0.6 && lastFootCycle < 0.6))
                {
                    playerSoundController.PlayFootStep();
                }
                lastFootCycle = currentFootCycle;
            }
        }
    }

    /// <summary>
    /// 回调方法
    /// </summary>
    private void OnAnimatorMove()
    {
        //攀爬
        if (playerPosture == PlayerPosture.Climbing)
        {
            characterController.enabled = false;

            animator.ApplyBuiltinRootMotion();
        }

        //正常地面移动
        else if (playerPosture != PlayerPosture.Jumping && playerPosture != PlayerPosture.Falling)
        {
            characterController.enabled = true;
            Vector3 playerDeltaMovement = animator.deltaPosition;
            playerDeltaMovement.y = VerticalVelocity * Time.deltaTime;
            characterController.Move(playerDeltaMovement);
            averageVel = AverageVel(animator.velocity);
        }

        //跳跃
        else
        {
            characterController.enabled = true;
            averageVel.y = VerticalVelocity;
            Vector3 playerDeltaMovement = averageVel * Time.deltaTime;
            characterController.Move(playerDeltaMovement);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rightHandPosition, 0.1f);
    }

}
*/