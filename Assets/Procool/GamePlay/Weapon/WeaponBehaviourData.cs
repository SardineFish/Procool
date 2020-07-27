namespace Procool.GamePlay.Weapon
{
    public class WeaponBehaviourData
    {
        public DamageStage NextStage;
        public IWeaponBehaviour Behaviour;

        public WeaponBehaviourData(IWeaponBehaviour behaviour)
        {
            Behaviour = behaviour;
        }
        
    }

    public class EmptyBehaviourData : WeaponBehaviourData
    {
        public EmptyBehaviourData(IWeaponBehaviour behaviour) : base(behaviour)
        {
        }
    }
}