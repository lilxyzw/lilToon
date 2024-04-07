#if UNITY_EDITOR && CVR_CCK_EXISTS
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Events;

namespace lilToon.External
{
    //------------------------------------------------------------------------------------------------------------------------------
    // ChilloutVR
    public static class ChilloutVRModule
    {
        [InitializeOnLoadMethod]
        public static void StartupMethod()
        {
            try
            {
                var type = Assembly.Load("Assembly-CSharp-Editor").GetType("ABI.CCK.Scripts.Editor.CCK_BuildUtility");
                var preAvatarBundleEvent = type.GetField("PreAvatarBundleEvent", BindingFlags.Static | BindingFlags.Public);
                var prePropBundleEvent = type.GetField("PrePropBundleEvent", BindingFlags.Static | BindingFlags.Public);
                var method = typeof(UnityEvent<GameObject>).GetMethod("AddListener");
                var methodOnBuild = typeof(ChilloutVRModule).GetMethod("OnBuildRequested", BindingFlags.Static | BindingFlags.Public);
                var m = (UnityAction<GameObject>)Delegate.CreateDelegate(typeof(UnityAction<GameObject>), null, methodOnBuild);
                method.Invoke(preAvatarBundleEvent.GetValue(null), new object[]{m});
                method.Invoke(prePropBundleEvent.GetValue(null), new object[]{m});
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
        }

        public static void OnBuildRequested(GameObject avatarGameObject)
        {
            try
            {
                var materials = GetMaterialsFromGameObject(avatarGameObject);
                var clips = GetAnimationClipsFromGameObject(avatarGameObject);
                SetShaderSettingBeforeBuild(materials, clips);
                lilMaterialUtils.SetupMultiMaterial(materials, clips);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                Debug.Log("[lilToon] OnBuildRequested() failed");
            }

            EditorApplication.delayCall -= SetShaderSettingAfterBuild;
            EditorApplication.delayCall += SetShaderSettingAfterBuild;
        }

        private static Material[] GetMaterialsFromGameObject(GameObject gameObject)
        {
            var materials = new List<Material>();
            foreach(var renderer in gameObject.GetComponentsInChildren<Renderer>(true))
            {
                materials.AddRange(renderer.sharedMaterials);
            }
            return materials.ToArray();
        }

        private static AnimationClip[] GetAnimationClipsFromGameObject(GameObject gameObject)
        {
            var clips = new List<AnimationClip>();

            foreach(var animator in gameObject.GetComponentsInChildren<Animator>(true))
            {
                if(animator.runtimeAnimatorController != null) clips.AddRange(animator.runtimeAnimatorController.animationClips);
            }

            try
            {
                var type = Assembly.Load("Assembly-CSharp").GetType("ABI.CCK.Components.CVRAvatar");
                var overridesField = type.GetField("overrides");
                foreach(var descriptor in gameObject.GetComponentsInChildren(type,true))
                {
                    var overrides = (AnimatorOverrideController)overridesField.GetValue(descriptor);
                    if(overrides != null)
                    {
                        clips.AddRange(overrides.animationClips);
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }

            return clips.ToArray();
        }

        private static void SetShaderSettingBeforeBuild(Material[] materials, AnimationClip[] clips)
        {
            Type type = typeof(lilToonSetting);
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            foreach(var method in methods)
            {
                var methodParams = method.GetParameters();
                if(method.Name != "SetShaderSettingBeforeBuild" || methodParams.Length != 2 || methodParams[0].ParameterType != typeof(Material[])) continue;
                method.Invoke(null, new object[]{materials,clips});
                break;
            }
        }

        private static void SetShaderSettingAfterBuild()
        {
            Type type = typeof(lilToonSetting);
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            foreach(var method in methods)
            {
                var methodParams = method.GetParameters();
                if(method.Name != "SetShaderSettingAfterBuild" || methodParams.Length != 0) continue;
                method.Invoke(null, null);
                break;
            }
        }

        [MenuItem("GameObject/lilToon/[Debug] Generate bug report (CVR Avatar)", false, 23)]
        public static void GenerateBugReportCVRAvatar()
        {
            var clips = new List<AnimationClip>();
            try
            {
                var type = Assembly.Load("Assembly-CSharp").GetType("ABI.CCK.Components.CVRAvatar");
                var overridesField = type.GetField("overrides");
                foreach(var descriptor in Selection.activeGameObject.GetComponentsInChildren(type,true))
                {
                    var overrides = (AnimatorOverrideController)overridesField.GetValue(descriptor);
                    if(overrides != null)
                    {
                        clips.AddRange(overrides.animationClips);
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }

            var methods = typeof(lilToonEditorUtils).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            foreach(var method in methods)
            {
                var methodParams = method.GetParameters();
                if(method.Name != "GenerateBugReport" || methodParams.Length != 3) continue;
                method.Invoke(null, new object[]{null, clips, "# CVR Avatar Debug"});
                return;
            }
            #pragma warning disable 0162
            if(lilConstants.currentVersionValue < 31) EditorUtility.DisplayDialog("[Debug] Generate bug report (CVR Avatar)","This version does not support bug reports. Prease import lilToon 1.3.5 or newer.","OK");
            else                                      EditorUtility.DisplayDialog("[Debug] Generate bug report (CVR Avatar)","Failed to generate bug report.","OK");
            #pragma warning restore 0162
        }

        [MenuItem("GameObject/lilToon/[Debug] Generate bug report (CVR Avatar)", true, 23)]
        public static bool CheckGenerateBugReportCVRAvatar()
        {
            if(Selection.activeGameObject == null) return false;
            try
            {
                var type = Assembly.Load("Assembly-CSharp").GetType("ABI.CCK.Components.CVRAvatar");
                return Selection.activeGameObject.GetComponent(type) != null;
            }
            catch(Exception e)
            {
                Debug.LogException(e);
            }
            return false;
        }
    }
}
#endif