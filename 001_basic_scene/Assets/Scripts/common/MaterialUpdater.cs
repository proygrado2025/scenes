using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]

[RequireComponent(typeof(MeshRenderer))]
public class MaterialUpdater : MonoBehaviour
{
    public MaterialMapping.MaterialMappingEnum MaterialType;
    public bool IsHiddenObject;
    private MeshRenderer rendererRef;
 
    private Vector3 lastPosition;
    private Quaternion lastRotation;
    private MaterialPropertyBlock propertyBlock;
    private bool canUpdate = true;

    void OnEnable()
    {
        rendererRef = GetComponent<MeshRenderer>();
        UpdateSceneRenderPipeline.RegisterRenderer(this);
        lastPosition = transform.position;
        lastRotation = transform.rotation;

        RenderPipelineManager.beginFrameRendering += UpdateInvalidatePrevFrame;
    }

    void OnDisable()
    {
        UpdateSceneRenderPipeline.UnregisterRenderer(this);
        RenderPipelineManager.beginFrameRendering -= UpdateInvalidatePrevFrame;
    }

    public void UpdateMaterial(MaterialMapping mm){
        rendererRef.material = mm.GetMaterial(MaterialType);
    }

    void FixedUpdate()
    {
        canUpdate = true;
    }

    void UpdateInvalidatePrevFrame(ScriptableRenderContext context, Camera[] cameras)
    {
        if(!canUpdate){
            return;
        }
        if( propertyBlock == null){
            propertyBlock = new MaterialPropertyBlock();
        }

        int hasChanged = 0;
        if( Vector3.Distance(lastPosition, transform.position) > 0.001){
            // not the same position
            hasChanged = 1;
        }
        else{
            Vector3 lastEuler = lastRotation.eulerAngles;
            Vector3 newEuler = transform.rotation.eulerAngles;
            if( Vector3.Distance(lastEuler, newEuler) > 0.001f){
                // not the same rotation
                hasChanged = 1;
            }


        }

        rendererRef.GetPropertyBlock(propertyBlock);
        propertyBlock.SetInt("_InvalidatePrevFrame", hasChanged );
        rendererRef.SetPropertyBlock(propertyBlock);

        lastPosition = transform.position;
        lastRotation = transform.rotation;
        canUpdate = false;
    }

}
