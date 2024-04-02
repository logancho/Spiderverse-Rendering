using System;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ObjectIDPass : ScriptableRenderPass
{
    RenderQueueType renderQueueType;
    FilteringSettings m_FilteringSettings;
    ObjectIDFeature.CustomCameraSettings m_CameraSettings;
    string m_ProfilerTag;
    ProfilingSampler m_ProfilingSampler;

    public Material overrideMaterial { get; set; }
    public int overrideMaterialPassIndex { get; set; }
    private RenderTexture target;

    List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

    public void SetDetphState(bool writeEnabled, CompareFunction function = CompareFunction.Less)
    {
        m_RenderStateBlock.mask |= RenderStateMask.Depth;
        m_RenderStateBlock.depthState = new DepthState(writeEnabled, function);
    }

    public void SetStencilState(int reference, CompareFunction compareFunction, StencilOp passOp, StencilOp failOp, StencilOp zFailOp)
    {
        StencilState stencilState = StencilState.defaultValue;
        stencilState.enabled = true;
        stencilState.SetCompareFunction(compareFunction);
        stencilState.SetPassOperation(passOp);
        stencilState.SetFailOperation(failOp);
        stencilState.SetZFailOperation(zFailOp);

        m_RenderStateBlock.mask |= RenderStateMask.Stencil;
        m_RenderStateBlock.stencilReference = reference;
        m_RenderStateBlock.stencilState = stencilState;
    }

    RenderStateBlock m_RenderStateBlock;

    public ObjectIDPass(RenderTexture texture, string profilerTag, RenderPassEvent renderPassEvent, string[] shaderTags, RenderQueueType renderQueueType, int layerMask, ObjectIDFeature.CustomCameraSettings cameraSettings)
    {
        base.profilingSampler = new ProfilingSampler(nameof(ObjectIDPass));
        this.target = texture;
        m_ProfilerTag = profilerTag;
        m_ProfilingSampler = new ProfilingSampler(profilerTag);
        this.renderPassEvent = renderPassEvent;
        this.renderQueueType = renderQueueType;
        this.overrideMaterial = null;
        this.overrideMaterialPassIndex = 0;
        RenderQueueRange renderQueueRange = (renderQueueType == RenderQueueType.Transparent)
            ? RenderQueueRange.transparent
            : RenderQueueRange.opaque;

        /////

        //renderQueueRange = RenderQueueRange.all;

        /////

        m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);

        if (shaderTags != null && shaderTags.Length > 0)
        {
            foreach (var passName in shaderTags)
                m_ShaderTagIdList.Add(new ShaderTagId(passName));
        }
        else
        {
            m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForwardOnly"));
        }

        m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
        m_CameraSettings = cameraSettings;
    }

    //public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
    //{
    //    //ConfigureTarget(target);
    //}

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        //RTHandle color = RTHandles.Initialize()
        //color.
        RTHandle target_rt = RTHandles.Alloc(target);
        //ConfigureTarget(target.colorBuffer);
        ConfigureTarget(target_rt);
        //RTHandle bruh = target;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        SortingCriteria sortingCriteria = (renderQueueType == RenderQueueType.Transparent)
            ? SortingCriteria.CommonTransparent
            : renderingData.cameraData.defaultOpaqueSortFlags;

        sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;

        DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);

        drawingSettings.overrideMaterial = overrideMaterial;
        drawingSettings.overrideMaterialPassIndex = overrideMaterialPassIndex;


        ref CameraData cameraData = ref renderingData.cameraData;
        Camera camera = cameraData.camera;

        // In case of camera stacking we need to take the viewport rect from base camera
        //Rect pixelRect = renderingData.cameraData.pixelRect;
        //renderingData.cameraData.pixelRect
        float cameraAspect = (float)camera.pixelWidth / (float)renderingData.cameraData.camera.pixelHeight;

        // NOTE: Do NOT mix ProfilingScope with named CommandBuffers i.e. CommandBufferPool.Get("name").
        // Currently there's an issue which results in mismatched markers.
        CommandBuffer cmd = CommandBufferPool.Get();
        using (new ProfilingScope(cmd, m_ProfilingSampler))
        {
            context.ExecuteCommandBuffer(cmd);

            //RTClearFlags m_clearFlag = RTClearFlags.Color;
            //cmd.ClearRenderTarget(m_clearFlag, Color.black, 1.0f, 0);

            //cmd.Clear();

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings, ref m_RenderStateBlock);
        }
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
        //target.Release();
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        if (cmd == null) throw new ArgumentNullException("cmd");

        target.Release();

    }
}
