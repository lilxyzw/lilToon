#if UNITY_EDITOR && CVR_CCK_EXISTS
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using ABI.CCK.Components;
using ABI.CCK.Scripts.Editor;

namespace lilToon
{
    //------------------------------------------------------------------------------------------------------------------------------
    // ChilloutVR
    public static class ChilloutVRModule
    {
        [InitializeOnLoadMethod]
        public static void StartupMethod()
        {
            CCK_BuildUtility.PreAvatarBundleEvent.AddListener(OnBuildRequested);
            CCK_BuildUtility.PrePropBundleEvent.AddListener(OnBuildRequested);
        }

        public static void OnBuildRequested(GameObject avatarGameObject)
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
                Debug.Log("[lilToon] OnBuildRequested() failed");
            }

            EditorApplication.delayCall += () =>
            {
                SetShaderSettingAfterBuild();
            };
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

            foreach(var descriptor in gameObject.GetComponentsInChildren<CVRAvatar>(true))
            {
                if(descriptor.overrides != null)
                {
                    clips.AddRange(descriptor.overrides.animationClips);
                }
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
    }
}
#endif