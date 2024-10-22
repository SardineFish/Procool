﻿using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Procool.UI
{
    public class InputHint : MonoBehaviour
    {
        [SerializeField] private Image holdProgress;
        [SerializeField] private TMP_Text KeyText;
        [SerializeField] private TMP_Text HintText;
        
        private void Awake()
        {
            KeyText.text = "";
            HintText.text = "";
            var color = holdProgress.color;
            color.a = 0;
            holdProgress.color = color;
        }

        public void DisplayHint(InputAction action, string hint, float progress)
        {
            KeyText.text = InputControlText.Instance.GetText(action);
            HintText.text = hint;
            var color = holdProgress.color;
            color.a = progress;
            holdProgress.color = color;
        }
    }
}