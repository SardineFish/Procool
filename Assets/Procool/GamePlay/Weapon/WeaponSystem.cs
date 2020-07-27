using System.Collections.Generic;
using Procool.GameSystems;
using Procool.Random;
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
            WeaponConstructor.Behaviour<Bounce>()
                .CanBeFirstStage()
                .CompatibleWith<Throw>()
                .CompatibleWith<Move>()
                .CompatibleWith<Laser>()
                .CompatibleWith<Damage>();

            WeaponConstructor.Behaviour<Damage>()
                .CanBeFirstStage()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Throw>()
                .CompatibleWith<Move>()
                .CompatibleWith<Laser>()
                .CompatibleWith<Damage>();

            WeaponConstructor.Behaviour<Laser>()
                .CanBeFirstStage()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>(10);

            WeaponConstructor.Behaviour<Move>()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>();

            WeaponConstructor.Behaviour<Throw>()
                .CompatibleWith<Bounce>()
                .CompatibleWith<Damage>();

        }

        public Weapon GenerateWeapon(PRNG prng)
        {
            return WeaponConstructor.BuildWeapon(prng);
        }
    }
}