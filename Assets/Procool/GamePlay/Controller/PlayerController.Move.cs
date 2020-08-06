using System.Collections;
using System.Linq;
using System.Net;
using Cinemachine;
using Procool.GamePlay.Inventory;
using Procool.GameSystems;
using Procool.Input;
using Procool.Utils;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Procool.GamePlay.Controller
{
    public partial  class PlayerController
    {
        class PlayerMove : PlayerAction
        {
            private IUsingState UsingItem = null;
            private Coroutine _actionCoroutine;

            public PlayerMove(Player player, PlayerController controller) : base(player, controller)
            {
            }

            public override void Enter()
            {
                _actionCoroutine = Controller.StartCoroutine(ActionCoroutine());
                Controller.Renderer.enabled = true;
                CameraManager.Instance.UsePlayerCamera();
            }

            public override void Exit()
            {
                Controller.StopCoroutine(_actionCoroutine);
            }


            IEnumerator ActionCoroutine()
            {
                while (true)
                {
                    if (Controller.Input.GamePlay.Fire.IsPressed())
                    {
                        var item = Controller.Player.Inventory.GetItem(0);
                        var usingState = item.Activate();
                        while (Controller.Input.GamePlay.Fire.IsPressed() && usingState.Tick())
                            yield return null;
                    }
                    else if (Controller.Input.GamePlay.Interact.IsPressed())
                    {
                        var interactObject = Player.AvailableInteractiveObjects
                            .OrderBy(obj => obj.Distance(Player))
                            .FirstOrDefault();
                        if(!interactObject)
                            continue;
                        
                        foreach (var t in Utility.TimerNormalized(base.Controller.keyHoldTime))
                        {
                            if (!Controller.Input.GamePlay.Interact.IsPressed() || !interactObject.StillInRange(Player))
                            {
                                interactObject.InteractKeyHold(0);
                                goto End;
                            }
                            
                            interactObject.InteractKeyHold(t);

                            yield return null;
                        }
                        
                        interactObject.Interact(Player);
                    }

                    End:
                    yield return null;
                }
            }

            public override void Update()
            {
                if (CameraManager.Instance.State != CameraManager.CameraState.Player)
                    return;
                var zoom = Controller.Input.GamePlay.Zoom.ReadValue<float>();
                if (CameraManager.Instance.State == CameraManager.CameraState.Player && Mathf.Abs(zoom) >= 0.01f)
                {
                    switch (InputManager.CurrentInputScheme)
                    {
                        case InputSchemeType.Keyboard:
                            CameraManager.Instance.Zoom(-zoom * Controller.keyboardZoomMultiplier);
                            break;
                        case InputSchemeType.GamePad:
                            CameraManager.Instance.Zoom(-zoom * Controller.gamePadZoomMultiplier * Time.deltaTime);
                            break;
                            
                    }
                }
            }

            public override void FixedUpdate()
            {
                var inputMovement = Controller.Input.GamePlay.Move.ReadValue<Vector2>();
                inputMovement = Vector2.ClampMagnitude(inputMovement, 1);
                Vector2 inputDirection = Vector2.zero;
                if (InputManager.CurrentInputScheme == InputSchemeType.GamePad)
                {
                    inputDirection = Controller.Input.GamePlay.Direction.ReadValue<Vector2>();
                    if (inputDirection.magnitude < 0.01f)
                        inputDirection = inputMovement.normalized;
                }
                else if (InputManager.CurrentInputScheme == InputSchemeType.Keyboard)
                {
                    var screenPos = Controller.Input.GamePlay.Pointer.ReadValue<Vector2>();
                    var ray = CameraManager.Camera.ScreenPointToRay(screenPos);
                    var pos = ray.origin + ray.direction * Mathf.Abs(ray.origin.z);
                    var dir = (pos - Controller.transform.position).ToVector2();
                    inputDirection = dir.normalized;
                }
                var dAng = Vector2.SignedAngle(Controller.transform.up, inputDirection);
                var angularV = dAng / Time.fixedDeltaTime;
                var maxAngularV = Mathf.Lerp(Controller.minAngularVelocity, Controller.maxAngularVelocity, Mathf.Abs(dAng) / 180);

                if (Mathf.Abs(angularV) > maxAngularV)
                {
                    angularV = MathUtility.SignInt(dAng) * maxAngularV;
                }

                Controller.transform.Rotate(Vector3.forward, angularV * Time.fixedDeltaTime);

                var targetVelocity = Controller.maxSpeed * inputMovement;
                Controller.velocity = Vector2.Lerp(Controller.velocity, targetVelocity, (1 - Controller.moveDamping));
                //player.rigidbody.velocity = player.velocity;
                Controller.rigidbody.MovePosition(Controller.transform.position.ToVector2() + Controller.velocity * Time.fixedDeltaTime);
            }
        }     
    }
}