using System;
using Procool.Input;
using Procool.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Procool.GamePlay.Controller
{
    [RequireComponent(typeof(WorldPositionUI))]
    public class InteractiveObject : MonoBehaviour
    {
        [System.Serializable]
        public class InteractEvent : UnityEvent<Player>
        {
        }

        [SerializeField] private string InteractHintText = "Hold to Interact";
        public InteractEvent OnInteract;
        
        private WorldPositionUI _worldPositionUI;
        private InputHint _inputHintUI;

        private bool _interactive = true;
        public bool Interactive
        {
            get => _interactive;
            set
            {
                if (_interactive && !value)
                {
                    _worldPositionUI.Hide();
                    _inputHintUI = null;
                }

                _interactive = value;
            }
        }

        private void Awake()
        {
            _worldPositionUI = GetComponent<WorldPositionUI>();
        }

        public void Interact(Player player)
        {
            UpdateHint(0);
            if (Interactive)
                OnInteract?.Invoke(player);
        }

        public bool StillInRange(Player player)
        {
            return player.AvailableInteractiveObjects.Contains(this);
        }

        public void InteractKeyHold(float t)
        {
            if (Interactive)
                UpdateHint(t);
        }

        public float Distance(Player player)
            => Vector2.Distance(player.transform.position, transform.position);

        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!Interactive)
                return;
            
            if (other.attachedRigidbody?.GetComponent<Player>() is Player player && player)
            {
                player.AvailableInteractiveObjects.Add(this);
                _inputHintUI = _worldPositionUI.Show<InputHint>();
                UpdateHint(0);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.attachedRigidbody?.GetComponent<Player>() is Player player && player)
            {
                player.AvailableInteractiveObjects.Remove(this);
                _worldPositionUI.Hide();
            }
        }

        private void UpdateHint(float progress)
        {
            _inputHintUI?.DisplayHint(InputManager.Input.GamePlay.Interact, InteractHintText, progress);
        }

    }
}