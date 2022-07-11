#if UNITY_EDITOR
using System;
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
            EditorGUI.DrawRect(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, 1)), lilConstants.lineColor);
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

        //------------------------------------------------------------------------------------------------------------------------------
        // Property drawer
        #region
        public static void UV4Decal(MaterialEditor m_MaterialEditor, MaterialProperty isDecal, MaterialProperty isLeftOnly, MaterialProperty isRightOnly, MaterialProperty shouldCopy, MaterialProperty shouldFlipMirror, MaterialProperty shouldFlipCopy, MaterialProperty tex, MaterialProperty angle, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty uvMode)
        {
            m_MaterialEditor.ShaderProperty(uvMode, "UV Mode|UV0|UV1|UV2|UV3|MatCap");
            ConvertGifToAtlas(tex, decalAnimation, decalSubParam, isDecal);
            // Toggle decal
            EditorGUI.BeginChangeCheck();
            m_MaterialEditor.ShaderProperty(isDecal, GetLoc("sAsDecal"));
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
                mirrorMode = EditorGUILayout.Popup(GetLoc("sMirrorMode"),mirrorMode,new string[]{GetLoc("sMirrorModeNormal"),GetLoc("sMirrorModeFlip"),GetLoc("sMirrorModeLeft"),GetLoc("sMirrorModeRight"),GetLoc("sMirrorModeRightFlip")});
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
                copyMode = EditorGUILayout.Popup(GetLoc("sCopyMode"),copyMode,new string[]{GetLoc("sCopyModeNormal"),GetLoc("sCopyModeSymmetry"),GetLoc("sCopyModeFlip")});
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
                    posX = EditorGUILayout.Slider(GetLoc("sPositionX"), posX, 0.5f, 1.0f);
                }
                else
                {
                    posX = EditorGUILayout.Slider(GetLoc("sPositionX"), posX, 0.0f, 1.0f);
                }

                // Draw properties
                posY = EditorGUILayout.Slider(GetLoc("sPositionY"), posY, 0.0f, 1.0f);
                scaleX = EditorGUILayout.Slider(GetLoc("sScaleX"), scaleX, -1.0f, 1.0f);
                scaleY = EditorGUILayout.Slider(GetLoc("sScaleY"), scaleY, -1.0f, 1.0f);
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

                m_MaterialEditor.ShaderProperty(angle, GetLoc("sAngle"));
                EditorGUI.indentLevel--;
                m_MaterialEditor.ShaderProperty(decalAnimation, lilLanguageManager.BuildParams(GetLoc("sAnimation"), GetLoc("sXFrames"), GetLoc("sYFrames"), GetLoc("sFrames"), GetLoc("sFPS")));
                m_MaterialEditor.ShaderProperty(decalSubParam, lilLanguageManager.BuildParams(GetLoc("sXRatio"), GetLoc("sYRatio"), GetLoc("sFixBorder")));
            }
            else
            {
                m_MaterialEditor.TextureScaleOffsetProperty(tex);
                m_MaterialEditor.ShaderProperty(angle, GetLoc("sAngle"));
            }

            if(EditorButton(GetLoc("sReset")) && EditorUtility.DisplayDialog(GetLoc("sDialogResetUV"),GetLoc("sDialogResetUVMes"),GetLoc("sYes"),GetLoc("sNo")))
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

        public static void ToneCorrectionGUI(MaterialEditor m_MaterialEditor, MaterialProperty hsvg)
        {
            m_MaterialEditor.ShaderProperty(hsvg, lilLanguageManager.BuildParams(GetLoc("sHue"), GetLoc("sSaturation"), GetLoc("sValue"), GetLoc("sGamma")));
            // Reset
            if(EditorButton(GetLoc("sReset")))
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
            float param1 = prop.vectorValue.x;
            float param2 = prop.vectorValue.y;
            float param3 = prop.vectorValue.z;
            float param4 = prop.vectorValue.w;

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
            float alpha = prop.colorValue.a;

            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            alpha = EditorGUILayout.Slider(label, alpha, 0.0f, 1.0f);
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
            if(EditorButton(GetLoc("sRemoveUnused")))
            {
                Undo.RecordObject(material, "Remove unused properties");
                lilMaterialUtils.RemoveUnusedTexture(material, material.shader.name.Contains("Lite"));
            }
        }

        public static void SetAlphaIsTransparencyGUI(MaterialProperty tex)
        {
            if(tex.textureValue != null && !((Texture2D)tex.textureValue).alphaIsTransparency && AutoFixHelpBox(GetLoc("sNotAlphaIsTransparency")))
            {
                string path = AssetDatabase.GetAssetPath(tex.textureValue);
                TextureImporter textureImporter = (TextureImporter)AssetImporter.GetAtPath(path);
                textureImporter.alphaIsTransparency = true;
                AssetDatabase.ImportAsset(path);
            }
        }

        public static void UVSettingGUI(MaterialEditor m_MaterialEditor, MaterialProperty uvst, MaterialProperty uvsr)
        {
            EditorGUILayout.LabelField(GetLoc("sUVSetting"), boldLabel);
            m_MaterialEditor.TextureScaleOffsetProperty(uvst);
            m_MaterialEditor.ShaderProperty(uvsr, lilLanguageManager.BuildParams(GetLoc("sAngle"), GetLoc("sUVAnimation"), GetLoc("sScroll"), GetLoc("sRotate")));
        }

        public static void InvBorderGUI(MaterialProperty prop)
        {
            float f = prop.floatValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            f = 1.0f - EditorGUILayout.Slider(GetLoc("sBorder"), 1.0f - f, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = f;
            }
        }

        public static void MinusRangeGUI(MaterialProperty prop, string label)
        {
            float f = -prop.floatValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = prop.hasMixedValue;
            f = EditorGUILayout.Slider(label, f, 0.0f, 1.0f);
            EditorGUI.showMixedValue = false;
            if(EditorGUI.EndChangeCheck())
            {
                prop.floatValue = -f;
            }
        }

        public static void BlendSettingGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, string labelName, MaterialProperty srcRGB, MaterialProperty dstRGB, MaterialProperty srcA, MaterialProperty dstA, MaterialProperty opRGB, MaterialProperty opA)
        {
            isShow = DrawSimpleFoldout(labelName, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.ShaderProperty(srcRGB, GetLoc("sSrcBlendRGB"));
                m_MaterialEditor.ShaderProperty(dstRGB, GetLoc("sDstBlendRGB"));
                m_MaterialEditor.ShaderProperty(srcA, GetLoc("sSrcBlendAlpha"));
                m_MaterialEditor.ShaderProperty(dstA, GetLoc("sDstBlendAlpha"));
                m_MaterialEditor.ShaderProperty(opRGB, GetLoc("sBlendOpRGB"));
                m_MaterialEditor.ShaderProperty(opA, GetLoc("sBlendOpAlpha"));
                EditorGUI.indentLevel--;
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName)
        {
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                EditorGUI.indentLevel--;
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba)
        {
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                EditorGUI.indentLevel--;
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty uvMode, string sUVMode)
        {
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                m_MaterialEditor.ShaderProperty(uvMode, sUVMode);
                EditorGUI.indentLevel--;
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate)
        {
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                m_MaterialEditor.ShaderProperty(scrollRotate, GetLoc("sScroll"));
                EditorGUI.indentLevel--;
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, bool useCustomUV, bool useUVAnimation)
        {
            if(useCustomUV)
            {
                isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
                if(isShow)
                {
                    EditorGUI.indentLevel++;
                    if(useUVAnimation)  UVSettingGUI(m_MaterialEditor, textureName, scrollRotate);
                    else                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            }
        }

        public static void TextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty scrollRotate, MaterialProperty uvMode, bool useCustomUV, bool useUVAnimation)
        {
            if(useCustomUV)
            {
                isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
                if(isShow)
                {
                    EditorGUI.indentLevel++;
                    if(useUVAnimation)  UVSettingGUI(m_MaterialEditor, textureName, scrollRotate);
                    else                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                    m_MaterialEditor.ShaderProperty(uvMode, "UV Mode|UV0|UV1|UV2|UV3|Rim");
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                m_MaterialEditor.TexturePropertySingleLine(guiContent, textureName, rgba);
            }
        }

        public static void MatCapTextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                m_MaterialEditor.ShaderProperty(blendUV1, GetLoc("sBlendUV1"));
                m_MaterialEditor.ShaderProperty(zRotCancel, GetLoc("sMatCapZRotCancel"));
                m_MaterialEditor.ShaderProperty(perspective, GetLoc("sFixPerspective"));
                m_MaterialEditor.ShaderProperty(vrParallaxStrength, GetLoc("sVRParallaxStrength"));
                EditorGUI.indentLevel--;
            }
        }

        public static void MatCapTextureGUI(MaterialEditor m_MaterialEditor, bool isCustomEditor, ref bool isShow, GUIContent guiContent, MaterialProperty textureName, MaterialProperty rgba, MaterialProperty blendUV1, MaterialProperty zRotCancel, MaterialProperty perspective, MaterialProperty vrParallaxStrength)
        {
            isShow = DrawSimpleFoldout(m_MaterialEditor, guiContent, textureName, rgba, isShow, isCustomEditor);
            if(isShow)
            {
                EditorGUI.indentLevel++;
                m_MaterialEditor.TextureScaleOffsetProperty(textureName);
                m_MaterialEditor.ShaderProperty(blendUV1, GetLoc("sBlendUV1"));
                m_MaterialEditor.ShaderProperty(zRotCancel, GetLoc("sMatCapZRotCancel"));
                m_MaterialEditor.ShaderProperty(perspective, GetLoc("sFixPerspective"));
                m_MaterialEditor.ShaderProperty(vrParallaxStrength, GetLoc("sVRParallaxStrength"));

                GUILayout.BeginHorizontal();
                Rect position2 = EditorGUILayout.GetControlRect();
                Rect labelRect = new Rect(position2.x, position2.y, EditorGUIUtility.labelWidth, position2.height);
                Rect buttonRect1 = new Rect(labelRect.x + labelRect.width, position2.y, (position2.width - EditorGUIUtility.labelWidth)*0.5f, position2.height);
                Rect buttonRect2 = new Rect(buttonRect1.x + buttonRect1.width, position2.y, buttonRect1.width, position2.height);
                EditorGUI.PrefixLabel(labelRect, new GUIContent("UV Preset"));
                if(GUI.Button(buttonRect1, new GUIContent("MatCap"))) ApplyMatCapUVPreset(false, blendUV1, zRotCancel, perspective, vrParallaxStrength);
                if(GUI.Button(buttonRect2, new GUIContent("AngelRing"))) ApplyMatCapUVPreset(true, blendUV1, zRotCancel, perspective, vrParallaxStrength);
                GUILayout.EndHorizontal();
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
            Rect position = EditorGUILayout.GetControlRect();
            Rect labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            Rect toggleRect = new Rect(labelRect.x + labelRect.width + 2, position.y, position.height, position.height);
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
            Rect numRect = new Rect(toggleRect.x, position.y, position.width - toggleRect.x + position.height + 2, position.height);
            val = EditorGUI.IntField(numRect, val);
            if(EditorGUI.EndChangeCheck())
            {
                val = IntClamp(val, 0, 255);
                prop.floatValue = val;
            }
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

        public static void ConvertGifToAtlas(MaterialProperty tex, MaterialProperty decalAnimation, MaterialProperty decalSubParam, MaterialProperty isDecal)
        {
            #if SYSTEM_DRAWING
                if(tex.textureValue != null && AssetDatabase.GetAssetPath(tex.textureValue).EndsWith(".gif", StringComparison.OrdinalIgnoreCase) && EditorButton(GetLoc("sConvertGif")))
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

        public static bool EditorButton(string label)
        {
            return GUI.Button(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect()), label);
        }
        #endregion

        private static string GetLoc(string value) { return lilLanguageManager.GetLoc(value); }
    }
}
#endif