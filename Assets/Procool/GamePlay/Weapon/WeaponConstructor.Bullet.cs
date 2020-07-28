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
                    return .3f;
                case 2:
                    return .2f;
                case 1:
                    return .1f;
                default:
                    return .1f;
            }
        }

        void GenerateBulletVFX(PRNG prng, DamageStage stage, int level)
        {
            if(!stage)
                return;
            foreach (var behaviour in stage.Behaviours)
            {
                if (behaviour.Behaviour is Move || behaviour.Behaviour is Throw)
                {
                    stage.BulletVFX.Sprite = WeaponSystem.Instance.midBullets.RandomTake(prng.GetScalar(), _ => 1);
                    stage.BulletVFX.SpriteColor = Color.HSVToRGB(prng.GetScalar(), .8f, .8f);
                }
                if(behaviour.Behaviour is Move move && (behaviour as Move.Data).Speed > 6)
                {
                    stage.BulletVFX.EnableTrail = true;
                    stage.BulletVFX.TrailStartWidth = BulletSize(level) * .8f;
                    stage.BulletVFX.TrailEndWidth = 0;
                }

                if (behaviour.Behaviour is Throw)
                {
                    stage.BulletVFX.Sprite = WeaponSystem.Instance.bombSprites.RandomTake(prng.GetScalar(), _ => 1);
                    stage.BulletVFX.SpriteColor = Color.white;
                }

                if (behaviour.Behaviour is Timeout)
                {
                    stage.BulletVFX.Flicking = true;
                    stage.BulletVFX.FlickingColor = Color.red;
                    stage.BulletVFX.Sprite = WeaponSystem.Instance.bombSprites.RandomTake(prng.GetScalar(), _ => 1);
                }

                if (behaviour is EmitContinuous || behaviour is EmitOnce || behaviour is EmitTick)
                    GenerateBulletVFX(prng, behaviour.NextStage, level - 1);
                else
                    GenerateBulletVFX(prng, behaviour.NextStage, level);
            }
        }
        
        public void GenerateBulletVFX(Weapon weapon, PRNG prng)
        {
            var maxEmitDepth = SearchEmitDepth(weapon.FirstStage);

            GenerateBulletVFX(prng, weapon.FirstStage, Mathf.Max(maxEmitDepth, 3));
        }
    }
}