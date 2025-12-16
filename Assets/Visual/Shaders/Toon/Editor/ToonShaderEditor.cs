using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ToonShaderEditor : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        List<MaterialProperty> propList = new(properties);
        int index = propList.FindIndex(x => x.name == "_RampType");
        if (propList[index].floatValue == 0)
        {
            propList.RemoveAt(propList.FindIndex(x => x.name == "_RampThreshold"));
            propList.RemoveAt(propList.FindIndex(x => x.name == "_RampSmoothing"));
        } else propList.RemoveAt(propList.FindIndex(x => x.name == "_Ramp"));

        index = propList.FindIndex(x => x.name == "_Specular");
        if (propList[index].floatValue == 0)
        {
            propList.RemoveAt(propList.FindIndex(x => x.name == "_SpecularIntensity"));
            propList.RemoveAt(propList.FindIndex(x => x.name == "_SpecularSmooth"));
            propList.RemoveAt(propList.FindIndex(x => x.name == "_SpecularColor"));
        }

        index = propList.FindIndex(x => x.name == "_Highlight");
        if (propList[index].floatValue == 0)
        {
            propList.RemoveAt(propList.FindIndex(x => x.name == "_HighlightScale"));
            propList.RemoveAt(propList.FindIndex(x => x.name == "_HighlightColor"));
        }

        index = propList.FindIndex(x => x.name == "_Emission");
        if (propList[index].floatValue == 0)
        {
            propList.RemoveAt(propList.FindIndex(x => x.name == "_EmissionColor"));
        }

        base.OnGUI(materialEditor, propList.ToArray());
    }
}
