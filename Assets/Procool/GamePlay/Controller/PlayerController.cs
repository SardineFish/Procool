using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Procool.Input;
using Procool.Utils;
using UnityEngine.InputSystem;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(Player), typeof(Rigidbody2D))]
    public partial class PlayerController : MonoBehaviour, ICustomEditorEX
    {
        public float maxSpeed = 3;
        public float maxAngularVelocity = 600f;
        public float minAngularVelocity = 300f;
        [Range(0, 1)] public float moveDamping = .3f;

        public float gamePadZoomMultiplier = 1;
        public float keyboardZoomMultiplier = 10;
        public AnimationCurve zoomSpeedCurve = new AnimationCurve(new Keyframe(0, 10), new Keyframe(700, 50));

        public float keyHoldTime = .4f;


        public Renderer Renderer;
        

        private GameInput Input;

        private new Rigidbody2D rigidbody;
        private Vector2 velocity;

        private PlayerAction _currentAction;
        private PlayerMove _playerMove;
        private PlayerDrive _playerDrive;

        public Player Player { get; private set; }

        private void Awake()
        {
            Input = new GameInput();
            rigidbody = GetComponent<Rigidbody2D>();
            Player = GetComponent<Player>();

            _playerMove = new PlayerMove(Player, this);
            _playerDrive = new PlayerDrive(Player, this);
            ChangeAction(_playerMove);
        }

        private void OnEnable()
        {
            Input.Enable();
        }

        private void OnDisable()
        {
            Input.Disable();
        }

        private void Update()
        {
            _currentAction?.Update();
        }

        private void FixedUpdate()
        {
            _currentAction?.FixedUpdate();
        }

        void ChangeAction(PlayerAction action)
        {
            if(_currentAction == action)
                return;
            _currentAction?.Exit();
            action?.Enter();
            _currentAction = action;
        }

        public void GetOnVehicle(Vehicle vehicle)
        {
            _playerDrive.Vehicle = vehicle;
            ChangeAction(_playerDrive);
        }

        public void GetOffVehicle(Vehicle vehicle)
        {
            ChangeAction(_playerMove);
        }

        public void LockPlayer()
        {
            ChangeAction(null);
        }

        [EditorButton]
        public void UnlockPlayer()
        {
            ChangeAction(_playerMove);
        }
    }

    
}