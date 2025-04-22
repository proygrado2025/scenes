using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Light[] lights;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Light light = RenderSettings.sun;
        Debug.Log("FW: " + light.transform.forward);

        // lights = FindObjectsOfType(typeof(Light)) as Light[];
        // foreach (Light light in lights)
        // {
        //     Debug.Log(light);
        // }
    }
}
