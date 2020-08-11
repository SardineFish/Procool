using System;
using System.Linq;
using Procool.GamePlay.Weapon;
using Procool.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Procool.Rendering
{
    public class BulletPass : ScriptableRendererFeature
    {
        private RenderPass pass;
        public override void Create()
        {
            pass = new RenderPass();
            pass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(pass);
        }

        class RenderPass : ScriptableRenderPass
        {
            private ComputeBuffer argsBuffer;
            private ComputeBuffer dataBuffer;
            private uint[] args = new uint[5];
            private int IDBulletData = Shader.PropertyToID("BulletDataBuffer");
            private Mesh mesh;

            struct InstanceData
            {
                public Vector3 Positions;
                public float Size;
                public Color PrimaryColor;
                public Color SecondaryColor;
            }

            public RenderPass()
            {
                mesh = Utility.GenerateQuadXY();
            }
            
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                UpdateData();

                var cmd = CommandBufferPool.Get("Render Bullets");
                cmd.SetGlobalBuffer(IDBulletData, dataBuffer);
                cmd.DrawMeshInstancedIndirect(mesh, 0, ShaderPool.Get("Procool/Bullet"), 0, argsBuffer);
                
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                CommandBufferPool.Release(cmd);
            }

            unsafe void UpdateData()
            {
                // var bullets = DamageEntity.AssetsManager.Assets
                //     .Where(entity => entity.gameObject && entity.gameObject.activeInHierarchy)
                //     .Select(entity => new InstanceData()
                //     {
                //         Positions = entity.transform.position,
                //         Size = entity.BulletVfx.BulletSize,
                //         PrimaryColor = entity.BulletVfx.PrimaryColor,
                //         SecondaryColor = entity.BulletVfx.SecondaryColor,
                //     })
                //     .ToList();
                var bullets = Utils.ListPool<InstanceData>.Get();
                bullets.Clear();
                bullets.Capacity = Mathf.Max(bullets.Capacity, 8192);
                foreach (var entity in DamageEntity.AssetsManager.RawAssetsList)
                {
                    if(entity && entity.gameObject.activeInHierarchy)
                        bullets.Add(new InstanceData()
                        {
                            Positions = entity.transform.position,
                            Size = entity.BulletVfx.BulletSize,
                            PrimaryColor = entity.BulletVfx.PrimaryColor,
                            SecondaryColor = entity.BulletVfx.SecondaryColor,
                        });
                }

                dataBuffer?.Release();
                argsBuffer?.Release();
                dataBuffer = new ComputeBuffer(bullets.Count, sizeof(InstanceData));
                dataBuffer.SetData(bullets);

                args[0] = (uint)mesh.GetIndexCount(0);
                args[1] = (uint)bullets.Count;
                args[2] = (uint) mesh.GetIndexStart(0);
                args[3] = (uint) mesh.GetBaseVertex(0);
                
                argsBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
                argsBuffer.SetData(args);
                Utils.ListPool<InstanceData>.Release(bullets);
            }
            

            public override void FrameCleanup(CommandBuffer cmd)
            {
            }
        }
    }
}