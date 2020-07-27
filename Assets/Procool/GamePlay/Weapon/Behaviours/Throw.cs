namespace Procool.GamePlay.Weapon
{
    public class Throw : WeaponBehaviour <Throw.Data>
    {
        public class Data : WeaponBehaviourData
        {
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
            }
        }
    }
}