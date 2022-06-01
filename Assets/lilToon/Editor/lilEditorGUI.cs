#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    public class lilEditorGUI
    {
        public static GUIStyle boxOuter         = InitializeBox(4, 4, 2);
        public static GUIStyle boxInnerHalf     = InitializeBox(4, 2, 2);
        public static GUIStyle boxInner         = InitializeBox(4, 2, 2);
        public static GUIStyle customBox        = InitializeBox(6, 4, 4);
        public static GUIStyle customToggleFont = new GUIStyle();
        public static GUIStyle wrapLabel        = new GUIStyle();
        public static GUIStyle boldLabel        = new GUIStyle();
        public static GUIStyle foldout          = new GUIStyle();
        public static GUIStyle middleButton     = new GUIStyle(){alignment = TextAnchor.MiddleCenter};

        public static GUIStyle InitializeBox(int border, int margin, int padding)
        {
            return new GUIStyle
                {
                    border = new RectOffset(border, border, border, border),
                    margin = new RectOffset(margin, margin, margin, margin),
                    padding = new RectOffset(padding, padding, padding, padding),
                    overflow = new RectOffset(0, 0, 0, 0)
                };
        }

        public static bool Foldout(string title, bool display)
        {
            return Foldout(title, "", display);
        }

        public static bool Foldout(string title, string help, bool display)
        {
            Rect rect = GUILayoutUtility.GetRect(16f, 20f, foldout);
			rect.width += 8f;
			rect.x -= 8f;
            GUI.Box(rect, new GUIContent(title, help), foldout);

            Event e = Event.current;

            Rect toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
            if(e.type == EventType.Repaint) {
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
            }

            rect.width -= 24;
            if(e.type == EventType.MouseDown && rect.Contains(e.mousePosition)) {
                display = !display;
                e.Use();
            }

            return display;
        }

        public static void DrawLine()
        {
            EditorGUI.DrawRect(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, 1)), lilConstant.lineColor);
        }

        public static void DrawWebButton(string text, string URL)
        {
            Rect position = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
            GUIContent icon = EditorGUIUtility.IconContent("BuildSettings.Web.Small");
            icon.text = text;
            GUIStyle style = new GUIStyle(EditorStyles.label){padding = new RectOffset()};
            if(GUI.Button(position, icon, style)){
                Application.OpenURL(URL);
            }
        }

        public static bool DrawSimpleFoldout(string label, bool condition, GUIStyle style, bool isCustomEditor = true)
        {
            EditorGUI.indentLevel++;
            Rect position = EditorGUILayout.GetControlRect();
            EditorGUI.LabelField(position, label, style);
            EditorGUI.indentLevel--;

            position.x += isCustomEditor ? 0 : 10;
            return EditorGUI.Foldout(position, condition, "");
        }

        public static bool DrawSimpleFoldout(string label, bool condition, bool isCustomEditor = true)
        {
            return DrawSimpleFoldout(label, condition, boldLabel, isCustomEditor);
        }

        public static bool DrawSimpleFoldout(MaterialEditor materialEditor, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, bool condition, bool isCustomEditor = true)
        {
            EditorGUI.indentLevel++;
            Rect position = materialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            EditorGUI.indentLevel--;

            position.x += isCustomEditor ? 0 : 10;
            return EditorGUI.Foldout(position, condition, "");
        }

        public static bool DrawSimpleFoldout(MaterialEditor materialEditor, GUIContent guiContent, MaterialProperty textureName, bool condition, bool isCustomEditor = true)
        {
            EditorGUI.indentLevel++;
            Rect position = materialEditor.TexturePropertySingleLine(guiContent, textureName);
            EditorGUI.indentLevel--;

            position.x += isCustomEditor ? 0 : 10;
            return EditorGUI.Foldout(position, condition, "");
        }

        public static void InitializeGUIStyles()
        {
            wrapLabel = new GUIStyle(GUI.skin.label){wordWrap = true};
            boldLabel = new GUIStyle(GUI.skin.label){fontStyle = FontStyle.Bold};
            foldout = new GUIStyle("ShurikenModuleTitle")
            {
                fontSize = EditorStyles.label.fontSize,
                border = new RectOffset(15, 7, 4, 4),
                contentOffset = new Vector2(20f, -2f),
                fixedHeight = 22
            };
            if(EditorGUIUtility.isProSkin)
            {
                boxOuter.normal.background      = (Texture2D)EditorGUIUtility.Load(lilDirectoryManager.GetGUIBoxOutDarkPath());
                boxInnerHalf.normal.background  = (Texture2D)EditorGUIUtility.Load(lilDirectoryManager.GetGUIBoxInHalfDarkPath());
                boxInner.normal.background      = (Texture2D)EditorGUIUtility.Load(lilDirectoryManager.GetGUIBoxInDarkPath());
                customBox.normal.background     = (Texture2D)EditorGUIUtility.Load(lilDirectoryManager.GetGUICustomBoxDarkPath());
                customToggleFont = EditorStyles.label;
            }
            else
            {
                boxOuter.normal.background      = (Texture2D)EditorGUIUtility.Load(lilDirectoryManager.GetGUIBoxOutLightPath());
                boxInnerHalf.normal.background  = (Texture2D)EditorGUIUtility.Load(lilDirectoryManager.GetGUIBoxInHalfLightPath());
                boxInner.normal.background      = (Texture2D)EditorGUIUtility.Load(lilDirectoryManager.GetGUIBoxInLightPath());
                customBox.normal.background     = (Texture2D)EditorGUIUtility.Load(lilDirectoryManager.GetGUICustomBoxLightPath());
                customToggleFont = new GUIStyle();
                customToggleFont.normal.textColor = Color.white;
                customToggleFont.contentOffset = new Vector2(2f,0f);
            }
        }

        public static void DrawHelpButton(string helpAnchor)
        {
            Rect position = GUILayoutUtility.GetLastRect();
            position.x += position.width - 24;
            position.width = 24;
            if(GUI.Button(position, EditorGUIUtility.IconContent("_Help"), middleButton)){
                Application.OpenURL(GetLoc("sManualURL") + helpAnchor);
            }
        }

        public static bool AutoFixHelpBox(string message)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label(EditorGUIUtility.IconContent("console.warnicon"), GUILayout.ExpandWidth(false));
                GUILayout.Space(-EditorStyles.label.fontSize);
                GUILayout.BeginVertical();
                GUILayout.Label(message, EditorStyles.wordWrappedMiniLabel);
                    GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        bool pressed = GUILayout.Button(GetLoc("sFixNow"));
                    GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            return pressed;
        }

        public static string GetLoc(string value) { return lilLanguageManager.GetLoc(value); }
    }
}
#endif