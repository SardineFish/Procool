using System.Collections;
using Cinemachine;
using Procool.GamePlay.Inventory;
using Procool.GameSystems;
using Procool.Input;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    public partial  class PlayerController
    {
        class PlayerMove : PlayerAction
        {
            private IUsingState UsingItem = null;
            
            public override bool CanEnter(PlayerController player)
            {
                return CameraManager.Instance.State == CameraManager.CameraState.Player;
            }

            public override void Enter(PlayerController player)
            {
                player.StartCoroutine(UseItemCoroutine(player));
            }


            IEnumerator UseItemCoroutine(PlayerController player)
            {
                while (true)
                {
                    if (player.Input.GamePlay.Fire.ReadValue<float>() > 0.5f)
                    {
                        var item = player.Player.Inventory.GetItem(0);
                        var usingState = item.Activate();
                        while (player.Input.GamePlay.Fire.ReadValue<float>() > 0.5f && usingState.Tick())
                            yield return null;
                    }

                    yield return null;
                }
            }

            IEnumerator UserItem(PlayerController player)
            {
                var item = player.Player.Inventory.GetItem(0);
                UsingItem = item.Activate();
                yield return UsingItem.Wait();
                UsingItem = null;
            }

            public override bool Update(PlayerController player)
            {
                if (CameraManager.Instance.State != CameraManager.CameraState.Player)
                    return false;
                var zoom = player.Input.GamePlay.Zoom.ReadValue<float>();
                if (CameraManager.Instance.State == CameraManager.CameraState.Player && Mathf.Abs(zoom) >= 0.01f)
                {
                    switch (InputDeviceDetector.CurrentInputScheme)
                    {
                        case InputSchemeType.Keyboard:
                            CameraManager.Instance.Zoom(-zoom * player.keyboardZoomMultiplier);
                            break;
                        case InputSchemeType.GamePad:
                            CameraManager.Instance.Zoom(-zoom * player.gamePadZoomMultiplier * Time.deltaTime);
                            break;
                            
                    }
                }

                // if (player.Input.GamePlay.Fire.ReadValue<float>() > 0.5f && (UsingItem is null))
                //     player.StartCoroutine(UserItem(player));
                // else if (player.Input.GamePlay.Fire.ReadValue<float>() <= 0)
                //     UsingItem?.Terminate();

                
                return true;
            }

            public override void FixedUpdate(PlayerController player)
            {
                var inputMovement = player.Input.GamePlay.Move.ReadValue<Vector2>();
                inputMovement = Vector2.ClampMagnitude(inputMovement, 1);
                Vector2 inputDirection = Vector2.zero;
                if (InputDeviceDetector.CurrentInputScheme == InputSchemeType.GamePad)
                {
                    inputDirection = player.Input.GamePlay.Direction.ReadValue<Vector2>();
                    if (inputDirection.magnitude < 0.01f)
                        inputDirection = inputMovement.normalized;
                }
                else if (InputDeviceDetector.CurrentInputScheme == InputSchemeType.Keyboard)
                {
                    var screenPos = player.Input.GamePlay.Pointer.ReadValue<Vector2>();
                    var ray = CameraManager.Camera.ScreenPointToRay(screenPos);
                    var pos = ray.origin + ray.direction * Mathf.Abs(ray.origin.z);
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
                //player.rigidbody.velocity = player.velocity;
                player.rigidbody.MovePosition(player.transform.position.ToVector2() + player.velocity * Time.fixedDeltaTime);
            }
        }     
    }
}