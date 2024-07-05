using System.Transactions;
using System.Collections;
using UnityEngine;
using EasyRoads3Dv3;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.DualShock;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;
    private CameraController cameraController;
    private ScreenFader screenFader;
    public float minMoveSpeed = 3.0f/3.6f;//km/h en m/s
    public float maxMoveSpeed = 7.0f/3.6f;
    public GameObject PedStartPos;
    public GameObject PedEndPos;
    private GameManager GameManager;
    public bool hasCrashed;
    private MarkerEvent markerEvent;
    public ERRoadNetwork roadNetwork;
    public ERRoad road;
    private Rigidbody rb;
    public float PedMove;
    private float totalPedDistance; // PedStartPos 和 PedEndPos 之间的总距离
    private DualShockGamepad PS5GamePad;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        GameManager = FindObjectOfType<GameManager>();
        cameraController = FindObjectOfType<CameraController>();
        markerEvent = FindObjectOfType<MarkerEvent>();
        screenFader=FindObjectOfType<ScreenFader>();
        hasCrashed = false;
        roadNetwork = new ERRoadNetwork();
        road=roadNetwork.GetRoadByName("200mRoad");
        UnityEngine.Vector3[] centerPoints = road.GetSplinePointsCenter();
        PedStartPos.transform.position = new UnityEngine.Vector3 (centerPoints[2].x+2.5f,centerPoints[2].y,centerPoints[2].z);
        transform.position = PedStartPos.transform.position;
        PedEndPos.transform.position= new UnityEngine.Vector3 (centerPoints[2].x-2.5f,centerPoints[2].y,centerPoints[2].z); 
        totalPedDistance = Mathf.Abs(PedStartPos.transform.position.x - PedEndPos.transform.position.x);
        PS5GamePad = InputSystem.GetDevice<DualShockGamepad>();
        if (PS5GamePad == null)
        {
            Debug.LogError("PS5 Gamepad not found!");
        }else{
            StartCoroutine(TriggerVibration(1.0f));
        }
    }

    // void Update()
    // {
    //     if (GameManager.canMove)
    //     {
    //         float moveInput = Input.GetAxis("Vertical");
    //         UnityEngine.Debug.Log("R thumbstick at "+moveInput);
    //         if (moveInput > 0)//128)//0
    //         {
    //             animator.SetBool("isWalking", true);
    //             animator.SetBool("goesBackwards", false);
    //         }
    //         else if (moveInput < 0)//128)//0
    //         {
    //             animator.SetBool("isWalking", false);
    //             animator.SetBool("goesBackwards", true);
    //         }
    //         else
    //         {
    //             animator.SetBool("isWalking", false);
    //             animator.SetBool("goesBackwards", false);
    //             characterController.SimpleMove(Vector3.zero);
    //         }

    //         // variable speeds of the player
    //         float speed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, Mathf.Abs(moveInput));
    //         UnityEngine.Vector3 moveDirection = transform.forward * moveInput * speed;//* Time.deltaTime;

    //         // move
    //         characterController.SimpleMove(moveDirection);

    //         if (transform.position.x >= PedStartPos.transform.position.x)
    //         {
    //             // Stop the player from moving further back
    //             transform.position = new Vector3(PedStartPos.transform.position.x, transform.position.y, transform.position.z);
    //         }

    //         if (transform.position.x <= PedEndPos.transform.position.x)
    //         {
    //             rb.velocity = Vector3.zero;
    //             characterController.SimpleMove(Vector3.zero);
    //             markerEvent.RecordHasArrived();
    //             GameManager.RestartTry();
    //         }
    //     }
    //     // if (Input.GetKeyDown(KeyCode.R))
    //     // {
    //     //     ResetPed();
    //     // }
    //     PedMove = Mathf.Clamp01((-transform.position.x + PedStartPos.transform.position.x) / totalPedDistance);
    //     UnityEngine.Debug.Log("PedMove: " + PedMove);
    // }
    void Update()
    {
        if (GameManager.canMove)
        {
            // 获取 thumbstick 控件
            Vector2Control thumbstick = PS5GamePad?.rightStick ?? null;

            // 检查 thumbstick 是否存在
            if (thumbstick != null)
            {
                // 获取 thumbstick 的值
                UnityEngine.Vector2 thumbstickValue = thumbstick.ReadValue();
                UnityEngine.Debug.Log("R thumbstick at "+thumbstickValue);
                // 检查 thumbstick 是否在默认位置
                bool isThumbstickAtDefault = thumbstickValue.Equals(Vector2.zero);

                if (thumbstickValue.y > 0 && !isThumbstickAtDefault)
                {
                    animator.SetBool("isWalking", true);
                    animator.SetBool("goesBackwards", false);

                    // variable speeds of the player
                    float speed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, Mathf.Abs(thumbstickValue.y));
                    UnityEngine.Vector3 moveDirection = transform.forward * thumbstickValue.y * speed;
                    characterController.SimpleMove(moveDirection);
                }
                else if (thumbstickValue.y < 0 && !isThumbstickAtDefault)
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("goesBackwards", true);

                    // variable speeds of the player
                    float speed = Mathf.Lerp(minMoveSpeed, maxMoveSpeed, Mathf.Abs(thumbstickValue.y));
                    UnityEngine.Vector3 moveDirection = transform.forward * thumbstickValue.y * speed;
                    characterController.SimpleMove(moveDirection);
                }
                else
                {
                    animator.SetBool("isWalking", false);
                    animator.SetBool("goesBackwards", false);
                    characterController.SimpleMove(Vector3.zero);
                }
            }

            if (transform.position.x >= PedStartPos.transform.position.x)
            {
                // Stop the player from moving further back
                transform.position = new Vector3(PedStartPos.transform.position.x, transform.position.y, transform.position.z);
            }

            if (transform.position.x <= PedEndPos.transform.position.x)
            {
                rb.velocity = Vector3.zero;
                characterController.SimpleMove(Vector3.zero);
                markerEvent.RecordHasArrived();
                GameManager.RestartTry();
            }
        }

        PedMove = Mathf.Clamp01((-transform.position.x + PedStartPos.transform.position.x) / totalPedDistance);
        UnityEngine.Debug.Log("PedMove: " + PedMove);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.StartsWith("V"))
        {
            GameManager.RestartTry();// in restarTry i called reset ped;
            UnityEngine.Debug.Log("Accident");
            hasCrashed = true;
            markerEvent.RecordHasCrashed();
            StartCoroutine(TriggerVibration(1.0f));
        }
    }
    IEnumerator TriggerVibration(float duration)
    {
        // 设置震动效果
        if (PS5GamePad != null)
        {
            // 设置震动效果
            PS5GamePad.SetMotorSpeeds(0.25f, 0.75f);
            UnityEngine.Debug.Log("Vroooooom");
            // 等待一段时间后停止震动
            yield return new WaitForSecondsRealtime(duration);

            // 停止震动
            PS5GamePad.SetMotorSpeeds(0f, 0f);
        }
        else
        {
            Debug.LogError("PS5 Gamepad not found! Unable to trigger vibration.");
        }
    }

    public void ResetPed()
    {
        cameraController.normalizedRotation = 0;
        GameManager.canMove = false;
        hasCrashed=false;
        animator.SetBool("isWalking", false);
        transform.position = PedStartPos.transform.position;
    }

}
