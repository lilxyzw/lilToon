#if UNITY_EDITOR
using System;
using System.Linq;
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
            var rect = GUILayoutUtility.GetRect(16f, 20f, foldout);
            rect.width += 8f;
            rect.x -= 8f;
            GUI.Box(rect, new GUIContent(title, help), foldout);

            var e = Event.current;

            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
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
            EditorGUI.DrawRect(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, 1)), lilConstants.lineColor);
        }

        public static void DrawWebButton(string text, string URL)
        {
            var position = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
            var icon = EditorGUIUtility.IconContent("BuildSettings.Web.Small");
            icon.text = text;
            var style = new GUIStyle(EditorStyles.label){padding = new RectOffset()};
            if(GUI.Button(position, icon, style)){
                Application.OpenURL(URL);
            }
        }

        public static bool DrawSimpleFoldout(string label, bool condition, GUIStyle style, bool isCustomEditor = true)
        {
            EditorGUI.indentLevel++;
            var position = EditorGUILayout.GetControlRect();
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
            var position = TexturePropertySingleLine(materialEditor, guiContent, textureName, rgba);
            EditorGUI.indentLevel--;

            position.x += isCustomEditor ? 0 : 10;
            return EditorGUI.Foldout(position, condition, "");
        }

        public static bool DrawSimpleFoldout(MaterialEditor materialEditor, GUIContent guiContent, MaterialProperty textureName, bool condition, bool isCustomEditor = true)
        {
            EditorGUI.indentLevel++;
            var position = TexturePropertySingleLine(materialEditor, guiContent, textureName);
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
                font = EditorStyles.label.font,
                fontSize = EditorStyles.label.fontSize,
                fontStyle = EditorStyles.label.fontStyle,
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
            var position = GUILayoutUtility.GetLastRect();
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

        public static bool Button(string label)
        {
            if(!CheckPropertyToDraw(label)) return false;
            return GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), label);
        }

        public static bool[] Buttons(string label, params string[] labels)
        {
            if(!CheckPropertyToDraw(label)) return Enumerable.Repeat(false,labels.Length).ToArray();
            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            var res = labels.Select(l => GUILayout.Button(l)).ToArray();
            GUILayout.EndHorizontal();
            return res;
        }

        public static int Popup(string label, int i, params string[] labels)
        {
            if(CheckPropertyToDraw(label)) return EditorGUILayout.Popup(label, i, labels);
            else                           return i;
        }

        public static int IntSlider(string label, int i, int min, int max)
        {
            if(CheckPropertyToDraw(label)) return EditorGUILayout.IntSlider(label, i, min, max);
            else                           return i;
        }

        public static float Slider(string label, float i, float min, float max)
        {
            if(CheckPropertyToDraw(label)) return EditorGUILayout.Slider(label, i, min, max);
            else                           return i;
        }

        public static float FloatField(string label, float f)
        {
            if(CheckPropertyToDraw(label)) return EditorGUILayout.FloatField(label, f);
            else                           return f;
        }

        public static bool Toggle(string label, bool b)
        {
            if(CheckPropertyToDraw(label)) return EditorGUILayout.Toggle(label, b);
            else                           return b;
        }

        public static Vector2 Vector2Field(string label, Vector2 v)
        {
            if(CheckPropertyToDraw(label)) return EditorGUILayout.Vector2Field(label, v);
            else                           return v;
        }

        //------------------------------------------------------------------------------------------------------------------------------
        // Property drawer
        #region
        public static void LocalizedProperty(MaterialEditor materialEditor, MaterialProperty prop, bool shouldCheck = true)
        {
            if(!shouldCheck || CheckPropertyToDraw(prop))
                materialEditor.ShaderProperty(prop, lilLanguageManager.GetDisplayName(prop));
        }

        public static void LocalizedProperty(MaterialEditor materialEditor, MaterialProperty prop, string label, bool shouldCheck = true)
        {
            if(!shouldCheck || CheckPropertyToDraw(prop))
            {
                if(Event.current.alt)
                {
                    if(label.Contains("|")) label = prop.name + label.Substring(label.IndexOf("|"));
                    else label = prop.name;
                }
                materialEditor.ShaderProperty(prop, lilLanguageManager.GetDisplayName(label));
            }
        }

        public static void LocalizedProperty(MaterialEditor materialEditor, MaterialProperty prop, int indent, bool shouldCheck = true)
        {
            if(!shouldCheck || CheckPropertyToDraw(prop))
                materialEditor.ShaderProperty(prop, lilLanguageManager.GetDisplayName(prop), indent);
        }

        public static void LocalizedPropertyColorWithAlpha(MaterialEditor materialEditor, MaterialProperty prop, bool shouldCheck = true)
        {
            if(!shouldCheck || CheckPropertyToDraw(prop))
            {
                LocalizedProperty(materialEditor, prop);
                DrawColorAsAlpha(prop);
            }
        }

        public static Rect TexturePropertySingleLine(MaterialEditor materialEditor, GUIContent content, MaterialProperty tex)
        {
            if(Event.current.alt) content = new GUIContent(tex.name);
            return materialEditor.TexturePropertySingleLine(content, tex);
        }

        public static Rect TexturePropertySingleLine(MaterialEditor materialEditor, GUIContent content, MaterialProperty tex, MaterialProperty prop)
        {
            if(Event.current.alt)
            {
                if(prop == null) content = new GUIContent(tex.name);
                else             content = new GUIContent(tex.name + ", " + prop.name);
            }
            return materialEditor.TexturePropertySingleLine(content, tex, prop);
        }

        public static void LocalizedPropertyTexture(MaterialEditor materialEditor, GUIContent content, MaterialProperty tex, bool shouldCheck = true)
        {
            if(!shouldCheck || CheckPropertyToDraw(tex) || CheckPropertyToDraw(content))
                TexturePropertySingleLine(materialEditor, content, tex);
        }

        public static void LocalizedPropertyTexture(MaterialEditor materialEditor, GUIContent content, MaterialProperty tex, MaterialProperty color, bool shouldCheck = true)
        {
            if(!shouldCheck || CheckPropertyToDraw(tex) || CheckPropertyToDraw(color) || CheckPropertyToDraw(content))
                TexturePropertySingleLine(materialEditor, content, tex, color);
        }

        public static void LocalizedPropertyTextureWithAlpha(MaterialEditor materialEditor, GUIContent content, MaterialProperty tex, MaterialProperty color, bool shouldCheck = true)
        {
            if(!shouldCheck || CheckPropertyToDraw(tex) || CheckPropertyToDraw(color) || CheckPropertyToDraw(content))
            {
                TexturePropertySingleLine(materialEditor, content, tex, color);
                DrawColorAsAlpha(color);
            }
        }

        public static void LocalizedPropertyAlpha(MaterialProperty prop, bool shouldCheck = true)
        {
            if(!shouldCheck || CheckPropertyToDraw(prop))
                DrawColorAsAlpha(prop);
        }

        public static bool CheckPropertyToDraw(MaterialProperty prop)
        {
            return string.IsNullOrEmpty(lilToonInspector.edSet.searchKeyWord) || lilLanguageManager.GetDisplayName(prop).Contains(lilToonInspector.edSet.searchKeyWord) || prop.name.Contains(lilToonInspector.edSet.searchKeyWord);
        }

        public static bool CheckPropertyToDraw(params MaterialProperty[] props)
        {
            return string.IsNullOrEmpty(lilToonInspector.edSet.searchKeyWord) || props.Count(prop => lilLanguageManager.GetDisplayName(prop).Contains(lilToonInspector.edSet.searchKeyWord) || prop.name.Contains(lilToonInspector.edSet.searchKeyWord)) > 0;
        }

        public static bool CheckPropertyToDraw(GUIContent content)
        {
            return string.IsNullOrEmpty(lilToonInspector.edSet.searchKeyWord) ||
                !string.IsNullOrEmpty(content.text) && content.text.Contains(lilToonInspector.edSet.searchKeyWord) ||
                !string.IsNullOrEmpty(content.tooltip) && content.tooltip.Contains(lilToonInspector.edSet.searchKeyWord);
        }

        public static bool CheckPropertyToDraw(string label)
        {
            return string.IsNullOrEmpty(lilToonInspector.edSet.searchKeyWord) || label.Contains(lilToonInspector.edSet.searchKeyWord);
        }

        public static bool CheckPropertyToDraw(params string[] labels)
        {
            return string.IsNullOrEmpty(lilToonInspector.edSet.searchKeyWord) || labels.Count(l => lilLanguageManager.GetDisplayName(l).Contains(lilToonInspector.edSet.searchKeyWord)) > 0;
        }

        public static bool CheckPropertyToDraw()
        {
            return string.IsNullOrEmpty(lilToonInspector.edSet.searchKeyWord);
        }

        public static void UV4Decal(MaterialEditor m_MaterialEditor, MaterialProperty isDecal, MaterialProperty isLeftOnly, MaterialProperty isRightOnly, MaterialProperty shouldCopy, MaterialProperty shouldFlipMirror, MaterialProperty shouldFlipCopy, MaterialProperty tex, MaterialProperty SR, MaterialProperty angle, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty uvMode)
        {
            if(!CheckPropertyToDraw(isDecal, isLeftOnly, isRightOnly, shouldCopy, shouldFlipMirror, shouldFlipCopy, tex, SR, angle, decalAnimation, decalSubParam, uvMode)) return;
            LocalizedProperty(m_MaterialEditor, uvMode);
            ConvertGifToAtlas(tex, decalAnimation, decalSubParam, isDecal);
            // Toggle decal
            EditorGUI.BeginChangeCheck();
            LocalizedProperty(m_MaterialEditor, isDecal);
            if(EditorGUI.EndChangeCheck() && isDecal.floatValue == 0.0f)
            {
                isLeftOnly.floatValue = 0.0f;
                isRightOnly.floatValue = 0.0f;
                shouldFlipMirror.floatValue = 0.0f;
                shouldCopy.floatValue = 0.0f;
                shouldFlipCopy.floatValue = 0.0f;
            }

            if(isDecal.floatValue == 1.0f)
            {
                EditorGUI.indentLevel++;
                // Mirror mode
                // 0 : Normal
                // 1 : Flip
                // 2 : Left only
                // 3 : Right only
                // 4 : Right only (Flip)
                int mirrorMode = 0;
                if(isRightOnly.floatValue == 1.0f) mirrorMode = 3;
                if(shouldFlipMirror.floatValue == 1.0f) mirrorMode++;
                if(isLeftOnly.floatValue == 1.0f) mirrorMode = 2;

                EditorGUI.BeginChangeCheck();
                string mmlabel = Event.current.alt ? isLeftOnly.name + ", " + isRightOnly.name + ", " + shouldFlipMirror.name : GetLoc("sMirrorMode");
                mirrorMode = Popup(mmlabel,mirrorMode,new string[]{GetLoc("sMirrorModeNormal"),GetLoc("sMirrorModeFlip"),GetLoc("sMirrorModeLeft"),GetLoc("sMirrorModeRight"),GetLoc("sMirrorModeRightFlip")});
                if(EditorGUI.EndChangeCheck())
                {
                    if(mirrorMode == 0)
                    {
                        isLeftOnly.floatValue = 0.0f;
                        isRightOnly.floatValue = 0.0f;
                        shouldFlipMirror.floatValue = 0.0f;
                    }
                    if(mirrorMode == 1)
                    {
                        isLeftOnly.floatValue = 0.0f;
                        isRightOnly.floatValue = 0.0f;
                        shouldFlipMirror.floatValue = 1.0f;
                    }
                    if(mirrorMode == 2)
                    {
                        isLeftOnly.floatValue = 1.0f;
                        isRightOnly.floatValue = 0.0f;
                        shouldFlipMirror.floatValue = 0.0f;
                    }
                    if(mirrorMode == 3)
                    {
                        isLeftOnly.floatValue = 0.0f;
                        isRightOnly.floatValue = 1.0f;
                        shouldFlipMirror.floatValue = 0.0f;
                    }
                    if(mirrorMode == 4)
                    {
                        isLeftOnly.floatValue = 0.0f;
                        isRightOnly.floatValue = 1.0f;
                        shouldFlipMirror.floatValue = 1.0f;
                    }
                }

                // Copy mode
                // 0 : Normal
                // 1 : Symmetry
                // 2 : Flip
                int copyMode = 0;
                if(shouldCopy.floatValue == 1.0f) copyMode = 1;
                if(shouldFlipCopy.floatValue == 1.0f) copyMode = 2;

                EditorGUI.BeginChangeCheck();
                string cmlabel = Event.current.alt ? shouldCopy.name + ", " + shouldFlipCopy.name : GetLoc("sCopyMode");
                copyMode = Popup(cmlabel,copyMode,new string[]{GetLoc("sCopyModeNormal"),GetLoc("sCopyModeSymmetry"),GetLoc("sCopyModeFlip")});
                if(EditorGUI.EndChangeCheck())
                {
                    if(copyMode == 0)
                    {
                        shouldCopy.floatValue = 0.0f;
                        shouldFlipCopy.floatValue = 0.0f;
                    }
                    if(copyMode == 1)
                    {
                        shouldCopy.floatValue = 1.0f;
                        shouldFlipCopy.floatValue = 0.0f;
                    }
                    if(copyMode == 2)
                    {
                        shouldCopy.floatValue = 1.0f;
                        shouldFlipCopy.floatValue = 1.0f;
                    }
                }

                // Load scale & offset
                float scaleX = tex.textureScaleAndOffset.x;
                float scaleY = tex.textureScaleAndOffset.y;
                float posX = tex.textureScaleAndOffset.z;
                float posY = tex.textureScaleAndOffset.w;

                // scale & offset -> scale & position
                if(scaleX==0.0f)
                {
                    posX = 0.5f;
                    scaleX = 0.000001f;
                }
                else
                {
                    posX = (0.5f - posX) / scaleX;
                    scaleX = 1.0f / scaleX;
                }

                if(scaleY==0.0f)
                {
                    posY = 0.5f;
                    scaleY = 0.000001f;
                }
                else
                {
                    posY = (0.5f - posY) / scaleY;
                    scaleY = 1.0f / scaleY;
                }
                scaleX = RoundFloat1000000(scaleX);
                scaleY = RoundFloat1000000(scaleY);
                posX = RoundFloat1000000(posX);
                posY = RoundFloat1000000(posY);

                // If uv copy enables, fix position
                EditorGUI.BeginChangeCheck();
                if(copyMode > 0)
                {
                    if(posX < 0.5f) posX = 1.0f - posX;
                    posX = EditorGUILayout.Slider(Event.current.alt ? tex.name + "_ST.z" : GetLoc("sPositionX"), posX, 0.5f, 1.0f);
                }
                else
                {
                    posX = EditorGUILayout.Slider(Event.current.alt ? tex.name + "_ST.z" : GetLoc("sPositionX"), posX, 0.0f, 1.0f);
                }

                // Draw properties
                posY = EditorGUILayout.Slider(Event.current.alt ? tex.name + "_ST.w" : GetLoc("sPositionY"), posY, 0.0f, 1.0f);
                scaleX = EditorGUILayout.Slider(Event.current.alt ? tex.name + "_ST.x" : GetLoc("sScaleX"), scaleX, -1.0f, 1.0f);
                scaleY = EditorGUILayout.Slider(Event.current.alt ? tex.name + "_ST.y" : GetLoc("sScaleY"), scaleY, -1.0f, 1.0f);
                if(EditorGUI.EndChangeCheck())
                {
                    // Avoid division by zero
                    if(scaleX == 0.0f) scaleX = 0.000001f;
                    if(scaleY == 0.0f) scaleY = 0.000001f;

                    // scale & position -> scale & offset
                    scaleX = 1.0f / scaleX;
                    scaleY = 1.0f / scaleY;
                    posX = (-posX * scaleX) + 0.5f;
                    posY = (-posY * scaleY) + 0.5f;

                    tex.textureScaleAndOffset = new Vector4(scaleX, scaleY, posX, posY);
                }

                LocalizedProperty(m_MaterialEditor, angle);
                ScrollAndRotateGUI(SR);
                EditorGUI.indentLevel--;
                LocalizedProperty(m_MaterialEditor, decalAnimation);
                LocalizedProperty(m_MaterialEditor, decalSubParam);
            }
            else
            {
                UVSettingGUI(m_MaterialEditor, tex);
                LocalizedProperty(m_MaterialEditor, angle);
                ScrollAndRotateGUI(SR);
            }

            if(Button(GetLoc("sReset")) && EditorUtility.DisplayDialog(GetLoc("sDialogResetUV"),GetLoc("sDialogResetUVMes"),GetLoc("sYes"),GetLoc("sNo")))
            {
                uvMode.floatValue = 0.0f;
                isDecal.floatValue = 0.0f;
                isLeftOnly.floatValue = 0.0f;
                isRightOnly.floatValue = 0.0f;
                shouldCopy.floatValue = 0.0f;
                shouldFlipMirror.floatValue = 0.0f;
                shouldFlipCopy.floatValue = 0.0f;
                angle.floatValue = 0.0f;
                decalAnimation.vectorValue = new Vector4(1.0f,1.0f,1.0f,30.0f);
                decalSubParam.vectorValue = new Vector4(1.0f,1.0f,0.01f,1.0f);
            }
        }

        private static void ScrollAndRotateGUI(MaterialProperty prop)
        {
            if(!CheckPropertyToDraw(prop)) return;
            var scroll = new Vector2(prop.vectorValue.x, prop.vectorValue.y);
            float angle = Radian2Degree(prop.vectorValue.z);
            float rotate = RoundFloat1000000(prop.vectorValue.w / Mathf.PI * 0.5f);

            EditorGUI.BeginChangeCheck();

            var positionVec2 = EditorGUILayout.GetControlRect();

            // Scroll label
            float labelWidth = EditorGUIUtility.labelWidth;
            var labelRect = new Rect(positionVec2.x, positionVec2.y, labelWidth, positionVec2.height);
            EditorGUI.PrefixLabel(labelRect, new GUIContent(Event.current.alt ? prop.name + "_ST.xy" : GetLoc("sScroll")));

            // Copy & Reset indent
            int indentBuf = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Scroll
            var vecRect = new Rect(positionVec2.x + labelWidth, positionVec2.y, positionVec2.width - labelWidth, positionVec2.height);
            scroll = EditorGUI.Vector2Field(vecRect, GUIContent.none, scroll);

            // Revert indent
            EditorGUI.indentLevel = indentBuf;

            // Rotate
            rotate = EditorGUI.FloatField(EditorGUILayout.GetControlRect(), Event.current.alt ? prop.name + "_ST.w" : GetLoc("sRotate"), rotate);

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(scroll.x, scroll.y, Degree2Radian(angle), rotate * Mathf.PI * 2.0f);
            }
        }

        public static void ToneCorrectionGUI(MaterialEditor m_MaterialEditor, MaterialProperty hsvg)
        {
            LocalizedProperty(m_MaterialEditor, hsvg);
            // Reset
            if(Button(GetLoc("sReset")))
            {
                hsvg.vectorValue = lilConstants.defaultHSVG;
            }
        }

        public static void ToneCorrectionGUI(MaterialEditor m_MaterialEditor, MaterialProperty hsvg, int indent)
        {
            EditorGUI.indentLevel += indent;
            ToneCorrectionGUI(m_MaterialEditor, hsvg);
            EditorGUI.indentLevel -= indent;
        }

        public static void DrawVectorAs4Float(MaterialProperty prop, string label0, string label1, string label2, string label3)
        {
            if(!CheckPropertyToDraw(prop)) return;
            float param1 = prop.vectorValue.x;
            float param2 = prop.vectorValue.y;
            float param3 = prop.vectorValue.z;
            float param4 = prop.vectorValue.w;
            if(Event.current.alt)
            {
                label0 = prop.name + ".x";
                label1 = prop.name + ".y";
                label2 = prop.name + ".z";
                label3 = prop.name + ".w";
            }

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            param1 = EditorGUILayout.FloatField(label0, param1);
            param2 = EditorGUILayout.FloatField(label1, param2);
            param3 = EditorGUILayout.FloatField(label2, param3);
            param4 = EditorGUILayout.FloatField(label3, param4);
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.vectorValue = new Vector4(param1, param2, param3, param4);
            }
        }

        public static void DrawColorAsAlpha(MaterialProperty prop, string label)
        {
            if(!CheckPropertyToDraw(label)) return;
            float alpha = prop.colorValue.a;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            alpha = EditorGUILayout.Slider(Event.current.alt ? prop.name + ".a" : label, alpha, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;

            if(EditorGUI.EndChangeCheck())
            {
                prop.colorValue = new Color(prop.colorValue.r, prop.colorValue.g, prop.colorValue.b, alpha);
            }
        }

        public static void DrawColorAsAlpha(MaterialProperty prop)
        {
            DrawColorAsAlpha(prop, GetLoc("sAlpha"));
        }

        public static void RemoveUnusedPropertiesGUI(Material material)
        {
            if(Button(GetLoc("sRemoveUnused")))
            {
                Undo.RecordObject(material, "Remove unused properties");
                lilMaterialUtils.RemoveUnusedTexture(material, material.shader.name.Contains("Lite"));
            }
        }

        public static void SetAlphaIsTransparencyGUI(MaterialProperty tex)
        {
            if(CheckPropertyToDraw(tex) && tex.textureValue is Texture2D && !((Texture2D)tex.textureValue).alphaIsTransparency && AutoFixHelpBox(GetLoc("sNotAlphaIsTransparency")))
            {
                string path = AssetDatabase.GetAssetPath(tex.textureValue);
                var textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
                textureImporter.alphaIsTransparency = true;
                AssetDatabase.ImportAsset(path);
            }
        }

        public static void UVSettingGUI(MaterialEditor m_MaterialEditor, MaterialProperty uvst)
        {
            if(!CheckPropertyToDraw(uvst)) return;
            m_MaterialEditor.TextureScaleOffsetProperty(uvst);
        }

        public static void UVSettingGUI(MaterialEditor m_MaterialEditor, MaterialProperty uvst, MaterialProperty uvsr)
        {
            if(!CheckPropertyToDraw(uvst, uvsr)) return;
            EditorGUILayout.LabelField(GetLoc("sUVSetting"), boldLabel);
            UVSettingGUI(m_MaterialEditor, uvst);
            LocalizedProperty(m_MaterialEditor, uvsr);
        }

        public static void InvBorderGUI(MaterialProperty prop)
        {
            if(!CheckPropertyToDraw(prop)) return;
            float f = prop.floatValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            f = 1.0f - EditorGUILayout.Slider(Event.current.alt ? prop.name : GetLoc("sBorder"), 1.0f - f, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = f;
            }
        }

        public static void MinusRangeGUI(MaterialProperty prop, string label)
        {
            if(!CheckPropertyToDraw(prop)) return;
            float f = -prop.floatValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            f = EditorGUILayout.Slider(Event.current.alt ? prop.name : label, f, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = -f;
            }
        }

        public static void BlendSettingGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, string labelName, MaterialProperty srcRGB, MaterialProperty dstRGB, MaterialProperty srcA, MaterialProperty dstA, MaterialProperty opRGB, MaterialProperty opA)
        {
            if(!CheckPropertyToDraw(srcRGB, dstRGB, srcA, dstA, opRGB, opA)) return;
            isShow = DrawSimpleFoldout(labelName, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                LocalizedProperty(m_MaterialEditor, srcRGB);
                LocalizedProperty(m_MaterialEditor, dstRGB);
                LocalizedProperty(m_MaterialEditor, srcA);
                LocalizedProperty(m_MaterialEditor, dstA);
                LocalizedProperty(m_MaterialEditor, opRGB);
                LocalizedProperty(m_MaterialEditor, opA);
                EditorGUI.indentLevel--;
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName)
        {
            if(!CheckPropertyToDraw(guiContent) && !CheckPropertyToDraw(textureName)) return;
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                UVSettingGUI(m_MaterialEditor, textureName);
                EditorGUI.indentLevel--;
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba)
        {
            if(!CheckPropertyToDraw(guiContent) && !CheckPropertyToDraw(textureName, rgba)) return;
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                UVSettingGUI(m_MaterialEditor, textureName);
                EditorGUI.indentLevel--;
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty uvMode, string sUVMode)
        {
            if(!CheckPropertyToDraw(guiContent) && !CheckPropertyToDraw(textureName, rgba, uvMode)) return;
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                UVSettingGUI(m_MaterialEditor, textureName);
                LocalizedProperty(m_MaterialEditor, uvMode, sUVMode);
                EditorGUI.indentLevel--;
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate)
        {
            if(!CheckPropertyToDraw(guiContent) && !CheckPropertyToDraw(textureName, rgba, scrollRotate)) return;
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                UVSettingGUI(m_MaterialEditor, textureName);
                LocalizedProperty(m_MaterialEditor, scrollRotate, GetLoc("sScroll"));
                EditorGUI.indentLevel--;
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, bool useCustomUV, bool useUVAnimation)
        {
            if(!CheckPropertyToDraw(guiContent) && !CheckPropertyToDraw(textureName, rgba, scrollRotate)) return;
            if(useCustomUV)
            {
                isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
                if(isShow)
                {
                    EditorGUI.indentLevel++;
                    if(useUVAnimation)  UVSettingGUI(m_MaterialEditor, textureName, scrollRotate);
                    else                UVSettingGUI(m_MaterialEditor, textureName);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                TexturePropertySingleLine(m_MaterialEditor, guiContent, textureName, rgba);
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, MaterialProperty uvMode, bool useCustomUV, bool useUVAnimation)
        {
            if(!CheckPropertyToDraw(guiContent) && !CheckPropertyToDraw(textureName, rgba, scrollRotate, uvMode)) return;
            if(useCustomUV)
            {
                isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
                if(isShow)
                {
                    EditorGUI.indentLevel++;
                    if(useUVAnimation)  UVSettingGUI(m_MaterialEditor, textureName, scrollRotate);
                    else                UVSettingGUI(m_MaterialEditor, textureName);
                    LocalizedProperty(m_MaterialEditor, uvMode);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                TexturePropertySingleLine(m_MaterialEditor, guiContent, textureName, rgba);
            }
        }

        public static void MatCapTextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            if(!CheckPropertyToDraw(guiContent) && !CheckPropertyToDraw(blendUV1, zRotCancel, perspective, vrParallaxStrength)) return;
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                UVSettingGUI(m_MaterialEditor, textureName);
                LocalizedProperty(m_MaterialEditor, blendUV1);
                LocalizedProperty(m_MaterialEditor, zRotCancel);
                LocalizedProperty(m_MaterialEditor, perspective);
                LocalizedProperty(m_MaterialEditor, vrParallaxStrength);
                EditorGUI.indentLevel--;
            }
        }

        public static void MatCapTextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            if(!CheckPropertyToDraw(guiContent) && !CheckPropertyToDraw(blendUV1, zRotCancel, perspective, vrParallaxStrength)) return;
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                UVSettingGUI(m_MaterialEditor, textureName);
                LocalizedProperty(m_MaterialEditor, blendUV1);
                LocalizedProperty(m_MaterialEditor, zRotCancel);
                LocalizedProperty(m_MaterialEditor, perspective);
                LocalizedProperty(m_MaterialEditor, vrParallaxStrength);

                var buttons = Buttons("UV Preset", "MatCap", "AngelRing");
                if(buttons[0]) ApplyMatCapUVPreset(false, blendUV1, zRotCancel, perspective, vrParallaxStrength);
                if(buttons[1]) ApplyMatCapUVPreset(true, blendUV1, zRotCancel, perspective, vrParallaxStrength);
                EditorGUI.indentLevel--;
            }
        }

        public static void ApplyMatCapUVPreset(bool isAngelRing, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            if(isAngelRing)
            {
                blendUV1.vectorValue = new Vector4(0.0f, 1.0f, 0.0f, 0.0f);
                zRotCancel.floatValue = 1.0f;
                perspective.floatValue = 0.0f;
                vrParallaxStrength.floatValue = 0.0f;
            }
            else
            {
                blendUV1.vectorValue = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
                zRotCancel.floatValue = 1.0f;
                perspective.floatValue = 1.0f;
                vrParallaxStrength.floatValue = 1.0f;
            }
        }

        public static void BitEditor8(MaterialProperty prop, string label)
        {
            int val = (int)prop.floatValue;
            val = IntClamp(val, 0, 255);
            bool[] b = {
                val / 1   % 2 != 0,
                val / 2   % 2 != 0,
                val / 4   % 2 != 0,
                val / 8   % 2 != 0,
                val / 16  % 2 != 0,
                val / 32  % 2 != 0,
                val / 64  % 2 != 0,
                val / 128 % 2 != 0
            };
            EditorGUI.BeginChangeCheck();
            var position = EditorGUILayout.GetControlRect();
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var toggleRect = new Rect(labelRect.x + labelRect.width + 2, position.y, position.height, position.height);
            EditorGUI.PrefixLabel(labelRect, new GUIContent(label));
            b[7] = EditorGUI.Toggle(toggleRect,b[7]); toggleRect.x += position.height;
            b[6] = EditorGUI.Toggle(toggleRect,b[6]); toggleRect.x += position.height;
            b[5] = EditorGUI.Toggle(toggleRect,b[5]); toggleRect.x += position.height;
            b[4] = EditorGUI.Toggle(toggleRect,b[4]); toggleRect.x += position.height;
            b[3] = EditorGUI.Toggle(toggleRect,b[3]); toggleRect.x += position.height;
            b[2] = EditorGUI.Toggle(toggleRect,b[2]); toggleRect.x += position.height;
            b[1] = EditorGUI.Toggle(toggleRect,b[1]); toggleRect.x += position.height;
            b[0] = EditorGUI.Toggle(toggleRect,b[0]); toggleRect.x += position.height;
            if(EditorGUI.EndChangeCheck())
            {
                val = 0;
                if(b[0]) val += 1;
                if(b[1]) val += 2;
                if(b[2]) val += 4;
                if(b[3]) val += 8;
                if(b[4]) val += 16;
                if(b[5]) val += 32;
                if(b[6]) val += 64;
                if(b[7]) val += 128;
                prop.floatValue = val;
            }

            EditorGUI.BeginChangeCheck();
            var numRect = new Rect(toggleRect.x, position.y, position.width - toggleRect.x + position.height + 2, position.height);
            val = EditorGUI.IntField(numRect, val);
            if(EditorGUI.EndChangeCheck())
            {
                val = IntClamp(val, 0, 255);
                prop.floatValue = val;
            }
        }

        public static void ConvertGifToAtlas(MaterialProperty tex, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty isDecal)
        {
            #if SYSTEM_DRAWING
                if(tex.textureValue != null && AssetDatabase.GetAssetPath(tex.textureValue).EndsWith(".gif", StringComparison.OrdinalIgnoreCase) && Button(GetLoc("sConvertGif")))
                {
                    int frameCount, loopXY, duration;
                    float xScale, yScale;
                    string savePath = lilTextureUtils.ConvertGifToAtlas(tex.textureValue, out frameCount, out loopXY, out duration, out xScale, out yScale);

                    tex.textureValue = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath);
                    decalAnimation.vectorValue = new Vector4(loopXY,loopXY,frameCount,100.0f/duration);
                    decalSubParam.vectorValue = new Vector4(xScale,yScale,decalSubParam.vectorValue.z,decalSubParam.vectorValue.w);
                    isDecal.floatValue = 1.0f;
                }
            #endif
        }

        public static void RenderQueueField(MaterialEditor materialEditor)
        {
            if(!CheckPropertyToDraw("Render Queue")) return;
            materialEditor.RenderQueueField();
        }

        public static void EnableInstancingField(MaterialEditor materialEditor)
        {
            if(!CheckPropertyToDraw("Enable GPU Instancing")) return;
            materialEditor.EnableInstancingField();
        }

        public static void DoubleSidedGIField(MaterialEditor materialEditor)
        {
            if(!CheckPropertyToDraw("Double Sided Global Illumination")) return;
            materialEditor.DoubleSidedGIField();
        }

        public static void LightmapEmissionFlagsProperty(MaterialEditor materialEditor)
        {
            if(!CheckPropertyToDraw("Global Illumination")) return;
            #if UNITY_2019_1_OR_NEWER
                materialEditor.LightmapEmissionFlagsProperty(0, true, true);
            #else
                materialEditor.LightmapEmissionFlagsProperty(0, true);
            #endif
        }

        private static int IntClamp(int val, int min, int max)
        {
            return val < min ? min : (val > max ? max : val);
        }

        public static float GetRemapMinValue(float scale, float offset)
        {
            return RoundFloat1000000(Mathf.Clamp(-offset / scale, -0.01f, 1.01f));
        }

        public static float GetRemapMaxValue(float scale, float offset)
        {
            return RoundFloat1000000(Mathf.Clamp((1.0f - offset) / scale, -0.01f, 1.01f));
        }

        public static float GetRemapScaleValue(float min, float max)
        {
            return 1.0f / (max - min);
        }

        public static float GetRemapOffsetValue(float min, float max)
        {
            return min / (min - max);
        }

        public static float Radian2Degree(float val)
        {
            return RoundFloat1000000(val / Mathf.PI * 180.0f);
        }

        public static float Degree2Radian(float val)
        {
            return val * Mathf.PI / 180.0f;
        }

        public static float RoundFloat1000000(float val)
        {
            return Mathf.Floor(val * 1000000.0f + 0.5f) * 0.000001f;
        }

        public static bool EditorButton(string label)
        {
            return GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), label);
        }
        #endregion

        private static string GetLoc(string value) { return lilLanguageManager.GetLoc(value); }
    }
}
#endif