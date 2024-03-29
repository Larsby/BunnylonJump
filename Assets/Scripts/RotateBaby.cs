using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateBaby : MonoBehaviour {
    public int direction = 1;
    void Update()
    {
    //     transform.Rotate(Vector3.right * Time.deltaTime*50);
        if(direction == 1)
        transform.Rotate(Vector3.forward * Time.deltaTime*55 );
        else
            transform.Rotate(Vector3.back * Time.deltaTime * 55);

    }
}