Shader "Custom/Shadow"
{
    Properties
    {
        // 后处理输入的当前屏幕渲染结果
        _MainTex ("Base (RGB)", 2D) = "white" {}
        // 阴影强度，数值越大阴影效果越明显
        _Intensity ("Intensity", Range(0, 1)) = 0.5
        // 阴影过渡的平滑度，数值越小过渡越尖锐
        _Smoothness ("Smoothness", Range(0.01, 1)) = 0.5
        // 用于阴影效果的贴图，请准备一张适合的贴图（例如黑色带渐变、纹理等）
        _VignetteTex ("Vignette Texture", 2D) = "black" {}
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            // 使用内置图片效果顶点程序
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _VignetteTex;
            float _Intensity;
            float _Smoothness;

            // 片元程序：根据屏幕坐标计算阴影因子，并用贴图中的颜色进行混合
            fixed4 frag(v2f_img i) : SV_Target
            {
                // 采样当前屏幕颜色
                fixed4 color = tex2D(_MainTex, i.uv);
                
                // 计算以屏幕中心 (0.5, 0.5) 为原点的坐标偏移
                float2 centeredUV = i.uv - 0.5;
                // 使用 x 和 y 分量绝对值的最大值，得到“方形”距离
                float squareDist = max(abs(centeredUV.x), abs(centeredUV.y));

                // 使用 smoothstep 生成从中心到边缘的渐变因子：
                // 当 squareDist 小于 (0.5 - _Smoothness) 时返回 0，超过 0.5 时返回 1
                float vignette = smoothstep(0.5 - _Smoothness, 0.5, squareDist);

                // 采样阴影贴图颜色（贴图的 UV 也采用屏幕 UV）
                fixed4 vignetteTexColor = tex2D(_VignetteTex, i.uv);
                
                // 使用 lerp 将原始颜色与阴影贴图颜色进行混合
                // vignette 因子乘以 _Intensity 决定混合比例
                color.rgb = lerp(color.rgb, vignetteTexColor.rgb, vignette * _Intensity);
                return color;
            }
            ENDCG
        }
    }
}
