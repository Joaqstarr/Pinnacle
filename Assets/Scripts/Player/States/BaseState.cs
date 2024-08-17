namespace Player.States
{
    public abstract class BaseState
    {
        protected readonly PlayerBrain Player;
        protected readonly PlayerControls Controls;
        protected readonly PlayerData Data;

        protected BaseState(PlayerBrain player, PlayerControls controls, PlayerData data)
        {
            Player = player;
            Controls = controls;
            Data = data;
        }
        public abstract void OnEnterState();

        public abstract void OnExitState();

        public abstract void OnUpdateState();
    }
}