using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenSpaceOutlines : ScriptableRendererFeature
{

    [System.Serializable] 
    private class ScreenSpaceOutlineSettings{

        [Header("Outline Settings")]
        public Color outlineColor = Color.black;
        [Range(0.0f, 20.0f)]
        public float outlineScale = 1.0f;

        [Header("Depth Settings")]
        [Range(0.0f, 500.0f)]
        public float depthThreshold = 1.5f;
        [Range(0.0f, 500.0f)]
        public float robertsCrossMultiplier = 100.0f;

        [Header("Normal Settings")]
        [Range(0.0f, 1.0f)]
        public float normalThreshold = 0.4f;

        [Header("View Space Normal Texture Settings")]
        public RenderTextureFormat colorFormat;
        public int depthBufferBits;
        public FilterMode filterMode;
        public Color backgroundColor = Color.clear;

        [Header("View Space Normal Texture Object Draw Settings")]
        public PerObjectData perObjectData;
        public bool enableDynamicBatching;
        public bool enableInstancing;
        
    }
    private class ScreenSpaceOutlinePass : ScriptableRenderPass{

        private ScreenSpaceOutlineSettings settings;
        private readonly List<ShaderTagId> shaderTagIdList;
        private readonly Material screenSpaceOutlineMaterial;
        private readonly Material normalsMaterial;
        private FilteringSettings filteringSettings;
        private RTHandle normals;
        private RendererList normalsRenderersList;
        RTHandle temporaryBuffer;

        //constructor
        public ScreenSpaceOutlinePass(RenderPassEvent renderPassEvent, LayerMask layerMask, ScreenSpaceOutlineSettings settings){
            this.renderPassEvent = renderPassEvent;
            this.settings = settings;

            screenSpaceOutlineMaterial = new Material(Shader.Find("Hidden/Outlines"));
            screenSpaceOutlineMaterial.SetColor("_OutlineColor", settings.outlineColor);
            screenSpaceOutlineMaterial.SetFloat("_OutlineScale", settings.outlineScale);

            normalsMaterial = new Material(Shader.Find("Hidden/ViewSpaceNormals"));


            shaderTagIdList = new List<ShaderTagId> {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefaultUnlit")
            };
            
        }

        //called before render pass
        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor normalsTextureDescriptor = cameraTextureDescriptor;
            normalsTextureDescriptor.colorFormat = normalsTextureSettings.colorFormat;
            normalsTextureDescriptor.depthBufferBits = normalsTextureSettings.depthBufferBits;
            cmd.GetTemporaryRT(normals.id, normalsTextureDescriptor, normalsTextureSettings.filterMode);
            ConfigureTarget(normals.Identifier());
            ConfigureClear(ClearFlag.All, normalsTextureSettings.backgroundColor);
        }

        //executes render pass
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if(!normalsMaterial){
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get();
            using(new ProfilingScope(cmd, new ProfilingSampler("SceneViewSpaceNormalsTextureCreation"))){
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
                drawingSettings.overrideMaterial = normalsMaterial;
                FilteringSettings filteringSettings = FilteringSettings.defaultValue;
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        //called when camera is finished rendering
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(normals.id);
        }

    }

    private class ScreenSpaceOutlinePass : ScriptableRenderPass{

    }

    [SerializeField] private RenderPassEvent renderPassEvent;
    [SerializeField] private ViewSpaceNormalTextureSettings viewSpaceNormalTextureSettings;

    private ViewSpaceNormalsTexturePass viewSpaceNormalsTexturePass;
    private ScreenSpaceOutlinePass screenSpaceOutlinePass;
    
    public override void Create()
    {
        viewSpaceNormalsTexturePass = new ViewSpaceNormalsTexturePass(renderPassEvent);
        screenSpaceOutlinePass =  new ScreenSpaceOutlinePass(renderPassEvent);

    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(viewSpaceNormalsTexturePass);
        renderer.EnqueuePass(screenSpaceOutlinePass);
    }

    [SerializeField] private LayerMask outlinesLayerMask;
}


