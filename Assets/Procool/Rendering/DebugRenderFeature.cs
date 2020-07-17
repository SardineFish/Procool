using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Procool.Map;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Procool.Rendering
{
    public class DebugRenderFeature : ScriptableRendererFeature
    {
        public override void Create()
        {
            
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cameraType == CameraType.SceneView)
            {
                var pass = new RenderPass(renderer.cameraColorTarget);
                
                renderer.EnqueuePass(pass);
            }
        }

        class RenderPass : ScriptableRenderPass
        {
            private RenderTargetIdentifier cameraColorTarget;

            public RenderPass(RenderTargetIdentifier cameraColorTarget)
            {
                this.cameraColorTarget = cameraColorTarget;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get("DrawHexagonGrid");

                var mat = ShaderPool.Get("Procool/Editor/HexagonGrid");

                var camera = renderingData.cameraData.camera;
                var halfSize = new Vector2(camera.orthographicSize * camera.aspect, camera.orthographicSize);
                var worldRect = new Rect(-halfSize + camera.transform.position.ToVector2(), 2 * halfSize);
                
                cmd.SetGlobalVector("ViewWorldRect",
                    new Vector4(worldRect.x, worldRect.y, worldRect.width, worldRect.height));
                cmd.SetGlobalVector("PixelWorldSize", worldRect.size / camera.pixelRect.size);

                foreach (var grid in HexagonGrid.AssetsManager.Assets)
                {
                    cmd.SetGlobalVector("WorldOrigin", grid.transform.position);
                    cmd.SetGlobalFloat("Size", grid.size);
                    cmd.Blit(BuiltinRenderTextureType.None, cameraColorTarget, mat);
                }
                
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                
                CommandBufferPool.Release(cmd);
            }
        }
    }
}