using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TMPro;
using UnityEngine.Rendering.MonteCarloRenderPipeline;



[Serializable]
public class RenderPipelineMapping : SerializableDictionaryBase<RenderPipelineAsset, MaterialMapping>{}

public struct KeyBinding{
    public string helpMenuText;
    public Func<string> helpMenuSingleValue;    // only one value for the setting
    public Func<bool> helpMenuMultipleBoolValue;  // multiple values for the setting
    public KeyControl PressedKey;
    public KeyControl PressedThisFrame;
    public Action<KeyBinding> Callback;
    public float floatParameter;
    public bool isPipelineAgnostic;

    void Keyboard(){
        helpMenuText = "";
        floatParameter = 0;
    }
}

[ExecuteInEditMode]
public class UpdateSceneRenderPipeline : MonoBehaviour
{
    public TMP_Text ui_info;
    // public RenderPipelineAsset defaultRenderPipelineAsset;

    public RenderPipelineMapping GlobalMapping;

    List<KeyValuePair<KeyControl, RenderPipelineAsset>> InputMapping = new List<KeyValuePair<KeyControl, RenderPipelineAsset>>();

    static List<MaterialUpdater> renderers = new List<MaterialUpdater>();

    static UpdateSceneRenderPipeline instance;
    private int currentIndex = 0;

    private bool showHelp = false;

    private List<KeyBinding> bindings = new List<KeyBinding>();

    static public void RegisterRenderer(MaterialUpdater r){
        renderers.Add(r);
        if( instance ){
            r.UpdateMaterial(instance.GlobalMapping[instance.InputMapping[instance.currentIndex].Value]);
        }

    }
    static public void UnregisterRenderer(MaterialUpdater r){
        renderers.Remove(r);
    }

    void Init(){
        if(Keyboard.current == null){
            return;
        }

        List<RenderPipelineAsset> globalKeys = new List<RenderPipelineAsset>(GlobalMapping.Keys);
        List<KeyControl> keyControls = new List<KeyControl>{
            Keyboard.current.digit1Key,
            Keyboard.current.digit2Key,
            Keyboard.current.digit3Key,
            Keyboard.current.digit4Key,
            Keyboard.current.digit5Key,
            Keyboard.current.digit6Key,
            Keyboard.current.digit7Key,
            Keyboard.current.digit8Key,
            Keyboard.current.digit9Key
        };

        for(int i = 0; i < Mathf.Min(globalKeys.Count, keyControls.Count); ++i){
            InputMapping.Add(new KeyValuePair<KeyControl, RenderPipelineAsset>(keyControls[i], globalKeys[i]));
        }

        // help
        bindings.Add(new KeyBinding{
            helpMenuText = "Hide HELP",
            PressedThisFrame = Keyboard.current.f1Key,
            Callback = (data) => { showHelp = !showHelp;},
            isPipelineAgnostic = true,
            });

        // // record
        // bindings.Add(new KeyBinding{
        //     helpMenuText = "START/STOP Recording",
        //     PressedThisFrame = Keyboard.current.zKey,
        //     Callback = (data) => { MonteCarloShaderBindings.isRecording = !MonteCarloShaderBindings.isRecording;}
        //     });


        // recursion
        bindings.Add(new KeyBinding{
            helpMenuText = "Recursion Depth",
            helpMenuSingleValue = () => { return MonteCarloShaderBindings.maxRecursionDepth.ToString(); },
            PressedKey = Keyboard.current.rKey,
            PressedThisFrame = Keyboard.current.jKey,
            Callback = UpdateMaxRecursionDepth,
            floatParameter = -1,
            isPipelineAgnostic = false,
            });
        bindings.Add(new KeyBinding{
            PressedKey = Keyboard.current.rKey,
            PressedThisFrame = Keyboard.current.kKey,
            Callback = UpdateMaxRecursionDepth,
            floatParameter = 1,
            isPipelineAgnostic = false,
            });

        // rays per pixel
        bindings.Add(new KeyBinding{
            helpMenuText = "Samples per pixel",
            helpMenuSingleValue = () => { return MonteCarloShaderBindings.samplesPerPixel.ToString(); },
            PressedKey = Keyboard.current.tKey,
            PressedThisFrame = Keyboard.current.jKey,
            Callback = UpdateSPP,
            floatParameter = -1,
            isPipelineAgnostic = false,
            });
        bindings.Add(new KeyBinding{
            PressedKey = Keyboard.current.tKey,
            PressedThisFrame = Keyboard.current.kKey,
            Callback = UpdateSPP,
            floatParameter = 1,
            isPipelineAgnostic = false,
            });

        // // rays per invalidated pixel
        // bindings.Add(new KeyBinding{
        //     helpMenuText = "Samples per invalidated pixel",
        //     helpMenuSingleValue = () => { return MonteCarloShaderBindings.samplesPerInvalidatedPixel.ToString(); },
        //     PressedKey = Keyboard.current.yKey,
        //     PressedThisFrame = Keyboard.current.jKey,
        //     Callback = UpdateSPPInvalidated,
        //     floatParameter = -1
        //     });
        // bindings.Add(new KeyBinding{
        //     PressedKey = Keyboard.current.yKey,
        //     PressedThisFrame = Keyboard.current.kKey,
        //     Callback = UpdateSPPInvalidated,
        //     floatParameter = 1
        //     });

        // denoiser
        bindings.Add(new KeyBinding{
            helpMenuText = "Denoiser Passes",
            helpMenuMultipleBoolValue = () => { return MonteCarloShaderBindings.UseDenoiserPreprocess; },
            PressedKey = Keyboard.current.qKey,
            PressedThisFrame = Keyboard.current.jKey,
            Callback = UpdateDenoiserPass,
            floatParameter = 1,
            isPipelineAgnostic = false,
            });
        bindings.Add(new KeyBinding{
            helpMenuMultipleBoolValue = () => { return MonteCarloShaderBindings.UseDenoiserRegression; },
            PressedKey = Keyboard.current.qKey,
            PressedThisFrame = Keyboard.current.kKey,
            Callback = UpdateDenoiserPass,
            floatParameter = 2,
            isPipelineAgnostic = false,
            });
        bindings.Add(new KeyBinding{
            helpMenuMultipleBoolValue = () => { return MonteCarloShaderBindings.UseDenoiserPostprocess; },
            PressedKey = Keyboard.current.qKey,
            PressedThisFrame = Keyboard.current.lKey,
            Callback = UpdateDenoiserPass,
            floatParameter = 3,
            isPipelineAgnostic = false,
            });

        // // Blend alpha preprocess
        // bindings.Add(new KeyBinding{
        //     helpMenuText = "Blend alpha preprocess",
        //     helpMenuSingleValue = () => { return MonteCarloShaderBindings.samplesPerInvalidatedPixel.ToString(); },
        //     PressedKey = Keyboard.current.cKey,
        //     PressedThisFrame = Keyboard.current.jKey,
        //     Callback = UpdateSPPInvalidated,
        //     floatParameter = -0.01f
        //     });
        // bindings.Add(new KeyBinding{
        //     PressedKey = Keyboard.current.cKey,
        //     PressedThisFrame = Keyboard.current.kKey,
        //     Callback = UpdateSPPInvalidated,
        //     floatParameter = 0.01f
        //     });

        Invoke( "UpdateDefault", 0.01f);
    }
    void UpdateDefault(){
        UpdateRenderPipeline(0);
    }
    void Start()
    {
        instance = this;
        if( InputMapping.Count == 0){
            Init();
        }
    }
    void OnEnable()
    {
        if( InputMapping.Count != 0){
            UpdateRenderPipeline(0);
        }
        else{
            Init();
        }
    }

