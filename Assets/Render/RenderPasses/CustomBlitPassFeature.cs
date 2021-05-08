using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experiemntal.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomBlitPassFeature : ScriptableRendererFeature
{

    [System.Serializable]

    public class CustomBlitSettings
    {
        public Material blitMaterial = null;
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
        public string passTag = "CustomBlitPassFeature";


    }

    public LayerMask layerMask = 0;



    public CustomBlitSettings settings = new CustomBlitSettings();


    CustomBlitPass m_ScriptablePass;

    public override void Create()
    {

        // Configures where the render pass should be injected.
        //        var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
        //        settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
        //     blitPass = new BlitPass(settings.Event, settings.blitMaterial, settings.blitMaterialPassIndex, name);
        //m_RenderTextureHandle.Init(settings.textureId);
        var passIndex = 0;

        m_ScriptablePass = new CustomBlitPass(
            settings.passTag,
            settings.Event,
            settings.blitMaterial,
            passIndex,
            name,
            layerMask
        );


    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {

        var src = renderer.cameraColorTarget;
        var dest = RenderTargetHandle.CameraTarget;

        if (settings.blitMaterial == null)
        {
            Debug.LogWarningFormat("Missing Blit Material. {0} blit pass will not execute. Check for missing reference in the assigned renderer.", GetType().Name);
            return;
        }

        m_ScriptablePass.Setup(src, dest);

        renderer.EnqueuePass(m_ScriptablePass);
    }




    class CustomBlitPass : ScriptableRenderPass
    {

        public Material blitMaterial = null;
        public int blitShaderPassIndex = 0;

        FilteringSettings m_FilteringSettings;
        List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

        public LayerMask layerMask;


        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }

        public FilterMode filterMode { get; set; }


        RenderTargetHandle m_TemporaryColorTexture;
        string m_ProfilerTag;
        ProfilingSampler m_ProfilingSampler;



        public void Setup(RenderTargetIdentifier source, RenderTargetHandle destination)
        {
            this.source = source;
            this.destination = destination;
        }


        public CustomBlitPass(
            string profilerTag,
            RenderPassEvent renderPassEvent,
            Material blitMaterial,
            int blitShaderPassIndex,
            string tag,
            LayerMask layerMask
            )
        {
            RenderQueueRange renderQueueRange = new RenderQueueRange();
            //renderQueueRange.lowerBound = 0;
            //renderQueueRange.upperBound = 2500;
            renderQueueRange = RenderQueueRange.opaque;

            m_ProfilerTag = profilerTag;
            m_ProfilingSampler = new ProfilingSampler(profilerTag);
            m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));

            //m_FilteringSettings.renderQueueRange = RenderQueueRange.opaque;
            //m_FilteringSettings.renderQueueRange = renderQueueRange;
            //m_FilteringSettings.layerMask = layerMask;
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);


            this.renderPassEvent = renderPassEvent;
            this.blitMaterial = blitMaterial;
            this.blitShaderPassIndex = blitShaderPassIndex;
            this.layerMask = layerMask;
            //m_TemporaryColorTexture.Init("_TemporaryColorTexture");

        }

        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in an performance manner.
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;
            cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, filterMode);
            Blit(cmd, source, m_TemporaryColorTexture.Identifier(), blitMaterial, blitShaderPassIndex);
            Blit(cmd, m_TemporaryColorTexture.Identifier(), source);




            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();



            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {


                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortFlags);
                drawSettings.overrideMaterial = blitMaterial;
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings);


            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);



        }

        /// Cleanup any allocated resources that were created during the execution of this render pass.
        public override void FrameCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
        }
    }

}


