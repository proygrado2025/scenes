using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetHidenObject : MonoBehaviour
{
    static private MaterialUpdater currentHidenObject;
    private MaterialUpdater thisMaterialUpdater;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 startScale;

    public UpdateSceneRenderPipeline usrp;

    void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        startScale = transform.localScale;
        thisMaterialUpdater = GetComponent<MaterialUpdater>();

        if (thisMaterialUpdater.IsHiddenObject){
            currentHidenObject = thisMaterialUpdater;
        }
    }

    public void DoReset(){
        transform.position = startPosition;
        transform.rotation = startRotation;
        transform.localScale = startScale;

        if( Random.value < 0.75 ){
            currentHidenObject.IsHiddenObject = false;
            currentHidenObject.MaterialType = MaterialMapping.MaterialMappingEnum.grid_orange;

            thisMaterialUpdater.IsHiddenObject = true;
            thisMaterialUpdater.MaterialType = MaterialMapping.MaterialMappingEnum.fake_grid_orange;

            currentHidenObject = thisMaterialUpdater;

            usrp.UpdateRenderersMaterial();
        }
    }
}
