using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TemplateMouseClick : MonoBehaviour
{
    private Transform sign;
    public Transform character;
    void Start()
    {
        sign = GetComponent<Transform>();
    }

    void OnMouseDown()
    {
        float distance = Vector3.Distance (sign.position, character.position);
        if (distance > 1.0f)
        {
            Debug.Log("NOPE");
        }
        else
        {
            Debug.Log("YEAP");
        }
    }
}
