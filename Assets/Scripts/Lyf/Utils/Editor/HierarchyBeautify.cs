#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Lyf.Utils.Editor
{
    /// <summary>
    /// Hierarchy窗口美化包, 来源: https://github.com/NicoIer/UnityToolkit/tree/main/Editor
    /// </summary>
    internal class HierarchyBeautify
    {
        private static HierarchyBeautify _instance;
        private static Event CurrentEvent => Event.current;
        private float _nameDistance;

        private Object _selectComponent;
        private readonly GUIContent _content;
        private readonly HierarchyIcon _icon;

        private HierarchyBeautify()
        {
            _icon = new HierarchyIcon();
            _content = new GUIContent();
            EditorApplication.hierarchyWindowItemOnGUI -= HierarchyOnGUI;
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyOnGUI;
        }

        [InitializeOnLoadMethod]
        private static void Enable() => _instance = new HierarchyBeautify();

        private void HierarchyOnGUI(int instanceId, Rect selectionRect)
        {
            _icon.Dispose();
            _icon.ID = instanceId;
            _icon.IconRect = selectionRect;
            _icon.GameObject = (GameObject)EditorUtility.InstanceIDToObject(_icon.ID);
            if (_icon.GameObject == null) return;
            _icon.Display();
            _icon.NameRect = _icon.IconRect;
            var nameStyle = new GUIStyle(TreeView.DefaultStyles.label);
            _icon.NameRect.width = nameStyle.CalcSize(new GUIContent(_icon.GameObject.name)).x;
            _icon.NameRect.x += Const.Int8;
            _nameDistance = _icon.NameRect.x + _icon.NameRect.width;
            _nameDistance += Const.Int8;
            ShowSplitLine();
            ShowComponent();
        }

        private void ShowSplitLine()
        {
            if (CurrentEvent.type != EventType.Repaint) return;
            var rect = _icon.IconRect;
            rect.xMin = Const.Int32;
            rect.y += Const.Int16 - 1;
            rect.width += Const.Int16;
            rect.height = 1;
            var color = GUI.color;
            GUI.color = new Color(0, 0, 0, 0.2f);
            GUI.DrawTexture(rect, GetTexture(), ScaleMode.StretchToFill);
            GUI.color = color;
        }

        private void ShowComponent()
        {
            var renderer = _icon.GameObject.GetComponent<Renderer>();
            var components = _icon.GameObject.GetComponents(typeof(Component)).ToList<Object>();

            var hasMaterial = renderer != null && renderer.sharedMaterial != null;
            if (hasMaterial)
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    components.Add(material);
                }
            }

            var count = components.Count;
            _nameDistance += Const.Int4;

            for (int i = 0; i < count; ++i)
            {
                var component = components[i];
                if (component == null) continue;
                var type = component.GetType();
                var rect = ComponentPosition(_icon.NameRect, Const.Int12, ref _nameDistance);
                if (hasMaterial && i == count - renderer.sharedMaterials.Length)
                {
                    foreach (var material in renderer.sharedMaterials)
                    {
                        if (material == null) continue;
                        ComponentIcon(material, type, rect, true);
                        rect = ComponentPosition(_icon.NameRect, Const.Int12, ref _nameDistance);
                    }

                    break;
                }

                ComponentIcon(component, type, rect, false);
                _nameDistance += Const.Int2;
            }
        }

        private void ComponentIcon(Object component, Type componentType, Rect rect, bool isMaterial)
        {
            if (CurrentEvent.type == EventType.Repaint)
            {
                var image = EditorGUIUtility.ObjectContent(component, componentType).image;
                var tooltip = isMaterial ? component.name : componentType.Name;
                _content.tooltip = tooltip;
                GUI.Box(rect, _content, GUIStyle.none);
                GUI.DrawTexture(rect, image, ScaleMode.ScaleToFit);
            }

            if (rect.Contains(CurrentEvent.mousePosition))
            {
                if (CurrentEvent.type == EventType.MouseDown)
                {
                    if (CurrentEvent.button == 0)
                    {
                        _selectComponent = component;
                        UtilityRef.DisplayObjectContextMenu(rect, component, 0);
                        CurrentEvent.Use();
                        return;
                    }
                }
            }

            if (_selectComponent != null && CurrentEvent.type == EventType.MouseDown && CurrentEvent.button == 0 &&
                !rect.Contains(CurrentEvent.mousePosition))
            {
                _selectComponent = null;
            }
        }

        private static Rect ComponentPosition(Rect rect, float width, ref float result)
        {
            rect.xMin = 0;
            rect.x += result;
            rect.width = width;
            result += width;
            return rect;
        }

        private static Texture2D GetTexture()
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            return texture;
        }

        private class HierarchyIcon
        {
            public int ID;
            public Rect IconRect;
            public Rect NameRect;
            public GameObject GameObject;

            public void Display()
            {
                var pos = CurrentEvent.mousePosition;
                var isHover = pos.x >= 0 && pos.x <= IconRect.xMax + Const.Int16 && pos.y >= IconRect.y &&
                              pos.y < IconRect.yMax;
                if (!isHover) return;
                var rect = new Rect(Const.Int32 + 0.5f, IconRect.y, Const.Int16, IconRect.height);
                var isShow = EditorGUI.Toggle(rect, GUIContent.none, GameObject.activeSelf);
                var active = GameObject.activeSelf;
                GameObject.SetActive(isShow);
                if (active != GameObject.activeSelf)
                {
                    EditorUtility.SetDirty(GameObject);
                }
            }

            public void Dispose()
            {
                ID = int.MinValue;
                IconRect = Rect.zero;
                NameRect = Rect.zero;
                GameObject = null;
            }
        }

        private struct Const
        {
            public const int Int2 = 2;
            public const int Int4 = 4;
            public const int Int8 = 8;
            public const int Int12 = 12;
            public const int Int16 = 16;
            public const int Int32 = 32;
        }
    }
}
#endif