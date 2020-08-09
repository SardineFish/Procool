using Procool.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Procool.Rendering
{
    public class BackgroundBlur : ScriptableRendererFeature
    {
        public int pass = 2;
        private RenderPass _renderPass;
        public override void Create()
        {
            _renderPass = new RenderPass(this);
            _renderPass.renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _renderPass.cameraTarget = renderer.cameraColorTarget;
            renderer.EnqueuePass(_renderPass);
        }

        class RenderPass : ScriptableRenderPass
        {
            enum BlurPass : int
            {
                DownScale = 0,
                UpScale = 1,
            }
            public RenderTargetIdentifier cameraTarget;
            private int blurryScreen = Shader.PropertyToID("_BlurryScreenImage");
            private int blurSource = Shader.PropertyToID("_BlurSource");
            private BackgroundBlur settings;

            public RenderPass(BackgroundBlur settings)
            {
                this.settings = settings;
            }
            
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                cmd.GetTemporaryRT(blurryScreen, cameraTextureDescriptor);
                ConfigureTarget(blurryScreen);
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get("Blur");
                
                

                var mat = ShaderPool.Get("Procool/VFX/Blur");

                int src = 0;
                var downScale = 2;
                for (var i = 0; i < settings.pass; i++)
                {
                    var width = renderingData.cameraData.cameraTargetDescriptor.width / downScale;
                    var height = renderingData.cameraData.cameraTargetDescriptor.height / downScale;

                    var tempRT = Shader.PropertyToID($"__TempRT_{downScale}");
                    cmd.GetTemporaryRT(tempRT, width, height, 0, FilterMode.Bilinear);
                    if (i == 0)
                        cmd.Blit(cameraTarget, tempRT, mat, (int)BlurPass.DownScale);
                    else
                    {
                        cmd.Blit(src, tempRT, mat, (int) BlurPass.DownScale);
                        cmd.ReleaseTemporaryRT(src);
                    }

                    src = tempRT;

                    downScale *= 2;
                }

                for (downScale /= 4; downScale != 1; downScale /= 2)
                {
                    var width = renderingData.cameraData.cameraTargetDescriptor.width / downScale;
                    var height = renderingData.cameraData.cameraTargetDescriptor.height / downScale;

                    var tempRT = Shader.PropertyToID($"__TempRT_{downScale}");
                    cmd.GetTemporaryRT(tempRT, width, height, 0, FilterMode.Bilinear);
                    
                    cmd.Blit(src, tempRT, mat, (int) BlurPass.UpScale);
                    
                    cmd.ReleaseTemporaryRT(src);
                    src = tempRT;
                }
                
                Blit(cmd, src, blurryScreen, mat, (int) BlurPass.UpScale);
                cmd.ReleaseTemporaryRT(src);
                
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }

            public override void FrameCleanup(CommandBuffer cmd)
            {
                cmd.ReleaseTemporaryRT(blurryScreen);
            }
        }
    }
}