using System;
using TMPro;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.UI;

public class MouseClick : MonoBehaviour
{
    public Transform sign;
    public Transform character;
    public Text textComponent;
    private string uzakYazi = "i need to get closer";
    //public string yakinYazi = "doldur";
    private float kaybolma = 2.0f;
    public float sayac = 0.0f;

    void Update()
    {
        if (sayac > 0)
        {
            
            sayac -= Time.deltaTime;
        }

        if (sayac < 0 && textComponent.text.Length != 0)
        {
            textComponent.text = "";
        }
        //Debug.Log(name + " " + kaybolma);
        //Debug.Log(name + " " + sayac);
    }

    private void OnMouseDown()
    {
        float distance = Vector3.Distance (sign.position, character.position);
        
        if (distance > 1.0f)
        {
            textComponent.text = uzakYazi;
            sayac = kaybolma;
        }
        else
        {
            textComponent.text = name;
            sayac = kaybolma;
        }
    }
}
