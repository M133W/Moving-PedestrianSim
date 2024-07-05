using System.Transactions;
// using System.Threading.Tasks.Dataflow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float rotationSpeed = 2.0f;
    public float maxHorizontalAngle = 90.0f;
    public float normalizedRotation = 0;
    private bool canRotateLeft = false;
    public float actualRotation=0;
    private ScreenFader screenFader;
    public Vector3 PedEyeWorldPos;
    void Start()
    {
        // hide the cursor
        Cursor.visible=false;
        screenFader=FindObjectOfType<ScreenFader>();
    }

    void Update()
    {
        if(!screenFader.isFading)
        {//Raw
            float horizontalRotationInput = Input.GetAxis("Horizontal")* rotationSpeed;
            UnityEngine.Debug.Log("L thumbstick at "+horizontalRotationInput);

            // if turn right
            if (horizontalRotationInput > 0)//128)//0
            {
                // horizontalRotation += horizontalRotationInput;
                // horizontalRotation = Mathf.Clamp(horizontalRotation, 0, maxHorizontalAngle);
                // canRotateLeft = true;
                normalizedRotation += horizontalRotationInput / maxHorizontalAngle; // 将角度映射到 [0, 1] 范围内
                normalizedRotation = Mathf.Clamp01(normalizedRotation);
                canRotateLeft = true;
            }
            // if turn left
            else if (canRotateLeft && horizontalRotationInput < 0)//128)//0
            {
                // horizontalRotation += horizontalRotationInput;
                // horizontalRotation = Mathf.Clamp(horizontalRotation, 0, maxHorizontalAngle);

                normalizedRotation += horizontalRotationInput / maxHorizontalAngle; // 将角度映射到 [0, 1] 范围内
                normalizedRotation = Mathf.Clamp01(normalizedRotation);

                // if at the initial view, stop turning left
                if (normalizedRotation == 0)
                {
                    canRotateLeft = false;
                }
            }
            actualRotation = normalizedRotation * maxHorizontalAngle;
            UnityEngine.Debug.Log("actualRot : "+actualRotation);

            PedEyeWorldPos = transform.position;
            // rotate the camera
            transform.localEulerAngles = new Vector3(0, actualRotation, 0);
            UnityEngine.Debug.Log("PedTurn : "+normalizedRotation);
        }
    }
}
