#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;

namespace lilToon
{
    public class lilShaderAPI
    {
        public static bool IsTextureLimitedAPI()
        {
            return CurrentAPITextureLimit() <= 32;
        }

        public static int CurrentAPITextureLimit()
        {
            switch(SystemInfo.graphicsDeviceType)
            {
                case GraphicsDeviceType.OpenGLES2:
                    return 32;
                case GraphicsDeviceType.OpenGLES3:
                    return 32;
                case GraphicsDeviceType.OpenGLCore:
                    return 32;
                default :
                    return 128;
            }
        }
    }
}
#endif