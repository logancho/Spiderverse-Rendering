using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NormalFeature : ScriptableRendererFeature
{
    //public LayerMask layerMask;
    public LayerMask normalsLayerMask;
    //public RenderTexture TargetTexture;
    public RenderTexture NormalsTexture;

    //public RenderPassEvent _event = RenderPassEvent.BeforeRenderingSkybox;
    public RenderPassEvent _NormalsEvent = RenderPassEvent.AfterRenderingOpaques;

    class NormalsPrePass : ScriptableRenderPass
    {
        private ProfilingSampler m_ProfilingSampler;
        private FilteringSettings m_FilteringSettings;
        private List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        private RenderTexture target;
        private Material depthNormalsMaterial;

        private RenderTargetHandle depthTex;

        public NormalsPrePass(RenderTexture targetTexture, LayerMask layerMask, Material mat)
        {
            m_ProfilingSampler = new ProfilingSampler("RenderNormals");
            m_FilteringSettings = new FilteringSettings(RenderQueueRange.opaque, layerMask);

            target = targetTexture;

            m_ShaderTagIdList.Add(new ShaderTagId("DepthOnly")); // Only render DepthOnly pass
            depthNormalsMaterial = mat;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(target);
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType != CameraType.Game)
                return;
            SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
            DrawingSettings drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterial = depthNormalsMaterial;

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings);
                //cmd.ClearRenderTarget(false, true, Color.black);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    NormalsPrePass m_NormalsPass;
    Material depthNormalsMaterial;


    /// <inheritdoc/>
    public override void Create()
    {
        //m_DepthPass = new DepthPrePass(TargetTexture, layerMask);
        //m_DepthPass.renderPassEvent = _event;

        //depthNormalsMaterial = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
        depthNormalsMaterial = CoreUtils.CreateEngineMaterial("Hidden/Normals Texture");
        //"Hidden/Normals Texture"
        m_NormalsPass = new NormalsPrePass(NormalsTexture, normalsLayerMask, depthNormalsMaterial);
        m_NormalsPass.renderPassEvent = _NormalsEvent;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        //renderer.EnqueuePass(m_DepthPass);
        if (renderingData.cameraData.cameraType == CameraType.Game)
            renderer.EnqueuePass(m_NormalsPass);
    }
}