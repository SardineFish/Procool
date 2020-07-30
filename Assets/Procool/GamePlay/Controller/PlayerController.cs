﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Procool.Input;
using UnityEngine.InputSystem;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(Player), typeof(Rigidbody2D))]
    public partial class PlayerController : MonoBehaviour
    {
        public float maxSpeed = 3;
        public float maxAngularVelocity = 600f;
        public float minAngularVelocity = 300f;
        [Range(0, 1)] public float moveDamping = .3f;

        public float gamePadZoomMultiplier = 1;
        public float keyboardZoomMultiplier = 10;
        public AnimationCurve zoomSpeedCurve = new AnimationCurve(new Keyframe(0, 10), new Keyframe(700, 50));
        

        private GameInput Input;

        private new Rigidbody2D rigidbody;
        private Vector2 velocity;

        private List<PlayerAction> _playerActions = new List<PlayerAction>();
        private PlayerAction _currentAction;

        public Player Player { get; private set; }

        private void Awake()
        {
            Input = new GameInput();
            rigidbody = GetComponent<Rigidbody2D>();

            
            _playerActions.Add(new PlayerMove());
            Player = GetComponent<Player>();
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
            bool actionKeepRunning = false;
            foreach (var action in _playerActions)
            {
                if (actionKeepRunning)
                {
                    action.Bypass(this);
                    continue;
                }

                if (_currentAction is null || _currentAction != action)
                {
                    if (action.CanEnter(this))
                    {
                        _currentAction?.Exit(this);
                        _currentAction = action;
                        _currentAction.Enter(this);
                    }
                    else
                        action.Bypass(this);
                }

                if (_currentAction == action)
                {
                    var keep = _currentAction.Update(this);
                    if (keep)
                        actionKeepRunning = true;
                    else
                    {
                        _currentAction.Exit(this);
                        _currentAction = null;
                    }
                }
                
            }
        }

        private void FixedUpdate()
        {
            _currentAction?.FixedUpdate(this);
        }
    }

    
}