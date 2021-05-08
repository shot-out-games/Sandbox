using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomRenderPassFeature : ScriptableRendererFeature
{

    public enum QueueType
    {
        Opaque,
        Transparent,
    }


    [System.Serializable]
    public class CustomRenderPassFeatureSettings
    {
        public string passTag = "CustomRenderPassFeature";
        public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;

        public FeatureFilterSettings filterSettings = new FeatureFilterSettings();

        public Material overrideMaterial = null;
        public int overrideMaterialPassIndex = 0;

        public bool overrideDepthState = false;
        public CompareFunction depthCompareFunction = CompareFunction.LessEqual;
        public bool enableWrite = true;

        public StencilStateData stencilSettings = new StencilStateData();
        

        public FeatureCameraSettings cameraSettings = new FeatureCameraSettings();
    }

    [System.Serializable]
    public class FeatureFilterSettings
    {
        // TODO: expose opaque, transparent, all ranges as drop down
        public QueueType RenderQueueType;
        public LayerMask LayerMask;
        public string[] PassNames;

        public FeatureFilterSettings()
        {
            RenderQueueType = QueueType.Opaque;
            LayerMask = 0;
        }
    }

    [System.Serializable]
    public class FeatureCameraSettings
    {
        public bool overrideCamera = false;
        public bool restoreCamera = true;
        public Vector4 offset;
        public float cameraFieldOfView = 60.0f;
    }


    public CustomRenderPassFeatureSettings passSettings = new CustomRenderPassFeatureSettings();

    class CustomRenderPass : ScriptableRenderPass
    {

        private RenderTargetIdentifier source { get; set; }
        private RenderTargetHandle destination { get; set; }



        FilteringSettings m_FilteringSettings;
        RenderStateBlock m_RenderStateBlock;
        List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();
        string m_ProfilerTag;
        ProfilingSampler m_ProfilingSampler;
        bool m_IsOpaque;
        private Material m_Material;

        static readonly int s_DrawObjectPassDataPropID = Shader.PropertyToID("_DrawObjectPassData");

        

        public CustomRenderPass(
            string profilerTag,
            bool opaque,
            RenderPassEvent evt,
            RenderQueueRange renderQueueRange,
            LayerMask layerMask,
            StencilState stencilState,
            int stencilReference,
            Material material
            )
        {

            renderQueueRange.lowerBound = 0;
            renderQueueRange.upperBound = 2500;

            m_Material = material;

            m_ProfilerTag = profilerTag;
            m_ProfilingSampler = new ProfilingSampler(profilerTag);
            m_ShaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            m_ShaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            m_ShaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            renderPassEvent = evt;
            m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            m_IsOpaque = opaque;

            if (stencilState.enabled)
            {
                m_RenderStateBlock.stencilReference = stencilReference;
                m_RenderStateBlock.mask = RenderStateMask.Stencil;
                m_RenderStateBlock.stencilState = stencilState;
            }
        }


        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {

            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // Global render pass data containing various settings.
                // x,y,z are currently unused
                // w is used for knowing whether the object is opaque(1) or alpha blended(0)

                //Vector4 drawObjectPassData = new Vector4(0.0f, 0.0f, 0.0f, (m_IsOpaque) ? 1.0f : 0.0f);
                //cmd.SetGlobalVector(s_DrawObjectPassDataPropID, drawObjectPassData);
                //context.ExecuteCommandBuffer(cmd);
                //cmd.Clear();


                var sortFlags = (m_IsOpaque) ? renderingData.cameraData.defaultOpaqueSortFlags : SortingCriteria.CommonTransparent;
                var drawSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortFlags);
                drawSettings.overrideMaterial = m_Material;
                //context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings, ref m_RenderStateBlock);
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref m_FilteringSettings);

            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);



        }
        public override void FrameCleanup(CommandBuffer cmd)
        {
        }
    }
//end pass

    CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        bool opaque = true;
        
        m_ScriptablePass = new CustomRenderPass(passSettings.passTag,
            opaque,
            renderPassEvent,
            new RenderQueueRange(), 
            passSettings.filterSettings.LayerMask,
            new StencilState(false), 
            passSettings.stencilSettings.stencilReference,
            passSettings.overrideMaterial


            );


    }

public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


