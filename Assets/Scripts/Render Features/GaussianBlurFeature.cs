using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static FullScreenFeature;

public class GaussianBlurFeature : ScriptableRendererFeature
{

    [System.Serializable]
    public class GaussianBlurSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public RenderTexture textureToModify;
        public Material material;

        [Tooltip("Standard deviation (spread) of the blur. Grid size is approx. 3x larger.")]
        public ClampedFloatParameter strength = new ClampedFloatParameter(0.0f, 0.0f, 15.0f);
    }

    [SerializeField] public GaussianBlurSettings settings;
    public class GaussianBlurPass : ScriptableRenderPass
    {

        const string ProfilerTag = "Gaussian Blur Pass";
        public GaussianBlurFeature.GaussianBlurSettings settings;
        RenderTargetIdentifier colorBuffer, temporaryBuffer;
        private int temporaryBufferID = Shader.PropertyToID("_TemporaryBuffer");

        public GaussianBlurPass(GaussianBlurFeature.GaussianBlurSettings passSettings)
        {
            this.settings = passSettings;
            this.renderPassEvent = settings.renderPassEvent;
        }



        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            if (settings.textureToModify == null)
            {
                colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
            }
            else
            {
                colorBuffer = settings.textureToModify.colorBuffer;
            }
            cmd.GetTemporaryRT(temporaryBufferID, descriptor, FilterMode.Point);
            temporaryBuffer = new RenderTargetIdentifier(temporaryBufferID);




            //Material configuration:

            int gridSize = Mathf.CeilToInt(settings.strength.value * 3.0f);

            if (gridSize % 2 == 0)
            {
                gridSize++;
            }

            settings.material.SetInteger("_GridSize", gridSize);
            settings.material.SetFloat("_Spread", settings.strength.value);
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
                //Blit to temp and back. Both passes because Gaussian blur is an O(2n) process of 2 passes. Horizontal + Vertical
                Blit(cmd, colorBuffer, temporaryBuffer, settings.material, 0);
                Blit(cmd, temporaryBuffer, colorBuffer, settings.material, 1);
            }

            // Execute the command buffer and release it.
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null) throw new ArgumentNullException("cmd");
            cmd.ReleaseTemporaryRT(temporaryBufferID);
        }
    }

    public GaussianBlurPass m_GaussianPass;
    /// <inheritdoc/>
    public override void Create()
    {
        m_GaussianPass = new GaussianBlurPass(settings);
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (renderingData.cameraData.cameraType != CameraType.Game)
            return;
        renderer.EnqueuePass(m_GaussianPass);
    }
}


