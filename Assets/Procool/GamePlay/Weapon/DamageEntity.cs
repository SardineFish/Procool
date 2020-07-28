using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.GamePlay.Controller;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    [RequireComponent(typeof(SpriteRenderer), typeof(CircleCollider2D), typeof(BoxCollider2D))]
    public class DamageEntity : MonoBehaviour
    {
        public Player Owner { get; private set; }
        public List<ContactPoint2D> Contacts { get; } = new List<ContactPoint2D>(16);
        public HashSet<Player> ContactedPlayers = new HashSet<Player>();
        public HashSet<Player> DamageRecord { get; } = new HashSet<Player>();
        public Weapon Weapon { get; private set; }
        public DamageStage Stage { get; private set; }
        public SpriteRenderer SpriteRenderer { get; private set; }
        public BoxCollider2D BoxCollider { get; private set; }
        public CircleCollider2D CircleCollider { get; private set; }

        public event Action<DamageEntity> OnTerminated;

        private void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            BoxCollider = GetComponent<BoxCollider2D>();
            CircleCollider = GetComponent<CircleCollider2D>();
            BoxCollider.enabled = false;
            CircleCollider.enabled = false;
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            HandleCollision(other);
        }

        private void OnCollisionStay2D(Collision2D other)
        {

            HandleCollision(other);
        }

        void HandleCollision(Collision2D other)
        {
            other.GetContacts(Contacts);
            var player = other.rigidbody.GetComponent<Player>();
            if (player)
                ContactedPlayers.Add(player);
        }

        private void FixedUpdate()
        {
            Contacts.Clear();
            ContactedPlayers.Clear();
        }

        public IEnumerator Run()
        {
            var runner = CoroutineRunner.All(Stage.Behaviours.Select(behaviour =>
                behaviour.Behaviour.Run(this, behaviour, Stage, Weapon)));
            var coroutine = StartCoroutine(runner);
            yield return coroutine;
            
            Terminate();
        }

        public void Init(Player player, Weapon weapon, DamageStage stage)
        {
            Owner = player;
            DamageRecord.Clear();
            Contacts.Clear();
            ContactedPlayers.Clear();
            Weapon = weapon;
            Stage = stage;
        }

        public bool GetMostContact(out ContactPoint2D result)
        {
            foreach (var contact in Contacts)
            {
                if(contact.rigidbody.gameObject == Owner.gameObject)
                    continue;

                result = contact;
                return true;
            }

            result = default;
            return false;
        }

        public void Terminate()
        {
            Owner = null;
            Weapon = null;
            Stage = null;
            BoxCollider.enabled = false;
            CircleCollider.enabled = false;
            
            OnTerminated?.Invoke(this);
            
            GameObjectPool.Release<DamageEntity>(this);
        }
        
    }
}