#if UNITY_EDITOR
using UnityEditor;

namespace lilToon
{
    public class lilEditorParameters : ScriptableSingleton<lilEditorParameters>
    {
        public bool forceOptimize;
        public bool startupEnd;
        public string modifiedShaders;
        public string versionInfo;
    }
}
#endif