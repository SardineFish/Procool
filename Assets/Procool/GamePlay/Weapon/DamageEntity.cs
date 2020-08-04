using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Procool.GamePlay.Controller;
using Procool.GamePlay.Inventory;
using Procool.Utils;
using Procool.VFX;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    [RequireComponent(typeof(CircleCollider2D), typeof(BoxCollider2D))]
    public class DamageEntity : ManagedMonobehaviour<DamageEntity>, IUsingState
    {
        public Player Owner { get; private set; }
        public List<ContactPoint2D> Contacts { get; } = new List<ContactPoint2D>(16);
        public HashSet<Player> ContactedPlayers = new HashSet<Player>();
        public HashSet<Player> DamageRecord { get; } = new HashSet<Player>();
        public Weapon Weapon { get; private set; }
        // public SpriteRenderer SpriteRenderer { get; private set; }
        public BoxCollider2D BoxCollider { get; private set; }
        public CircleCollider2D CircleCollider { get; private set; }

        public BulletVFX BulletVfx { get; private set; } = new BulletVFX()
        {
            BulletSize = .2f,
            PrimaryColor = Color.red,
            SecondaryColor = Color.white,
        };

        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private Flicker flicker;

        private ParallelCoroutineRunner runner;

        [TextArea]
        public string StageInfo;

        private void Awake()
        {
            // SpriteRenderer = GetComponent<SpriteRenderer>();
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

        // public IEnumerator Run()
        // {
        //     var runner = CoroutineRunner.All(Stage.Behaviours.Select(behaviour =>
        //         behaviour.Behaviour.Run(this, behaviour, Stage, Weapon)));
        //     var coroutine = StartCoroutine(runner);
        //     yield return coroutine;
        //     
        //     Terminate();
        // }

        public IEnumerator Wait()
        {
            while (runner.Tick())
                yield return null;

            if (runner.Completed)
                Terminate();
        }

        public Coroutine RunDetach()
        {
            return StartCoroutine(Wait());
        }

        public void AppendCoroutine(IEnumerator coroutine)
        {
            runner.Append(coroutine);
        }

        public void Init(Player player, Weapon weapon, Transform inheritTransform)
        {
            Owner = player;
            DamageRecord.Clear();
            Contacts.Clear();
            ContactedPlayers.Clear();
            Weapon = weapon;
            transform.position = inheritTransform.position;
            transform.rotation = inheritTransform.rotation;


            runner = new ParallelCoroutineRunner();
        }

        public void SetVFX(ref BulletVFX vfx)
        {
            BulletVfx = vfx;
            // SpriteRenderer.sprite = vfx.Sprite;
            // SpriteRenderer.color = vfx.SpriteColor;
            flicker.enabled = vfx.Flicking;
            flicker.renderer.color = vfx.FlickingColor;
            trailRenderer.endColor = trailRenderer.startColor = vfx.SpriteColor;
            trailRenderer.startWidth = vfx.TrailStartWidth;
            trailRenderer.endWidth = vfx.TrailEndWidth;
            // trailRenderer.time = vfx.TrailLength;
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
            runner.Abort();
            
            Owner = null;
            Weapon = null;
            BoxCollider.enabled = false;
            CircleCollider.enabled = false;
            
            GameObjectPool.Release<DamageEntity>(this);
        }
        
    }
}