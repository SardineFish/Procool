namespace Procool.GamePlay.Weapon
{
    public class Move : WeaponBehaviour<Move.Data>
    {
        public class Data : WeaponBehaviourData
        {
            public Data(IWeaponBehaviour behaviour) : base(behaviour)
            {
            }
        }
    }
}