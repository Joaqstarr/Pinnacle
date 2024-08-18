using UnityEngine;

namespace Player.States
{
    public abstract class BaseState
    {
        protected readonly PlayerBrain Player;
        protected readonly PlayerControls Controls;
        protected readonly PlayerData Data;
        protected readonly Rigidbody Rb;

        protected BaseState(PlayerBrain player, PlayerControls controls, PlayerData data, Rigidbody rb)
        {
            Player = player;
            Controls = controls;
            Data = data;
            Rb = rb;
        }
        public abstract void OnEnterState();

        public abstract void OnExitState();

        public abstract void OnUpdateState();
        
        public abstract void OnFixedUpdateState();

        public abstract void OnEnterBuildMode();

        public abstract void OnExitBuildMode();

    }
}