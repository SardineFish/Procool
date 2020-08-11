using System;
using System.Collections.Generic;
using System.Linq;
using Procool.GameSystems;
using Procool.Random;
using Procool.Utils;
using UnityEngine;

namespace Procool.GamePlay.Weapon
{
    public partial class WeaponConstructor
    {
        public struct PossibleBehaviour
        {
            public BehaviourConstructData ConstructData;
            public float Probability;
            public IWeaponBehaviour Behaviour => ConstructData.Behaviour;

            public PossibleBehaviour(BehaviourConstructData constructData, float probability)
            {
                ConstructData = constructData;
                Probability = probability;
            }
        }

        public class BehaviourConstructData
        {
            public IWeaponBehaviour Behaviour;
            public bool IsEmitter = false;
            public float Probability = 1;
            public int StageDepthLimit = -1;
            public bool IsTerminator = false;
            public bool IsDefaultTerminator = false;
            public bool IsPrimary = false;
            public bool IsDetachEmitter = false;
            public bool IsCollider = false;
            public List<PossibleBehaviour> CompatibleParallels = new List<PossibleBehaviour>();
            public List<PossibleBehaviour> NextStages = new List<PossibleBehaviour>();
            public WeaponConstructor Constructor;

            public BehaviourConstructData(IWeaponBehaviour behaviour, WeaponConstructor constructor)
            {
                Behaviour = behaviour;
                Constructor = constructor;
            }
        }

        public class BehaviourConstructor<T> : BehaviourConstructData where T : IWeaponBehaviour
        {
            public BehaviourConstructor<T> CompatibleWith<TBehaviour>(float probability = 1)
                where TBehaviour : IWeaponBehaviour, new()
            {
                CompatibleParallels.Add(new PossibleBehaviour(Constructor.GetBehaviourConstructor<TBehaviour>(),
                    probability));
                return this;
            }

            public BehaviourConstructor<T> NextStage<TBehaviour>(float probability = 1)
                where TBehaviour : IWeaponBehaviour, new()
            {
                NextStages.Add(new PossibleBehaviour(Constructor.GetBehaviourConstructor<TBehaviour>(), probability));
                return this;
            }

            public BehaviourConstructor<T> Emitter()
            {
                IsEmitter = true;
                return this;
            }

            public BehaviourConstructor<T> DetachEmitter()
            {
                IsEmitter = true;
                IsDetachEmitter = true;
                return this;
            }

            /// <summary>
            /// The probability weight to have no next stage.
            /// </summary>
            /// <param name="probability"></param>
            /// <returns></returns>
            public BehaviourConstructor<T> Terminate(float probability)
            {
                NextStages.Add(new PossibleBehaviour(null, probability));
                return this;
            }

            public BehaviourConstructor<T> DepthLimit(int depth)
            {
                StageDepthLimit = depth;
                return this;
            }

            public BehaviourConstructor<T> Terminator()
            {
                IsTerminator = true;
                return this;
            }

            public BehaviourConstructor<T> Collider()
            {
                IsCollider = true;
                return this;
            }

            public BehaviourConstructor<T> Primary()
            {
                IsPrimary = true;
                return this;
            }

            public BehaviourConstructor<T> DefaultTerminator()
            {
                IsTerminator = true;
                IsDefaultTerminator = true;
                return this;
            }

            public BehaviourConstructor(IWeaponBehaviour behaviour, WeaponConstructor constructor) : base(behaviour,
                constructor)
            {
            }
        }

        private readonly List<PossibleBehaviour> PossibleBehaviours = new List<PossibleBehaviour>();

        Dictionary<System.Type, BehaviourConstructData> ConstructDatas =
            new Dictionary<System.Type, BehaviourConstructData>();

        Dictionary<IWeaponBehaviour, BehaviourConstructData> ConstructDataByBehaviour =
            new Dictionary<IWeaponBehaviour, BehaviourConstructData>();

        public BehaviourConstructor<T> Behaviour<T>(float probability = 1)
            where T : IWeaponBehaviour, new()
        {
            var constructor = GetBehaviourConstructor<T>();
            PossibleBehaviours.Add(new PossibleBehaviour(constructor, probability));
            return constructor;
        }

        BehaviourConstructor<T> GetBehaviourConstructor<T>() where T : IWeaponBehaviour, new()
        {
            var type = typeof(T);
            if (!ConstructDatas.ContainsKey(type))
            {
                var behaviour = new T();
                var constructData = new BehaviourConstructor<T>(behaviour, this);
                ConstructDatas[type] = constructData;
                ConstructDataByBehaviour[behaviour] = constructData;
                return constructData;
            }

            return ConstructDatas[type] as BehaviourConstructor<T>;
        }

        BehaviourConstructData GetBehaviourConstructor(IWeaponBehaviour behaviour)
        {
            var type = behaviour.GetType();
            
            if (!ConstructDatas.ContainsKey(type))
            {
                throw new Exception("Behaviour not found.");
            }

            return ConstructDatas[type];
        }
        

        bool CheckTerminator(DamageStage stage)
        {
            if (!stage || stage.Behaviours.Count <= 0)
                return false;
            foreach (var behaviour in stage.Behaviours)
            {
                if (GetBehaviourConstructor(behaviour.Behaviour).IsTerminator)
                    return true;
                return CheckTerminator(behaviour.NextStage);
            }
            
            throw new Exception("Unreachable code.");
        }

        bool CheckCollisionBehaviour(DamageStage stage)
        {
            if (!stage || stage.Behaviours.Count <= 0)
                return false;
            var needCollider = stage.Behaviours.Any(behavior =>
                behavior.Behaviour is Move || behavior.Behaviour is Throw || behavior.Behaviour is Laser);
            var hasCollider = stage.Behaviours.Any(data => GetBehaviourConstructor(data.Behaviour).IsCollider);

            if (needCollider && !hasCollider)
                return false;
            return true;
        }

