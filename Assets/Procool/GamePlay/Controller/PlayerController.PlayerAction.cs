namespace Procool.GamePlay.Controller
{
    public partial class PlayerController
    {
        private abstract class PlayerAction
        {
            public virtual bool CanEnter(PlayerController player) => true;

            public virtual void Enter(PlayerController player)
            {
            }

            public virtual bool Update(PlayerController player) => false;

            public virtual void FixedUpdate(PlayerController player)
            {
            }

            public virtual void Exit(PlayerController player)
            {
            }

            public virtual void Bypass(PlayerController player)
            {
            }
        }
    }
}