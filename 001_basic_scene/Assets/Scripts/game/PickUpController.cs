using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpController : MonoBehaviour{
    [Header("Pickup Settings")]
    [SerializeField]
    private Transform holdArea;
    private GameObject heldObj;
    private Rigidbody heldObjRB;

    [Header("Physics Parameters")]
    [SerializeField]
    private float pickupRange = 5.0f;
    [SerializeField]
    private float pickupForce = 150.0f;

    [SerializeField] private InputActionAsset _input;
    private InputAction inputAction;
    void Start()
    {
        inputAction =_input.FindAction("Grab");
        inputAction.started += ButtonPressed;
        inputAction.canceled += ButtonReleased;
    }

    private void ButtonReleased(InputAction.CallbackContext obj) {
        // Debug.Log("RELEASED");
        if( heldObj != null){
            // drop the object
            DropObject();
        }
        // Debug.Log("RELEASED END");
    }

    private void ButtonPressed(InputAction.CallbackContext obj) {
        // Debug.Log("PRESSED");
        if( heldObj == null){
            RaycastHit hit;
            if( Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickupRange)){
                // pick up our object
                PickUpObject(hit.transform.gameObject);
            }
        }
        // Debug.Log("PRESSED END");
    }

    void FixedUpdate()
    {
        if( heldObj != null){
            //move object
            MoveObject();
        }
    }

    void MoveObject()
    {
        if(Vector3.Distance(heldObj.transform.position, holdArea.position) > 0.1f){
            Vector3 moveDirection = (holdArea.position - heldObj.transform.position);
            heldObjRB.AddForce(moveDirection * pickupForce); 
        }
    }

    void PickUpObject(GameObject pickObj){
        // Debug.Log("PICKUP OBJECT");
        var tmpRB = pickObj.GetComponent<Rigidbody>();
        if( tmpRB != null ){
            heldObjRB = tmpRB;
            heldObjRB.useGravity = false;
            heldObjRB.drag = 10;
            heldObjRB.angularDrag = 10;
            // heldObjRB.constraints = RigidbodyConstraints.FreezeRotation;

            heldObjRB.transform.parent = holdArea;
            heldObj = pickObj;
        }
        // Debug.Log("PICKUP OBJECT END");
    }

    void DropObject(){
        // Debug.Log("DROP OBJECT");
        heldObjRB.useGravity = true;
        heldObjRB.drag = 1;
        heldObjRB.constraints = RigidbodyConstraints.None;

        heldObjRB.transform.parent = null;
        heldObj = null;
        // Debug.Log("DROP OBJECT END");
    }
}