        public DamageStage BuildStage(PRNG prng, IEnumerable<PossibleBehaviour> behaviours, bool firstStage, bool requirePrimary, bool detach, int depth,
            int depthLimit)
        {
            if (depth >= depthLimit)
                return null;

            var maxComponents = prng.GetInRange(2, 4);
            var possibleBehaviours = ObjectPool<Dictionary<IWeaponBehaviour, PossibleBehaviour>>.Get();
            possibleBehaviours.Clear();

            behaviours
                .Where(b => (!firstStage) || (firstStage && b.ConstructData.IsEmitter))
                .ForEach(behaviour => possibleBehaviours.Add(behaviour.Behaviour, behaviour));

            var pendingBehaviours = ObjectPool<List<BehaviourConstructData>>.Get();
            pendingBehaviours.Clear();

            if (possibleBehaviours.Count <= 0)
            {
                ObjectPool<Dictionary<IWeaponBehaviour, PossibleBehaviour>>.Release(possibleBehaviours);
                return null;
            }

            var stage = new DamageStage();
            stage.Detach = detach;

            if (firstStage)
            {
                var t = prng.GetScalar();
                var behaviour = behaviours
                    .Where(data => data.ConstructData.IsEmitter)
                    .RandomTake(t, data => data.Probability);
                pendingBehaviours.Add(behaviour.ConstructData);
            }
            else
            {
                for (var i = 0; i < maxComponents; i++)
                {
                    if (possibleBehaviours.Count <= 0)
                        break;
                    PossibleBehaviour behaviour;
                    if (i == 0 && requirePrimary)
                        behaviour = possibleBehaviours.Values
                            .Where(data => data.ConstructData.IsPrimary)
                            .RandomTake(prng.GetScalar(), b => b.Probability);
                    else
                        behaviour = possibleBehaviours.Values.RandomTake(prng.GetScalar(), b => b.Probability);

                    if (behaviour.ConstructData is null)
                    {
                        if (i == 0)
                        {
                            ObjectPool<Dictionary<IWeaponBehaviour, PossibleBehaviour>>.Release(possibleBehaviours);
                            return null;
                        }

                        continue;
                    }

                    pendingBehaviours.Add(behaviour.ConstructData);
                    // var behaviourData = behaviour.Behaviour.GenerateBehaviourData(prng);
                    // stage.Behaviours.Add(behaviourData);

                    var intersectSet = ObjectPool<Dictionary<IWeaponBehaviour, PossibleBehaviour>>.Get();
                    intersectSet.Clear();
                    if (i == 0)
                    {
                        foreach (var compatibleBehaviour in behaviour.ConstructData.CompatibleParallels)
                            intersectSet.Add(compatibleBehaviour.Behaviour, compatibleBehaviour);
                    }
                    else
                    {
                        foreach (var compatibleBehaviour in behaviour.ConstructData.CompatibleParallels)
                        {
                            if (possibleBehaviours.ContainsKey(compatibleBehaviour.Behaviour))
                            {
                                var currentPossible = possibleBehaviours[compatibleBehaviour.Behaviour];
                                intersectSet.Add(compatibleBehaviour.Behaviour,
                                    new PossibleBehaviour(compatibleBehaviour.ConstructData,
                                        currentPossible.Probability + compatibleBehaviour.Probability));
                            }
                        }
                    }

                    ObjectPool<Dictionary<IWeaponBehaviour, PossibleBehaviour>>.Release(possibleBehaviours);
                    possibleBehaviours = intersectSet;

                }
            }


            foreach (var constructData in pendingBehaviours)
            {
                var behaviourData = constructData.Behaviour.GenerateBehaviourData(prng);
                stage.Behaviours.Add(behaviourData);
                var nextDepthLimit = constructData.StageDepthLimit;
                if (nextDepthLimit < 0)
                    nextDepthLimit = depthLimit;
                
                behaviourData.NextStage = BuildStage(
                    prng, 
                    constructData.NextStages, 
                    false, 
                    constructData.IsEmitter,
                    constructData.IsDetachEmitter, 
                    depth + 1,
                    nextDepthLimit);
            }

            // We need at least one terminator to ensure damage entity will be terminated.
            if (detach && !CheckTerminator(stage))
            {
                var t = prng.GetScalar();
                var terminator = PossibleBehaviours
                    .Where(data => data.ConstructData.IsDefaultTerminator)
                    .RandomTake(t, data => data.Probability);
                
                stage.Behaviours.Add(terminator.Behaviour.GenerateBehaviourData(prng));
            }

            if (!CheckCollisionBehaviour(stage))
            {
                var collisionBehaviour = PossibleBehaviours
                    .Where(data => data.ConstructData.IsCollider && !data.ConstructData.IsPrimary)
                    .RandomTake(prng.GetScalar(), data => data.Probability);
                stage.Behaviours.Add(collisionBehaviour.Behaviour.GenerateBehaviourData(prng));
            }


            ObjectPool<Dictionary<IWeaponBehaviour, PossibleBehaviour>>.Release(possibleBehaviours);
            return stage;
        }

        public Weapon BuildWeapon(PRNG prng)
        {
            var weapon = new Weapon();

            var depthLimit = prng.GetInRange(5, 5);
            var stage = BuildStage(prng, PossibleBehaviours, true, false, false, 0, depthLimit);
            stage.IsFirstStage = true;

            weapon.FirstStage = stage;
            var damage = weapon.FirstStage.EvaluateDamage();
            weapon.Damage = 100 / damage;
            GenerateBulletVFX(weapon, prng);
            return weapon;
        }
    }
}