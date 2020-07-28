using System.Collections.Generic;
using Procool.GameSystems;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public class WeaponSystem : Singleton<WeaponSystem>
    {
        [SerializeField] private List<GameObject> bulletPrefabs;
        [SerializeField] private List<GameObject> laserPrefabs;
        [SerializeField] private List<GameObject> bombPrefabs;
        [SerializeField] private List<GameObject> timerPrefabs;

        private WeaponConstructor WeaponConstructor = new WeaponConstructor();

        protected override void Awake()
        {
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
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>()
                .CompatibleWith<Destructor>()
                .CompatibleWith<EmitContinuous>()
                .CompatibleWith<EmitTick>()
                .CompatibleWith<Timeout>();
            
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
            return WeaponConstructor.BuildWeapon(prng);
        }
    }
}