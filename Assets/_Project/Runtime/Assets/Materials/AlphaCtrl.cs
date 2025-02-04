using UnityEngine;

public class AlphaCtrl : MonoBehaviour
{
    // 指定需要控制透明度的材质（在 Inspector 中赋值）
    public Material targetMaterial;

    // 透明度控制范围：0（完全透明）到 1（完全不透明）
    [Range(0f, 1f)]
    public float alpha = 1f;

    void Start()
    {
        // 如果使用的是 Standard Shader 或其他需要设置渲染模式的 Shader，则先设置为透明模式
        if (targetMaterial != null)
        {
            SetupMaterialWithTransparency(targetMaterial);
        }
    }

    void Update()
    {
        if (targetMaterial != null)
        {
            // 通过修改材质的颜色的 alpha 值来控制透明度
            Color currentColor = targetMaterial.color;
            currentColor.a = alpha;
            targetMaterial.color = currentColor;
        }
    }

    /// <summary>
    /// 设置材质为透明模式（针对 Unity Standard Shader）
    /// </summary>
    /// <param name="mat">要设置的材质</param>
    void SetupMaterialWithTransparency(Material mat)
    {
        // 设置渲染模式为 Transparent
        // 这里假设材质使用的是 Unity 内置 Standard Shader
        mat.SetFloat("_Mode", 3); // 3 表示透明模式
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        // 将渲染队列设置到透明队列
        mat.renderQueue = 3000;
    }
}
