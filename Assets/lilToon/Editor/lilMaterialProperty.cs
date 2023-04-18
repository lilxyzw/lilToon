#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace lilToon
{
    internal class lilMaterialProperty
    {
        public MaterialProperty p;
        public List<PropertyBlock> blocks;
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

        public MaterialProperty.PropFlags flags
        {
            get { return p.flags; }
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

        public UnityEngine.Rendering.TextureDimension textureDimension
        {
            get { return p.textureDimension; }
            private set { }
        }

        public MaterialProperty.PropType type
        {
            get { return p.type; }
            private set { }
        }

        public void FindProperty(MaterialProperty[] props)
        {
            p = props.FirstOrDefault(prop => prop != null && prop.name == propertyName);
        }

        public lilMaterialProperty()
        {
            p = null;
            blocks = new List<PropertyBlock>();
            isTexture = false;
            propertyName = null;
        }

        public lilMaterialProperty(string name, params PropertyBlock[] inBrocks)
        {
            p = null;
            blocks = inBrocks.ToList();
            isTexture = false;
            propertyName = name;
        }

        public lilMaterialProperty(string name, bool isTex, params PropertyBlock[] inBrocks)
        {
            p = null;
            blocks = inBrocks.ToList();
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