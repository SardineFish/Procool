namespace Procool.GamePlay.Controller
{
    public partial class PlayerController
    {
        private abstract class PlayerAction
        {
            protected PlayerAction(Player player, PlayerController controller)
            {
                Player = player;
                Controller = controller;
            }

            public Player Player { get; }
            public PlayerController Controller { get; }

            public virtual void Enter()
            {
            }

            public virtual void Update()
            {
                
            }

            public virtual void FixedUpdate()
            {
            }

            public virtual void Exit()
            {
            }

        }
    }
}