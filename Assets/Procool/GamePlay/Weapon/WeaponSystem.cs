using System.Collections.Generic;
using Procool.GamePlay.Controller;
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
                .CompatibleWith<Damage>(100000)
                .CompatibleWith<CollisionTrigger>(2)
                .CompatibleWith<Destructor>()
                .CompatibleWith<EmitTick>()
                .CompatibleWith<TraceTarget>()
                .CompatibleWith<Timeout>();

            WeaponConstructor.Behaviour<Throw>()
                .Primary()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<CollisionTrigger>()
                .CompatibleWith<EmitTick>()
                .NextStage<Explode>(2)
                .NextStage<Sticky>();
            
            WeaponConstructor.Behaviour<Bounce>()
                .Collider()
                .CompatibleWith<Damage>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<EmitTick>()
                .CompatibleWith<TraceTarget>()
                .CompatibleWith<Timeout>();

            WeaponConstructor.Behaviour<TraceTarget>()
                .CompatibleWith<Bounce>()
                .CompatibleWith<CollisionTrigger>()
                .CompatibleWith<Damage>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<EmitTick>()
                .CompatibleWith<Timeout>();

            WeaponConstructor.Behaviour<CollisionTrigger>()
                .Collider()
                .CompatibleWith<Damage>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<EmitTick>()
                .CompatibleWith<TraceTarget>()
                .CompatibleWith<Timeout>()
                .NextStage<Explode>()
                .NextStage<Sticky>()
                .NoNext(2);

            WeaponConstructor.Behaviour<Sticky>()
                .CompatibleWith<Timeout>()
                .CompatibleWith<EmitTick>(100)
                .CompatibleWith<EmitOnce>()
                .CompatibleWith<EmitScatter>(0.2f);

            WeaponConstructor.Behaviour<Explode>()
                .Terminator();

            WeaponConstructor.Behaviour<Damage>()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<CollisionTrigger>()
                .CompatibleWith<TraceTarget>()
                // .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>();


            WeaponConstructor.Behaviour<Destructor>()
                .DefaultTerminator()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                .CompatibleWith<CollisionTrigger>()
                .CompatibleWith<TraceTarget>()
                // .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>();

            WeaponConstructor.Behaviour<Timeout>()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                // .NextStage<EmitContinuous>()
                .CompatibleWith<CollisionTrigger>()
                .CompatibleWith<TraceTarget>()
                .NextStage<EmitOnce>()
                .NextStage<EmitTick>()
                .NextStage<EmitScatter>(0.5f)
                .NextStage<Explode>(2);

            WeaponConstructor.Behaviour<EmitOnce>()
                .DetachEmitter()
                .Terminator()
                .NextStage<Move>(1)
                .NextStage<Throw>();

            WeaponConstructor.Behaviour<EmitTick>(2)
                .DetachEmitter()
                .CompatibleWith<Destructor>()
                .CompatibleWith<CollisionTrigger>()
                .CompatibleWith<Damage>()
                .CompatibleWith<Bounce>()
                .CompatibleWith<TraceTarget>()
                .NextStage<Move>(3)
                .NextStage<Throw>(1)
                .NextStage<EmitOnce>()
                .NextStage<EmitScatter>(0.2f);

            WeaponConstructor.Behaviour<EmitScatter>(.6f)
                .Primary()
                .DetachEmitter()
                .Terminator()
                .NextStage<Move>()
                .NextStage<Throw>();

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
            prng.GetScalar();
            var weapon = WeaponConstructor.BuildWeapon(prng);
            weapon.Sprite = weaponSprites.RandomTake(prng.GetScalar(), _ => 1);
            return weapon;
        }

        public void GenerateExplosion(Vector2 position, float radius, float damage, Player owner = null)
        {
            Explosion explosion;
            if (radius > 7)
            {
                explosion = GameObjectPool.Get<Explosion>(PrefabManager.Instance.LargeExplosionPrefab);
                explosion.Prefab = PrefabManager.Instance.LargeExplosionPrefab;
            }
            else
            {
                explosion = GameObjectPool.Get<Explosion>(PrefabManager.Instance.SmallExplosionPrefab);
                explosion.Prefab = PrefabManager.Instance.SmallExplosionPrefab;
            }
            
            explosion.Explode(position, radius, damage, owner);
        }
    }
}