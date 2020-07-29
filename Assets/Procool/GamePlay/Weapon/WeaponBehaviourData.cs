using System;
using System.Text;

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

        public override string ToString()
        {
            if (!NextStage)
                return Behaviour.GetType().Name;

            var selfName = Behaviour.GetType().Name;
            var lines = NextStage.ToString().Split(new string[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            for (var i = 0; i < lines.Length; i++)
            {
                if (i == 0)
                {
                    sb.Append(selfName);
                    sb.Append(" - ");
                    sb.AppendLine(lines[i]);
                }
                else
                {
                    for (var j = 0; j < selfName.Length + 3; j++)
                        sb.Append(" ");
                    sb.AppendLine(lines[i]);
                }
            }

            return sb.ToString();
        }
    }

    public class EmitterBehaviourData : WeaponBehaviourData
    {
        public BulletVFX BulletVFX;
        public EmitterBehaviourData(IWeaponBehaviour behaviour) : base(behaviour)
        {
        }
    }

    public class EmptyBehaviourData : WeaponBehaviourData
    {
        public EmptyBehaviourData(IWeaponBehaviour behaviour) : base(behaviour)
        {
        }
    }
}