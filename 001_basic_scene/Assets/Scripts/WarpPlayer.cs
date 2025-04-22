using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WarpPlayer : MonoBehaviour
{
    public Transform player;
    public Transform corridorSrc;
    public Transform corridorDst;
    public UnityEvent onWrarp;

    void OnTriggerEnter(Collider other)
    {
        var otherTransform = other.transform;
        if( otherTransform == player){
            Debug.Log("ENTER:" + other.name);

            otherTransform.position = corridorDst.position + (player.position - corridorSrc.position);

            if( onWrarp != null){
                onWrarp.Invoke();
            }
        }

    }

}
