using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

public class ClearFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class ClearSettings
    {
        public RenderTexture texture;
        public RenderPassEvent Event = RenderPassEvent.BeforeRendering;
        public Color bg_color = Color.black;
    }

    [SerializeField] private ClearSettings settings;

    class ClearPass : ScriptableRenderPass
    {
        private ClearSettings settings;

        public ClearPass(ClearSettings i_settings)
        {
            this.settings = i_settings;
            this.renderPassEvent = i_settings.Event;
        }
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            //colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
            ConfigureTarget(settings.texture);
        }

        //Clear

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler("Clear Pass")))
            {
                RTClearFlags m_clearFlag = RTClearFlags.ColorDepth;
                cmd.ClearRenderTarget(m_clearFlag, settings.bg_color, 1.0f, 0);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    ClearPass m_ScriptablePass;
    CopyDepthPass m_CopyDepthPass;

    RenderTargetHandle depthTex1, depthTex2;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new ClearPass(settings);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType == CameraType.Game || renderingData.cameraData.cameraType == CameraType.Reflection)
            renderer.EnqueuePass(m_ScriptablePass);
    }
}


