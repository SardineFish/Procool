using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Procool.Editor;
using Procool.Utils;
using UnityEditor;

namespace Procool.Editor
{
    [CustomAttributeEditor(typeof(DisplayInInspectorAttribute))]
    class DisplayInInspectorEditor : AttributeEditor
    {
        public override void OnEdit(MemberInfo member, CustomEditorAttribute attr)
        {
            var readonlyAttr = attr as DisplayInInspectorAttribute;
            if (readonlyAttr == null)
                return;

            object value = "<error>";
            if (member.MemberType == MemberTypes.Property)
                value = (member as PropertyInfo)?.GetValue(target);
            else if (member.MemberType == MemberTypes.Field)
                value = (member as FieldInfo)?.GetValue(target);
            EditorUtils.Horizontal(() =>
            {
                EditorGUILayout.LabelField(readonlyAttr.Label == "" ? member.Name : readonlyAttr.Label);
                if (value is Array array)
                {
                    var str = "[";
                    foreach (var element in array)
                        str += element.ToString() + ",";
                    str += "]";

                    EditorGUILayout.LabelField(str);
                }
                else
                    EditorGUILayout.LabelField(value.ToString());
            });

        }
    }
}