    void OnValidate()
    {
        if( InputMapping.Count != 0){
            UpdateRenderPipeline(0);
        }
        else{
            Init();
        }
    }

    void LateUpdate()
    {
        for(int i = 0; i < InputMapping.Count; ++i){
            if( InputMapping[i].Key.wasPressedThisFrame){
                UpdateRenderPipeline(i);
                return;
            }
        }

        for( int i = 0; i < bindings.Count; ++i ){
            KeyBinding currentBinding = bindings[i];
            if( currentBinding.PressedKey == null || currentBinding.PressedKey.isPressed ){
                if( currentBinding.PressedThisFrame != null && currentBinding.PressedThisFrame.wasPressedThisFrame){
                    currentBinding.Callback(currentBinding);
                }
            }
        }

        UpdateUI();
    }

    void UpdateUI(){
        System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();

        // strBuilder.AppendLine($"{MonteCarloShaderBindings.maxRecursionDepth} => {MonteCarloShaderBindings.fileIndex.ToString("00000")}");

        if( showHelp ){
            for(int i = 0; i < InputMapping.Count; ++i){

                string strRPA = InputMapping[i].Value.ToString();
                strBuilder.AppendLine($"{i+1}: {(i==currentIndex?">":"")}{strRPA.Substring(0, strRPA.IndexOf("Render"))}");
            }

            strBuilder.AppendLine(" ==== ");

            int bindingIndex = 0;
            while(bindingIndex < bindings.Count){
                KeyBinding currentBinding = bindings[bindingIndex];
                if( !currentBinding.isPipelineAgnostic && !GlobalMapping[InputMapping[currentIndex].Value].isMonteCarlo){
                    ++bindingIndex;
                    continue;
                }
                if( currentBinding.PressedKey != null){

                    string selectedPre = currentBinding.PressedKey.isPressed ? "<<" : "";
                    string selectedPost = currentBinding.PressedKey.isPressed ? ">>" : "";



                    strBuilder.Append($"[{currentBinding.PressedKey.displayName}] {selectedPre}{currentBinding.helpMenuText}{selectedPost}: { (currentBinding.helpMenuSingleValue == null ? "" : currentBinding.helpMenuSingleValue()) } ");

                    do{
                        if( bindings[bindingIndex].helpMenuMultipleBoolValue != null || currentBinding.PressedKey.isPressed){
                            bool selected = bindings[bindingIndex].helpMenuMultipleBoolValue != null && bindings[bindingIndex].helpMenuMultipleBoolValue();
                            if( selected ){
                                strBuilder.Append($"<[{bindings[bindingIndex].PressedThisFrame.displayName}]> ");
                            }
                            else{
                                strBuilder.Append($"({bindings[bindingIndex].PressedThisFrame.displayName}{((bindings[bindingIndex].floatParameter<0) ? "↓" : "↑")}) ");
                            }
                        }
                        ++bindingIndex;
                    }while( (bindingIndex < bindings.Count) && (bindings[bindingIndex].PressedKey == bindings[bindingIndex-1].PressedKey));

                    strBuilder.AppendLine("");
                }
                else{
                    strBuilder.AppendLine($"[{currentBinding.PressedThisFrame.displayName}] {currentBinding.helpMenuText}");
                    ++bindingIndex;
                }
            }
        }
        else{
            strBuilder.AppendLine("Press F1 to show HELP");
        }

        if(ui_info){
            ui_info.text = strBuilder.ToString();
        }

    }

