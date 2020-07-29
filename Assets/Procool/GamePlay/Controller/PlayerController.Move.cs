using System.Collections;
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
            private Coroutine coroutuine;
            public override void Enter(PlayerController player)
            {
                coroutuine = player.StartCoroutine(UpdateCoroutine(player));
            }

            IEnumerator UpdateCoroutine(PlayerController player)
            {
                while (true)
                {
                    if (player.Input.GamePlay.Fire.ReadValue<float>() > .5f)
                    {
                        var item = player.Player.Inventory.GetItem(0);
                        if (item != null)
                        {
                            yield return item.Activate().Wait();
                        }

                        while ((player.Input.GamePlay.Fire.ReadValue<float>() > .5f))
                            yield return null;
                    }
                    
                    yield return null;
                }
            }

            public override bool Update(PlayerController player) => true;

            public override void Exit(PlayerController player)
            {
                
            }

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