using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Lyf.Utils.Extension
{
    [CustomPropertyDrawer(typeof(LabelAttribute))]
    public class LabelAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(attribute is LabelAttribute attr && attr.Name.Length > 0)
            {
                label.text = attr.Name;
            }
            EditorGUI.PropertyField(position, property, label);
        }
    }
}
#endif