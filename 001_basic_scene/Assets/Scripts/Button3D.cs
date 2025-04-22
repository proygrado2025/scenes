using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button3D : MonoBehaviour
{
    public MaterialUpdater updater;
    public MaterialMapping.MaterialMappingEnum DefaultMaterial;
    public MaterialMapping.MaterialMappingEnum FoundObjectMaterial;
    public MaterialMapping.MaterialMappingEnum NotFoundObjectMaterial;
    public MaterialMapping.MaterialMappingEnum ManyObjectsMaterial;

    public UpdateSceneRenderPipeline usrp;

    public UnityEvent onRight;
    public UnityEvent onWrong;
    public UnityEvent onLeave;
    public UnityEvent onError;
    private HashSet<MaterialUpdater> objects = new HashSet<MaterialUpdater>();

    // ontrigger
    void OnTriggerEnter(Collider other)
    {
        var otherMaterialUpdater = other.GetComponent<MaterialUpdater>();
        if( otherMaterialUpdater != null ){
            objects.Add(otherMaterialUpdater);

            CheckAndUpdateMaterial();

            Debug.Log($"[ENTER]: {otherMaterialUpdater.IsHiddenObject}");
        }
    }
    void OnTriggerExit(Collider other)
    {
        var otherMaterialUpdater = other.GetComponent<MaterialUpdater>();
        if( otherMaterialUpdater != null ){
            objects.Remove(otherMaterialUpdater);
            CheckAndUpdateMaterial();

            Debug.Log($"[EXIT]: {otherMaterialUpdater.IsHiddenObject}");
        }
    }

    void CheckAndUpdateMaterial(){
        Debug.Log($"objects: {objects.Count}");
        if( objects.Count == 0){
            // original color
            updater.MaterialType = DefaultMaterial;
            usrp.UpdateRenderersMaterial();
            if(onLeave != null){
                onLeave.Invoke();
            }
        }
        else if( objects.Count == 1){
            // right or wrong color
            var enumerator = objects.GetEnumerator();
            enumerator.MoveNext();
            MaterialUpdater current = enumerator.Current;
            if(current.IsHiddenObject){
                updater.MaterialType = FoundObjectMaterial;
                usrp.UpdateRenderersMaterial();
                if(onRight != null){
                    onRight.Invoke();
                }
            }
            else{
                updater.MaterialType = NotFoundObjectMaterial;
                usrp.UpdateRenderersMaterial();
                if(onWrong != null){
                    onWrong.Invoke();
                }
            }
        }
        else{
            // error color
            updater.MaterialType = ManyObjectsMaterial;
            usrp.UpdateRenderersMaterial();
            if(onError != null){
                onError.Invoke();
            }
        }

    }
}
