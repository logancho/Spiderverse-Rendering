using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ColorBufferFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class ColorBufferPassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public RenderTexture textureToCopyTo;
        //public Material material;
    }

    [SerializeField] public ColorBufferPassSettings settings;
    public class ColorBufferPass : ScriptableRenderPass
    {
        const string ProfilerTag = "Color Buffer Blit Pass";
        public ColorBufferFeature.ColorBufferPassSettings settings;
        RenderTargetIdentifier colorBuffer;
        //private int temporaryBufferID = Shader.PropertyToID("_TemporaryBuffer");

        public ColorBufferPass(ColorBufferFeature.ColorBufferPassSettings passSettings)
        {
            this.settings = passSettings;
            this.renderPassEvent = settings.renderPassEvent;
            //if (settings.material == null) settings.material = CoreUtils.CreateEngineMaterial("Shader Graphs/Invert");
        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            //if (settings.textureToCopyTo == null)
            //{
            colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
            //} else
            //{
            //    colorBuffer = settings.textureToModify.colorBuffer;
            //}
            

            //cmd.GetTemporaryRT(temporaryBufferID, descriptor, FilterMode.Point);
            //temporaryBuffer = new RenderTargetIdentifier(temporaryBufferID);
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(ProfilerTag)))
            {
                // HW 4 Hint: Blit from the color buffer to a temporary buffer and *back*.
                Blit(cmd, colorBuffer, settings.textureToCopyTo);
            }

            // Execute the command buffer and release it.
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            //if (cmd == null) throw new ArgumentNullException("cmd");
            //cmd.ReleaseTemporaryRT(temporaryBufferID);
        }
    }

    public ColorBufferPass m_ColorBufferPass;

    /// <inheritdoc/>
    public override void Create()
    {
        m_ColorBufferPass = new ColorBufferPass(settings);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game)
            return;
        renderer.EnqueuePass(m_ColorBufferPass);
    }
}


