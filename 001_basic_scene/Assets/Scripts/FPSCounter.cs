using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using System.Linq;
using System.Collections.Generic;

public class FPSCounter : MonoBehaviour
{
    public TMP_Text fpsCounter;
    float lastChange = 0;
    int fpsCount = 0;

    // Update is called once per frame
    void Start()
    {
        RenderPipelineManager.beginFrameRendering += OnBeginFrameRendering;
    }

    void OnBeginFrameRendering(ScriptableRenderContext context, Camera[] cameras)
    {
        if( !cameras.Any( c => c == Camera.main)){
            return;
        }

        if( fpsCounter ){
            fpsCount++;
            if( Time.timeSinceLevelLoad >= lastChange+1){
                fpsCounter.text = $"FPS: {fpsCount}";
                lastChange = Time.timeSinceLevelLoad;
                fpsCount = 0;
            }
        }
    }
}
