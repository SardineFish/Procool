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
                .Collider()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>(10)
                .CompatibleWith<Destructor>();

            WeaponConstructor.Behaviour<Move>()
                .Primary()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                .CompatibleWith<CollisionTrigger>()
                .CompatibleWith<Destructor>()
                // .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>()
                .CompatibleWith<Timeout>();

            WeaponConstructor.Behaviour<Throw>()
                .Primary()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<CollisionTrigger>()
                // .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>()
                .NextStage<EmitOnce>()
                .NextStage<Timeout>()
                .NextStage<Destructor>();
            
            WeaponConstructor.Behaviour<Bounce>()
                .Collider()
                .CompatibleWith<Damage>()
                .CompatibleWith<Destructor>()
                // .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>()
                .CompatibleWith<Timeout>();

            WeaponConstructor.Behaviour<CollisionTrigger>()
                .Collider()
                .CompatibleWith<Damage>()
                .CompatibleWith<Destructor>()
                // .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>()
                .CompatibleWith<Timeout>();

            WeaponConstructor.Behaviour<Damage>()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<CollisionTrigger>()
                // .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>();


            WeaponConstructor.Behaviour<Destructor>()
                .DefaultTerminator()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                .CompatibleWith<CollisionTrigger>()
                // .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>();

            WeaponConstructor.Behaviour<Timeout>()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                // .NextStage<EmitContinuous>()
                .CompatibleWith<CollisionTrigger>()
                .NextStage<EmitOnce>()
                .NextStage<EmitTick>();

            WeaponConstructor.Behaviour<EmitOnce>()
                .DetachEmitter()
                .Terminator()
                .NextStage<Move>(1)
                .NextStage<Throw>(1);

            WeaponConstructor.Behaviour<EmitTick>(8)
                .DetachEmitter()
                .CompatibleWith<Destructor>()
                .CompatibleWith<CollisionTrigger>()
                .CompatibleWith<Damage>()
                .CompatibleWith<Bounce>()
                .NextStage<Move>(3)
                .NextStage<Throw>(1);

            // WeaponConstructor.Behaviour<EmitContinuous>()
            //     .Emitter()
            //     .CompatibleWith<Destructor>()
            //     .CompatibleWith<CollisionTrigger>()
            //     .CompatibleWith<Damage>()
            //     .CompatibleWith<Bounce>()
            //     .NextStage<Laser>();
                

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