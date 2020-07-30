using System.Collections.Generic;
using Procool.Map;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Procool.Rendering
{
    public class LODPass : ScriptableRendererFeature
    {
        [SerializeField] private List<float> lodSize = new List<float>();
        public override void Create()
        {
            
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            var pass = new RenderPass(this, renderer.cameraColorTarget);

            renderer.EnqueuePass(pass);
        }

        class RenderPass : ScriptableRenderPass
        {
            private RenderTargetIdentifier cameraColorTarget;
            private ShaderTagId PassName = new ShaderTagId("OpaqueWithLOD");
            private int GlobalLOD = Shader.PropertyToID("_GlobalLOD");
            private LODPass Settings;

            public RenderPass(LODPass settings, RenderTargetIdentifier cameraColorTarget)
            {
                Settings = settings;
                this.cameraColorTarget = cameraColorTarget;
                this.renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var camera = renderingData.cameraData.camera;
                var halfHeight = 0f;
                if (camera.orthographic)
                {
                    halfHeight = camera.orthographicSize;
                }
                else
                {
                    halfHeight = Mathf.Abs(camera.transform.position.z) *
                                 Mathf.Tan(camera.fieldOfView / 2 * Mathf.Deg2Rad);
                }

                var lod = Settings.lodSize.Count - 1;
                for (var i = 0; i < Settings.lodSize.Count; i++)
                {
                    if (Settings.lodSize[i] > halfHeight)
                    {
                        lod = i - 1;
                        break;
                    }
                }

                var cmd = CommandBufferPool.Get();
                cmd.SetGlobalInt(GlobalLOD, lod);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
                
                var drawingSettings = new DrawingSettings(PassName, new SortingSettings(renderingData.cameraData.camera)
                {
                    criteria = renderingData.cameraData.defaultOpaqueSortFlags,
                });
                var filterSettings = new FilteringSettings(RenderQueueRange.opaque);
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filterSettings);
            }
        }
    }
}