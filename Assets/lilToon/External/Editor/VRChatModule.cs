#if UNITY_EDITOR && VRC_SDK_VRCSDK3
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using VRC.SDKBase.Editor.BuildPipeline;

namespace lilToon.External
{
    //------------------------------------------------------------------------------------------------------------------------------
    // VRChat
    public class VRChatModule : IVRCSDKBuildRequestedCallback, IVRCSDKPreprocessAvatarCallback, IVRCSDKPostprocessAvatarCallback
    {
        public int callbackOrder { get { return 100; } }

        #if UDON
            public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
            {
                try
                {
                    SetShaderSettingBeforeBuild();
                    EditorApplication.delayCall -= SetShaderSettingAfterBuild;
                    EditorApplication.delayCall += SetShaderSettingAfterBuild;
                }
                catch(Exception e)
                {
                    Debug.LogException(e);
                    Debug.Log("[lilToon] OnBuildRequested() failed");
                }
                return true;
            }
        #else
            public bool OnBuildRequested(VRCSDKRequestedBuildType requestedBuildType)
            {
                ForceOptimization();
                return true;
            }
        #endif

        public bool OnPreprocessAvatar(GameObject avatarGameObject)
        {
            try
            {
                var materials = GetMaterialsFromGameObject(avatarGameObject);
                var clips = GetAnimationClipsFromGameObject(avatarGameObject);
                SetShaderSettingBeforeBuild(materials, clips);
            }
            catch(Exception e)
            {
                Debug.LogException(e);
                Debug.Log("[lilToon] OnPreprocessAvatar() failed");
            }
            return true;
        }

        public void OnPostprocessAvatar()
        {
            SetShaderSettingAfterBuild();
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

            #if !UDON
                foreach(var descriptor in gameObject.GetComponentsInChildren<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>(true))
                {
                    foreach(var layer in descriptor.specialAnimationLayers)
                    {
                        if(layer.animatorController != null) clips.AddRange(layer.animatorController.animationClips);
                    }
                    if(descriptor.customizeAnimationLayers)
                    {
                        foreach(var layer in descriptor.baseAnimationLayers)
                        {
                            if(layer.animatorController != null) clips.AddRange(layer.animatorController.animationClips);
                        }
                    }
                }
            #endif

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

        private static void SetShaderSettingBeforeBuild()
        {
            Type type = typeof(lilToonSetting);
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            foreach(var method in methods)
            {
                var methodParams = method.GetParameters();
                if(method.Name != "SetShaderSettingBeforeBuild" || methodParams.Length != 0) continue;
                method.Invoke(null, null);
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

        private static void ForceOptimization()
        {
            Type type = typeof(lilToonSetting);
            var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
            foreach(var method in methods)
            {
                var methodParams = method.GetParameters();
                if(method.Name != "ForceOptimization" || methodParams.Length != 0) continue;
                method.Invoke(null, null);
                break;
            }
        }

        // Debug
        #if !UDON
            [MenuItem("GameObject/lilToon/[Debug] Generate bug report (VRChat Avatar)", false, 23)]
            public static void GenerateBugReportVRChatAvatar()
            {
                var clips = new List<AnimationClip>();
                foreach(var descriptor in Selection.activeGameObject.GetComponentsInChildren<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>(true))
                {
                    foreach(var layer in descriptor.specialAnimationLayers)
                    {
                        if(layer.animatorController != null) clips.AddRange(layer.animatorController.animationClips);
                    }
                    if(descriptor.customizeAnimationLayers)
                    {
                        foreach(var layer in descriptor.baseAnimationLayers)
                        {
                            if(layer.animatorController != null) clips.AddRange(layer.animatorController.animationClips);
                        }
                    }
                }
                Type type = typeof(lilToonEditorUtils);
                var methods = type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic);
                foreach(var method in methods)
                {
                    var methodParams = method.GetParameters();
                    if(method.Name != "GenerateBugReport" || methodParams.Length != 3) continue;
                    method.Invoke(null, new object[]{null, clips, "# VRChat Avatar Debug"});
                    return;
                }
                #pragma warning disable 0162
                if(lilConstants.currentVersionValue < 31) EditorUtility.DisplayDialog("[Debug] Generate bug report (VRChat Avatar)","This version does not support bug reports. Prease import lilToon 1.3.5 or newer.","OK");
                else                                      EditorUtility.DisplayDialog("[Debug] Generate bug report (VRChat Avatar)","Failed to generate bug report.","OK");
                #pragma warning restore 0162
            }

            [MenuItem("GameObject/lilToon/[Debug] Generate bug report (VRChat Avatar)", true, 23)]
            public static bool CheckGenerateBugReportVRChatAvatar()
            {
                return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<VRC.SDK3.Avatars.Components.VRCAvatarDescriptor>() != null;
            }
        #endif
    }
}
#endif