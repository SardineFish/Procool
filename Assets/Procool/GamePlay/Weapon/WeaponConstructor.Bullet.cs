using System.Linq;
using Procool.Random;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public partial class WeaponConstructor
    {
        int SearchEmitDepth(DamageStage stage)
        {
            if (!stage)
                return 0;
            var maxDepth = 0;
            foreach (var behaviour in stage.Behaviours)
            {
                switch (behaviour.Behaviour)
                {
                    case EmitOnce emitOnce:
                    case EmitContinuous emitContinuous:
                    case EmitTick emitTick:
                    case EmitScatter emitScatter:
                        maxDepth = Mathf.Max(1 + SearchEmitDepth(behaviour.NextStage));
                        break;
                }
            }

            return maxDepth;
        }

        float BulletSize(int level)
        {
            switch (level)
            {
                case 3:
                    return .4f;
                case 2:
                    return .3f;
                case 1:
                    return .2f;
                default:
                    return .15f;
            }
        }

        // Sprite GetBulletByLevel(PRNG prng, int level)
        // {
        //     switch (level)
        //     {
        //         case 3:
        //             return WeaponSystem.Instance.largeBullets.RandomTake(prng.GetScalar());
        //         case 2:
        //             return WeaponSystem.Instance.midBullets.RandomTake(prng.GetScalar());
        //         default:
        //             return WeaponSystem.Instance.smallBullets.RandomTake(prng.GetScalar());
        //     }
        // }

        BulletVFX GenerateBulletVFXByNextStage(PRNG prng, DamageStage stage, int level)
        {
            BulletVFX vfx = new BulletVFX();
            if (!stage)
                return vfx;
            foreach (var behaviour in stage.Behaviours)
            {
                if (behaviour.Behaviour is Move || behaviour.Behaviour is Throw)
                {
                    // vfx.Sprite = GetBulletByLevel(prng, level);
                    vfx.SpriteColor = Color.HSVToRGB(prng.GetScalar(), .8f, .8f);
                    vfx.BulletSize = BulletSize(level);
                    vfx.PrimaryColor = Color.HSVToRGB(prng.GetScalar(), .8f, .8f);
                    vfx.SecondaryColor = Color.white;
                }

                if (behaviour.Behaviour is Move move && (behaviour as Move.Data).Speed > 6)
                {
                    vfx.EnableTrail = true;
                    vfx.TrailStartWidth = BulletSize(level) * .8f;
                    vfx.TrailEndWidth = 0;
                }


                // if (behaviour.Behaviour is Throw)
                // {
                //     vfx.Sprite = WeaponSystem.Instance.bombSprites.RandomTake(prng.GetScalar(), _ => 1);
                //     vfx.SpriteColor = Color.white;
                // }

                // if (behaviour.Behaviour is Timeout)
                // {
                //     vfx.Flicking = true;
                //     vfx.FlickingColor = Color.red;
                //     vfx.Sprite = WeaponSystem.Instance.bombSprites.RandomTake(prng.GetScalar(), _ => 1);
                // }

                if (behaviour is EmitContinuous || behaviour is EmitOnce || behaviour is EmitTick)
                    GenerateBulletVFX(prng, behaviour.NextStage, level - 1);
                else
                    GenerateBulletVFX(prng, behaviour.NextStage, level);
            }

            return vfx;
        }

        void GenerateBulletVFX(PRNG prng, DamageStage stage, int level)
        {
            if (!stage)
                return;
            foreach (var data in stage.Behaviours)
            {
                switch (data.Behaviour)
                {
                    case EmitOnce emitOnce:
                    case EmitTick emitTick:
                    case EmitScatter emitScatter:
                        var emitData = data as EmitterBehaviourData;
                        emitData.BulletVFX = GenerateBulletVFXByNextStage(prng, data.NextStage, level);
                        GenerateBulletVFX(prng, data.NextStage, level - 1);
                        break;
                    default:
                        GenerateBulletVFX(prng, data.NextStage, level);
                        break;
                }
            }
        }

        public void GenerateBulletVFX(Weapon weapon, PRNG prng)
        {
            var maxEmitDepth = SearchEmitDepth(weapon.FirstStage);

            GenerateBulletVFX(prng, weapon.FirstStage, Mathf.Min(maxEmitDepth, 3));
        }
    }
}