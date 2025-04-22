using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEnter : MonoBehaviour
{
    public UnityEvent onTrigger;

    void OnTriggerEnter(Collider other){
        if( onTrigger != null){
            onTrigger.Invoke();
        }

    }

}
