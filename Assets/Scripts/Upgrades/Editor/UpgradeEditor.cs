using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Upgrade)), CanEditMultipleObjects]
public class UpgradeEditor : Editor
{
    Upgrade upgrade;

    private void OnEnable()
    {
        upgrade = target as Upgrade;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Compute"))
        {
            EditorUtility.SetDirty(upgrade);
            for (int i = 0; i < upgrade.lvl.Length; i++)
            {
                string formula = upgrade.valueFormula.Replace("n", i.ToString());
                if (ExpressionEvaluator.Evaluate(formula, out float value)) upgrade.lvl[i].value = value;
            }
            EditorUtility.SetDirty(upgrade);
            for (int i = 0; i < upgrade.lvl.Length; i++)
            {
                string formula = upgrade.priceFormula.Replace("n", i.ToString());
                if (ExpressionEvaluator.Evaluate(formula, out float value)) upgrade.lvl[i].price = Mathf.RoundToInt(value);
            }
        }
        GUILayout.Label("a+b, a-b, a*b, a/b\na^b, a%b\nsqrt(a), floor(a), ceil(a), round(a)\ncos(a), sin(a), tan(a), pi");
    }
}