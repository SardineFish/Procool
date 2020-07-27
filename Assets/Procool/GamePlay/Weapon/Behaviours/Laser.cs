namespace Procool.GamePlay.Weapon
{
    public class Laser : WeaponBehaviour<Laser.Data>
    {
        public class Data : WeaponBehaviourData
        {
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
            }
        }
    }
}