    void UpdateRenderPipeline(int index){
        currentIndex = index;

        GraphicsSettings.renderPipelineAsset = InputMapping[index].Value;

        UpdateRenderersMaterial();
        // for(int i =0; i < renderers.Count; ++i){
        //     renderers[i].UpdateMaterial(GlobalMapping[InputMapping[index].Value]);
        // }
    }

    public void UpdateRenderersMaterial(){
        for(int i =0; i < renderers.Count; ++i){
            renderers[i].UpdateMaterial(GlobalMapping[InputMapping[currentIndex].Value]);
        }
    }

    public void UpdateMaxRecursionDepth(KeyBinding inData){
        if (GraphicsSettings.currentRenderPipeline != null)
        {
            var pipelineAsset = GraphicsSettings.currentRenderPipeline as MonteCarloRenderPipelineAsset;
            if( pipelineAsset != null){
                pipelineAsset.maxRecursionDepth =

                MonteCarloShaderBindings.maxRecursionDepth = (uint)Mathf.Max(0, MonteCarloShaderBindings.maxRecursionDepth + inData.floatParameter);
            }
        }
    }

    public void UpdateSPP(KeyBinding inData){
        if (GraphicsSettings.currentRenderPipeline != null)
        {
            var pipelineAsset = GraphicsSettings.currentRenderPipeline as MonteCarloRenderPipelineAsset;
            if( pipelineAsset != null){
                pipelineAsset.spp =

                MonteCarloShaderBindings.samplesPerPixel = (uint)Mathf.Max(0, MonteCarloShaderBindings.samplesPerPixel + inData.floatParameter);
            }
        }
    }

    // public void UpdateSPPInvalidated(KeyBinding inData){
    //     if (GraphicsSettings.currentRenderPipeline != null)
    //     {
    //         var pipelineAsset = GraphicsSettings.currentRenderPipeline as MonteCarloRenderPipelineAsset;
    //         // if( pipelineAsset != null){
    //         //     pipelineAsset.spp_invalidated =

    //         //     MonteCarloShaderBindings.samplesPerInvalidatedPixel = (uint)Mathf.Max(0, MonteCarloShaderBindings.samplesPerInvalidatedPixel + inData.floatParameter);
    //         // }
    //     }
    // }

    // public void UpdateDenoiserPass(int pass){
    public void UpdateDenoiserPass(KeyBinding data){
        if (GraphicsSettings.currentRenderPipeline != null)
        {
            var pipelineAsset = GraphicsSettings.currentRenderPipeline as MonteCarloRenderPipelineAsset;
            if( pipelineAsset != null){

                switch(data.floatParameter){
                    case 1:{
                        pipelineAsset.UseDenoiserPreprocess =
                        MonteCarloShaderBindings.UseDenoiserPreprocess ^= true;
                        break;
                    }
                    case 2:{
                        pipelineAsset.UseDenoiserRegression =
                        MonteCarloShaderBindings.UseDenoiserRegression ^= true;
                        break;
                    }
                    case 3:{
                        pipelineAsset.UseDenoiserPostprocess =
                        MonteCarloShaderBindings.UseDenoiserPostprocess ^= true;
                        break;
                    }
                }
            }
        }
    }

}

