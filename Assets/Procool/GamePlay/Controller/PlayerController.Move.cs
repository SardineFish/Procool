﻿using Procool.GameSystems;
using Procool.Input;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    public partial  class PlayerController
    {
        class PlayerMove : PlayerAction
        {
            public override void FixedUpdate(PlayerController player)
            {
                var inputMovement = player.Input.GamePlay.Move.ReadValue<Vector2>();
                Debug.Log(inputMovement);
                inputMovement = Vector2.ClampMagnitude(inputMovement, 1);
                Vector2 inputDirection = Vector2.zero;
                if (player.CurrentInputScheme == InputSchemeType.GamePad)
                {
                    inputDirection = player.Input.GamePlay.Direction.ReadValue<Vector2>();
                    if (inputDirection.magnitude < 0.01f)
                        inputDirection = inputMovement.normalized;
                }
                else if (player.CurrentInputScheme == InputSchemeType.Keyboard)
                {
                    var screenPos = player.Input.GamePlay.Pointer.ReadValue<Vector2>();
                    var pos = CameraManager.Camera.ScreenToWorldPoint(screenPos);
                    var dir = (pos - player.transform.position).ToVector2();
                    inputDirection = dir.normalized;
                }
                var dAng = Vector2.SignedAngle(player.transform.up, inputDirection);
                var angularV = dAng / Time.fixedDeltaTime;
                var maxAngularV = Mathf.Lerp(player.minAngularVelocity, player.maxAngularVelocity, Mathf.Abs(dAng) / 180);

                if (Mathf.Abs(angularV) > maxAngularV)
                {
                    angularV = MathUtility.SignInt(dAng) * maxAngularV;
                }

                player.transform.Rotate(Vector3.forward, angularV * Time.fixedDeltaTime);

                var targetVelocity = player.maxSpeed * inputMovement;
                player.velocity = Vector2.Lerp(player.velocity, targetVelocity, (1 - player.moveDamping));
                player.rigidbody.velocity = player.velocity;
            }
        }     
    }
}