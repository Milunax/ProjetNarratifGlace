using UnityEngine;

public class AlphaCtrl : MonoBehaviour
{
    // 指定需要控制透明度的 URP Lit 材质（在 Inspector 中赋值）
    public Material targetMaterial;

    // 透明度控制范围：0（完全透明）到 1（完全不透明）
    [Range(0f, 1f)]
    public float alpha = 1f;

    void Update()
    {
        if (targetMaterial != null && targetMaterial.HasProperty("_BaseColor"))
        {
            // 获取 URP Lit 材质的 Base Color
            Color baseColor = targetMaterial.GetColor("_BaseColor");
            // 修改 alpha 值
            baseColor.a = alpha;
            // 将修改后的颜色赋值回材质
            targetMaterial.SetColor("_BaseColor", baseColor);
        }
    }
}
