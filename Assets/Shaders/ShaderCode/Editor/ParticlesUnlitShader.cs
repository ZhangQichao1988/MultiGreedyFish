using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Rendering.Universal.ShaderGUI.Custom
{
    

    internal class ParticlesUnlitShader : BaseShaderGUI
    {
        enum ZTestType 
        {
            Off = 0,
            None = 1,
            On = 2,
        };    

        public static readonly GUIContent zTest = new GUIContent("ZTest", "ZTest.");
        public static readonly GUIContent baseMapTiling = new GUIContent("BaseMap Tiling",  "BaseMap Tiling.");
        public static readonly GUIContent alphaMap = new GUIContent("AlphaMap", "AlphaMap.");

        public static readonly GUIContent dissolveMap = new GUIContent("DissolveMap", "DissolveMap.");
        public static readonly GUIContent dissColor = new GUIContent("DissColor", "DissColor.");
        public static readonly GUIContent dissAddColor = new GUIContent("DissAddColor", "DissAddColor.");
        public static readonly GUIContent dissSize = new GUIContent("DissSize", "dissSize.");
        public static readonly GUIContent dissProcess = new GUIContent("DissProcess", "DissProcess.");

        public static readonly GUIContent noiseMap = new GUIContent("NoiseMap", "NoiseMap.");

        protected MaterialProperty zTestProp { get; set; }
        protected MaterialProperty alphaMapProp { get; set; }
        protected MaterialProperty dissolveMapProp { get; set; }
        protected MaterialProperty dissSizeProp { get; set; }
        protected MaterialProperty dissColorProp { get; set; }
        protected MaterialProperty dissAddColorProp { get; set; }
        protected MaterialProperty dissProcessProp { get; set; }

        protected MaterialProperty noiseMapProp { get; set; }
        protected MaterialProperty noisePowerProp { get; set; }


        // Properties
        private BakedLitGUI.BakedLitProperties shadingModelProperties;
        private ParticleGUI.ParticleProperties particleProps;

        // List of renderers using this material in the scene, used for validating vertex streams
        List<ParticleSystemRenderer> m_RenderersUsingThisMaterial = new List<ParticleSystemRenderer>();

        public override void DrawBaseProperties(Material material)
        {
            Vector4 scrollUV4;
            Vector2 scrollUV;
            if (baseMapProp != null && baseColorProp != null) // Draw the baseMap, most shader will have at least a baseMap
            {
                materialEditor.TexturePropertySingleLine(Styles.baseMap, baseMapProp, baseColorProp);
                ++EditorGUI.indentLevel;
                materialEditor.TextureScaleOffsetProperty(baseMapProp);
                float baseColorPower = material.GetFloat("_BaseColorPower");
                baseColorPower = EditorGUILayout.FloatField("baseColorPower", baseColorPower);
                material.SetFloat("_BaseColorPower", baseColorPower);
                
                scrollUV4 = material.GetVector("_BaseMapUVScroll");
                scrollUV = new Vector2(scrollUV4.x, scrollUV4.y);
                scrollUV = EditorGUILayout.Vector2Field("BaseMapUVScroll", scrollUV);
                material.SetVector("_BaseMapUVScroll", new Vector4(scrollUV.x, scrollUV.y, 0, 0));
                --EditorGUI.indentLevel;
            }
            if (alphaMapProp != null) 
            {
                materialEditor.TexturePropertySingleLine(alphaMap, alphaMapProp);
                var hasTexture = alphaMapProp.textureValue != null;
                EditorGUI.BeginDisabledGroup(!hasTexture);
                {
                    ++EditorGUI.indentLevel;
                    materialEditor.TextureScaleOffsetProperty(alphaMapProp);
                    scrollUV4 = material.GetVector("_AlphaMapUVScroll");
                    scrollUV = new Vector2(scrollUV4.x, scrollUV4.y);
                    scrollUV = EditorGUILayout.Vector2Field("AlphaMapUVScroll", scrollUV);
                    material.SetVector("_AlphaMapUVScroll", new Vector4(scrollUV.x, scrollUV.y, 0, 0));
                    --EditorGUI.indentLevel;
                }
                EditorGUI.EndDisabledGroup();
            }
        }

        public override void FindProperties(MaterialProperty[] properties)
        {
            base.FindProperties(properties);
            zTestProp = FindProperty("_ZTest", properties);
            alphaMapProp = FindProperty("_AlphaMap", properties, false);

            dissolveMapProp = FindProperty("_DissolveMap", properties, false);
            dissSizeProp = FindProperty("_DissSize", properties);
            dissColorProp = FindProperty("_DissColor", properties);
            dissAddColorProp = FindProperty("_DissAddColor", properties);
            dissProcessProp = FindProperty("_DissProcess", properties);

            noiseMapProp = FindProperty("_NoiseMap", properties, false);
            noisePowerProp = FindProperty("_NoisePower", properties, false);

            shadingModelProperties = new BakedLitGUI.BakedLitProperties(properties);
            particleProps = new ParticleGUI.ParticleProperties(properties);
        }

        public override void MaterialChanged(Material material)
        {
            if (material == null)
                throw new ArgumentNullException("material");

            SetMaterialKeywords(material, null, ParticleGUI.SetMaterialKeywords);

            if (material.HasProperty("_AlphaMap"))
                CoreUtils.SetKeyword(material, "_ALPHAMAP", material.GetTexture("_AlphaMap"));

            if (material.HasProperty("_DissolveMap"))
                CoreUtils.SetKeyword(material, "_DISSOLVE", material.GetTexture("_DissolveMap"));

            if (material.HasProperty("_NoiseMap"))
                CoreUtils.SetKeyword(material, "_NOISE", material.GetTexture("_NoiseMap"));

        }

        public override void DrawSurfaceOptions(Material material)
        {
            // Detect any changes to the material
            EditorGUI.BeginChangeCheck();
            {
                base.DrawSurfaceOptions(material);

                // ZTest
                var mode = zTestProp.floatValue == 0 ? 0 : 1;
                EditorGUI.BeginChangeCheck();
                mode = EditorGUILayout.Popup(zTest, (int)mode, new string[] { "Off", "On" });
                if (EditorGUI.EndChangeCheck())
                {
                    materialEditor.RegisterPropertyChangeUndo(zTest.text);
                    zTestProp.floatValue = mode == 1 ? 2 : 0;
                }

                DoPopup(ParticleGUI.Styles.colorMode, particleProps.colorMode, Enum.GetNames(typeof(ParticleGUI.ColorMode)));
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendModeProp.targets)
                    MaterialChanged((Material)obj);
            }
        }

        public override void DrawSurfaceInputs(Material material)
        {
            base.DrawSurfaceInputs(material);
            BakedLitGUI.Inputs(shadingModelProperties, materialEditor);
            DrawEmissionProperties(material, true);
            DrawDissolveProperties(material);
            DrawNoiseProperties(material);
        }

        public virtual void DrawDissolveProperties(Material material)
        {
            EditorGUI.BeginChangeCheck();
            {
                materialEditor.TexturePropertySingleLine(dissolveMap, dissolveMapProp);
                var hadDissolveTexture = dissolveMapProp.textureValue != null;
                EditorGUI.BeginDisabledGroup(!hadDissolveTexture);
                {
                    materialEditor.ShaderProperty(dissColorProp, dissColor, 1);
                    materialEditor.ShaderProperty(dissAddColorProp, dissAddColor, 1);
                    materialEditor.ShaderProperty(dissSizeProp, dissSize, 1);
                    materialEditor.ShaderProperty(dissProcessProp, dissProcess, 1);
                }
                EditorGUI.EndDisabledGroup();
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendModeProp.targets)
                    MaterialChanged((Material)obj);
            }            
        }

        public virtual void DrawNoiseProperties(Material material)
        {
            EditorGUI.BeginChangeCheck();
            {
                materialEditor.TexturePropertySingleLine(noiseMap, noiseMapProp);
                var hasTexture = noiseMapProp.textureValue != null;
                EditorGUI.BeginDisabledGroup(!hasTexture);
                {
                    ++EditorGUI.indentLevel;

                    materialEditor.TextureScaleOffsetProperty(noiseMapProp);
                    var scrollUV4 = material.GetVector("_NoiseMapUVScroll");
                    var scrollUV = new Vector2(scrollUV4.x, scrollUV4.y);
                    scrollUV = EditorGUILayout.Vector2Field("NoiseMapUVScroll", scrollUV);
                    material.SetVector("_NoiseMapUVScroll", new Vector4(scrollUV.x, scrollUV.y, 0, 0));

                    materialEditor.ShaderProperty(noisePowerProp, "NoisePower");
                    --EditorGUI.indentLevel;
                }
                EditorGUI.EndDisabledGroup();
            }
            if (EditorGUI.EndChangeCheck())
            {
                foreach (var obj in blendModeProp.targets)
                    MaterialChanged((Material)obj);
            }
        }

        public override void DrawAdvancedOptions(Material material)
        {
            EditorGUI.BeginChangeCheck();
            {
                materialEditor.ShaderProperty(particleProps.flipbookMode, ParticleGUI.Styles.flipbookMode);
                ParticleGUI.FadingOptions(material, materialEditor, particleProps);
                ParticleGUI.DoVertexStreamsArea(material, m_RenderersUsingThisMaterial);
            }
            base.DrawAdvancedOptions(material);
        }

        public override void OnOpenGUI(Material material, MaterialEditor materialEditor)
        {
            CacheRenderersUsingThisMaterial(material);
            base.OnOpenGUI(material, materialEditor);
        }

        void CacheRenderersUsingThisMaterial(Material material)
        {
            m_RenderersUsingThisMaterial.Clear();

            ParticleSystemRenderer[] renderers = UnityEngine.Object.FindObjectsOfType(typeof(ParticleSystemRenderer)) as ParticleSystemRenderer[];
            foreach (ParticleSystemRenderer renderer in renderers)
            {
                if (renderer.sharedMaterial == material)
                    m_RenderersUsingThisMaterial.Add(renderer);
            }
        }
    }
} // namespace UnityEditor
