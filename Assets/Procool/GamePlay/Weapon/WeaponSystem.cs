using System.Collections.Generic;
using Procool.GameSystems;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class WeaponSystem : Singleton<WeaponSystem>
    {
        [SerializeField] public List<Sprite> largeBullets;
        [SerializeField] public List<Sprite> midBullets;
        [SerializeField] public List<Sprite> smallBullets;
        [SerializeField] public List<Sprite> weaponSprites;
        [SerializeField] public List<Sprite> bombSprites;

        private WeaponConstructor WeaponConstructor = new WeaponConstructor();

        protected override void Awake()
        {
            base.Awake();
            WeaponConstructor.Behaviour<Laser>()
                .Primary()
                .Terminator()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>(10)
                .CompatibleWith<Destructor>();

            WeaponConstructor.Behaviour<Move>()
                .Primary()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>()
                .CompatibleWith<Timeout>();

            WeaponConstructor.Behaviour<Throw>()
                .Primary()
                .Terminator()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>()
                .NextStage<Timeout>();
            
            WeaponConstructor.Behaviour<Bounce>()
                .CompatibleWith<Damage>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>()
                .CompatibleWith<Timeout>();

            WeaponConstructor.Behaviour<Damage>()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>();


            WeaponConstructor.Behaviour<Destructor>()
                .DefaultTerminator()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>();

            WeaponConstructor.Behaviour<Timeout>()
                .Terminator()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                .NextStage<EmitContinuous>()
                .NextStage<EmitOnce>()
                .NextStage<EmitTick>();

            WeaponConstructor.Behaviour<EmitOnce>()
                .Emitter()
                .Terminator()
                .NextStage<Move>()
                .NextStage<Throw>();

            WeaponConstructor.Behaviour<EmitTick>()
                .Emitter()
                .CompatibleWith<Destructor>()
                .NextStage<Move>()
                .NextStage<Throw>();

            WeaponConstructor.Behaviour<EmitContinuous>()
                .Emitter()
                .CompatibleWith<Destructor>()
                .NextStage<Laser>();

                

            GameObjectPool.PreAlloc<DamageEntity>(16);

        }

        public Weapon GenerateWeapon(PRNG prng)
        {
            var weapon = WeaponConstructor.BuildWeapon(prng);
            weapon.Sprite = weaponSprites.RandomTake(prng.GetScalar(), _ => 1);
            return weapon;
        }
    }
}