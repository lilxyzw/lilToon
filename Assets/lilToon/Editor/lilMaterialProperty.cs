#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace lilToon
{
    internal class lilMaterialProperty
    {
        public MaterialProperty p;
        public HashSet<PropertyBlock> blocks;
        public string propertyName;
        public bool isTexture;

        public float floatValue
        {
            get { return p.floatValue; }
            set { p.floatValue = value; }
        }

        public Vector4 vectorValue
        {
            get { return p.vectorValue; }
            set { p.vectorValue = value; }
        }

        public Color colorValue
        {
            get { return p.colorValue; }
            set { p.colorValue = value; }
        }

        public Texture textureValue
        {
            get { return p.textureValue; }
            set { p.textureValue = value; }
        }

        // Other
        public string name
        {
            get { return p.name; }
            private set { }
        }

        public string displayName
        {
            get { return p.displayName; }
            private set { }
        }

        public bool hasMixedValue
        {
            get { return p.hasMixedValue; }
            private set { }
        }

        public Vector2 rangeLimits
        {
            get { return p.rangeLimits; }
            private set { }
        }

        public Object[] targets
        {
            get { return p.targets; }
            private set { }
        }

        public TextureDimension textureDimension
        {
            get { return p.textureDimension; }
            private set { }
        }

        public ShaderPropertyType propertyType
        {
#if UNITY_6000_0_OR_NEWER
            get { return p.propertyType; }
#else
            get
            {
                return p.type switch
                {
                    MaterialProperty.PropType.Color => ShaderPropertyType.Color,
                    MaterialProperty.PropType.Vector => ShaderPropertyType.Vector,
                    MaterialProperty.PropType.Float => ShaderPropertyType.Float,
                    MaterialProperty.PropType.Range => ShaderPropertyType.Range,
                    MaterialProperty.PropType.Texture => ShaderPropertyType.Texture,
                    MaterialProperty.PropType.Int => ShaderPropertyType.Int,
                    _ => ShaderPropertyType.Float,
                };
            }
#endif
            private set { }
        }

        public void FindProperty(MaterialProperty[] props)
        {
            p = props.FirstOrDefault(prop => prop != null && prop.name == propertyName);
        }

        public lilMaterialProperty()
        {
            p = null;
            blocks = new HashSet<PropertyBlock>();
            isTexture = false;
            propertyName = null;
        }

        public lilMaterialProperty(string name, params PropertyBlock[] inBrocks)
        {
            p = null;
            blocks = new HashSet<PropertyBlock>(inBrocks);
            isTexture = false;
            propertyName = name;
        }

        public lilMaterialProperty(string name, bool isTex, params PropertyBlock[] inBrocks)
        {
            p = null;
            blocks = new HashSet<PropertyBlock>(inBrocks);
            isTexture = isTex;
            propertyName = name;
        }

        public lilMaterialProperty(MaterialProperty prop)
        {
            p = prop;
        }

        public static implicit operator MaterialProperty(lilMaterialProperty prop)
        {
            return prop.p;
        }
    }
}
#endif
