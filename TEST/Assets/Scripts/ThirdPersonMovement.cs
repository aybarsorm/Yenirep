using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public float speed = 6f;
    public CinemachineFreeLook cine;
    private float gravity = 9.81f;
    public float turnSmoothTime = 0.1f;
    private float directionY;
    private float turnSmoothVelocity;
    float changeQ = 90.0f;
    float changeE = -90.0f;
    public Animator anim;

    private float newAngle;
    public float camTurnSpeed = 2;
    private void Awake()
    {
        newAngle = cine.m_XAxis.Value;
    }

    // Update is called once per frame
    void Update()
    {
        changeCameraAngle();
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0, vertical).normalized;

        Vector3 toGround = new Vector3(0, 0, 0).normalized;
        if (controller.isGrounded)
        {
            directionY = -1.5f;
            //Debug.Log("NO");
            if (direction.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle + 180f, ref turnSmoothVelocity, turnSmoothTime);
                transform.rotation = Quaternion.Euler(0f, angle, 0f);
                Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                controller.Move(moveDir.normalized * (speed * Time.deltaTime));
                
                anim.SetBool("Walk", true);
            }
            else
            {
                anim.ResetTrigger("Walk");
            }
        }
        else
        {
            //Debug.Log("YES");
            directionY -= gravity * Time.deltaTime;
            toGround.y = directionY;
            controller.Move(toGround * (speed * Time.deltaTime));
        }

        
    }

    void changeCameraAngle()
    {
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            newAngle += changeQ;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            newAngle += changeE;
        }
        
        cine.m_XAxis.Value = Mathf.Lerp(cine.m_XAxis.Value, newAngle, Time.fixedDeltaTime * camTurnSpeed);
    }
}
