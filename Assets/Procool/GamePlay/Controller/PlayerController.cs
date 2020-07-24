using System;
using UnityEngine;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        public float MaxSpeed = 10;
        [Range(0, 1)]
        public float MoveDamping = .3f;
        
        private GameInput Input;

        private new Rigidbody2D rigidbody;

        private void Awake()
        {
            Input = new GameInput();
            rigidbody = GetComponent<Rigidbody2D>();
            
        }

        private void Update()
        {
            var inputMovement = Input.GamePlay.Move.ReadValue<Vector2>();
            var targetVelocity = MaxSpeed * inputMovement.normalized;
            var velocity = Vector2.Lerp(rigidbody.velocity, targetVelocity, (1 - MoveDamping));
            rigidbody.velocity = velocity;
        }
    }
}