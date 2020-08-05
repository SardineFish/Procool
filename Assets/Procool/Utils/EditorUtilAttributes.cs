﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Procool.Utils
{
    public interface ICustomEditorEX
    {

    }

    public abstract class CustomEditorAttribute : Attribute
    {

    }


    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class EditorButtonAttribute : CustomEditorAttribute
    {
        public string Label { get; private set; }

        public EditorButtonAttribute(string label = "")
        {
            Label = label;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class DisplayInInspectorAttribute : CustomEditorAttribute
    {
        public string Label { get; private set; }

        public DisplayInInspectorAttribute(string label = "") : base()
        {
            Label = label;
        }
    }

    public interface INotifyOnReload
    {
        void OnReload();
    }
}