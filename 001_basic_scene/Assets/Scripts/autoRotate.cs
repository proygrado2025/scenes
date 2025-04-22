using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoRotate : MonoBehaviour
{
    public Vector3 delta;


    // Update is called once per frame
    void Update()
    {
        transform.rotation *= Quaternion.Euler(delta * Time.deltaTime);
    }
}
