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
    public class DamageEntity : ManagedMonobehaviour<DamageEntity>, IUsingState
    {
        public Player Owner { get; private set; }
        public List<ContactPoint2D> Contacts { get; } = new List<ContactPoint2D>(16);
        public HashSet<IDamageTarget> ContactedDamageTargets = new HashSet<IDamageTarget>();
        public Weapon Weapon { get; private set; }
        // public SpriteRenderer SpriteRenderer { get; private set; }
        // public CircleCollider2D CircleCollider { get; private set; }
        // private new CircleCollider2D collider;
        // private new Rigidbody2D rigidbody;

        public BulletVFX BulletVfx { get; private set; }

        public float PreviousEmitTime = 0;

        [SerializeField] private TrailRenderer trailRenderer;
        [SerializeField] private float physicsInterval = 0.1f;

        private ParallelCoroutineRunner runner;

        [TextArea]
        public string StageInfo;

        private void Awake()
        {
            // collider = GetComponent<CircleCollider2D>();
            // rigidbody = GetComponent<Rigidbody2D>();
        }

        // void HandleCollision(Collision2D other)
        // {
        //     other.GetContacts(Contacts);
        //     var player = other.rigidbody.GetComponent<Player>();
        //     if (player)
        //         ContactedPlayers.Add(player);
        // }
        //
        // private void OnCollisionEnter2D(Collision2D other)
        // {
        //     HandleCollision(other);
        // }
        //
        // private void OnCollisionStay2D(Collision2D other)
        // {
        //     HandleCollision(other);
        // }


        private float previousCollisionCheckTime = 0;
        private Vector2 previousCollisionCheckPos = Vector2.zero;
        private readonly List<RaycastHit2D> raycastHitsList = new List<RaycastHit2D>();
        private readonly RaycastHit2D[] raycastHitsArray = new RaycastHit2D[8];
        List<RaycastHit2D> QueryCollision()
        {
            if (Time.time < previousCollisionCheckTime + physicsInterval)
                return raycastHitsList;
        
            var distance = Vector2.Distance(transform.position, previousCollisionCheckPos);

            var count = Physics2D.CircleCastNonAlloc(previousCollisionCheckPos, BulletVfx.BulletSize, transform.position.ToVector2() - previousCollisionCheckPos, raycastHitsArray, distance);
            previousCollisionCheckPos = transform.position;
            previousCollisionCheckTime = Time.time;
            
            raycastHitsList.Clear();
            for (var i = 0; i < count; i++)
            {
                if(!raycastHitsArray[i].collider || raycastHitsArray[i].collider.isTrigger)
                    continue;
                raycastHitsList.Add(raycastHitsArray[i]);   
            }
        
            return raycastHitsList;
        }

        public IEnumerable<IDamageTarget> HitTarget()
        {
            var hits = QueryCollision();
            foreach (var hit in hits)
            {
                var target = hit.rigidbody?.GetComponent<IDamageTarget>();
                if (target != null && target != Owner && ContactedDamageTargets.Add(target))
                {
                    yield return target;
                }
            }
        }

        public IEnumerator Wait()
        {
            while (runner.Tick())
                yield return null;

            if (runner.Completed || runner.Aborted)
                Dispose();
        }

        public bool Tick()
            => runner.Tick();

        public Coroutine RunDetach()
        {
            return StartCoroutine(Wait());
        }

        public void AppendCoroutine(IEnumerator coroutine)
        {
            runner.Append(coroutine);
        }

        public void Init(Player player, Weapon weapon, Vector2 position, Quaternion rotation)
        {
            Owner = player;
            ContactedDamageTargets.Clear();
            Weapon = weapon;
            transform.position = position;
            transform.rotation = rotation;
            previousCollisionCheckPos = position;
            previousCollisionCheckTime = -10000;

            

            runner = new ParallelCoroutineRunner();
        }

        public void SetVFX(ref BulletVFX vfx)
        {
            BulletVfx = vfx;
            // SpriteRenderer.sprite = vfx.Sprite;
            // SpriteRenderer.color = vfx.SpriteColor;
            // flicker.enabled = vfx.Flicking;
            // flicker.renderer.color = vfx.FlickingColor;
            trailRenderer.endColor = trailRenderer.startColor = vfx.PrimaryColor;
            trailRenderer.startWidth = vfx.TrailStartWidth;
            trailRenderer.endWidth = vfx.TrailEndWidth;
            if (vfx.EnableTrail)
                trailRenderer.emitting = true;
            // trailRenderer.time = vfx.TrailLength;
        }

        public bool GetMostContact(out RaycastHit2D result)
        {
            var hits = QueryCollision();
            foreach (var hit in hits)
            {
                if(hit.rigidbody?.gameObject == Owner.gameObject)
                    continue;
        
                result = hit;
                return true;
            }
        
            result = default;
            return false;
        }

        public void Dispose()
        {
            Owner = null;
            Weapon = null;
            ContactedDamageTargets.Clear();
            GameObjectPool.Release<DamageEntity>(this);
        }
        

        public void Terminate()
        {
            runner.Abort();
            
        }
        
    }
}