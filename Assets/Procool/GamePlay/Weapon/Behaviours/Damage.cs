namespace Procool.GamePlay.Weapon
{
    public class Damage : WeaponBehaviour<EmptyBehaviourData>
    {
        protected override float EvaluateAdditionalDamage(EmptyBehaviourData data) => 1;
    }
}