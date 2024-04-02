using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum RenderQueueType
{
    Opaque,
    Transparent,
}

[ExcludeFromPreset]
public class ObjectIDFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class ObjectIDSettings
    {
        public string passTag = "Object ID Feature";
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;

        public RenderTexture TargetTexture;

        public FilterSettings filterSettings = new FilterSettings();

        public Material overrideMaterial = null;
        public int overrideMaterialPassIndex = 0;

        public bool overrideDepthState = false;
        public CompareFunction depthCompareFunction = CompareFunction.LessEqual;
        public bool enableWrite = true;

        public StencilStateData stencilSettings = new StencilStateData();

        public CustomCameraSettings cameraSettings = new CustomCameraSettings();
    }

    [System.Serializable]
    public class FilterSettings
    {
        // TODO: expose opaque, transparent, all ranges as drop down
        public RenderQueueType RenderQueueType;
        public LayerMask LayerMask;
        public string[] PassNames;

        public FilterSettings()
        {
            RenderQueueType = RenderQueueType.Opaque;
            LayerMask = 0;
        }
    }

    [System.Serializable]
    public class CustomCameraSettings
    {
        public bool overrideCamera = false;
        public bool restoreCamera = true;
        public Vector4 offset;
        public float cameraFieldOfView = 60.0f;
    }

    public ObjectIDSettings settings = new ObjectIDSettings();
    ObjectIDPass objectIDPass;

    public override void Create()
    {
        FilterSettings filter = settings.filterSettings;

        // Render Objects pass doesn't support events before rendering prepasses.
        // The camera is not setup before this point and all rendering is monoscopic.
        // Events before BeforeRenderingPrepasses should be used for input texture passes (shadow map, LUT, etc) that doesn't depend on the camera.
        // These events are filtering in the UI, but we still should prevent users from changing it from code or
        // by changing the serialized data.
        if (settings.Event < RenderPassEvent.BeforeRenderingPrePasses)
            settings.Event = RenderPassEvent.BeforeRenderingPrePasses;

        objectIDPass = new ObjectIDPass(settings.TargetTexture, settings.passTag, settings.Event, filter.PassNames,
            filter.RenderQueueType, filter.LayerMask, settings.cameraSettings);

        objectIDPass.overrideMaterial = settings.overrideMaterial;
        objectIDPass.overrideMaterialPassIndex = settings.overrideMaterialPassIndex;

        if (settings.overrideDepthState)
            objectIDPass.SetDetphState(settings.enableWrite, settings.depthCompareFunction);

        if (settings.stencilSettings.overrideStencilState)
            objectIDPass.SetStencilState(settings.stencilSettings.stencilReference,
                settings.stencilSettings.stencilCompareFunction, settings.stencilSettings.passOperation,
                settings.stencilSettings.failOperation, settings.stencilSettings.zFailOperation);
    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game)
            renderer.EnqueuePass(objectIDPass);
    }


}


