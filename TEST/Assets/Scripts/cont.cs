using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cont : MonoBehaviour
{
    public float moveSpeed = 5f;
    
    public float gravity = 9.81f;
    

    public CharacterController controller;

    private float directionY;

   

    // Start is calle   d before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontalInput, 0, verticalInput).normalized;

        

        directionY -= gravity * Time.deltaTime;

        direction.y = directionY;

        //controller.Move(direction * (moveSpeed * Time.deltaTime));
    }
}