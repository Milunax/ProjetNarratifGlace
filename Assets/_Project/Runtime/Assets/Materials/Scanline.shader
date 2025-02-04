Shader "Custom/Scanline"
{
    Properties
    {
        // 原图贴图
        _MainTex("Texture", 2D) = "white" {}
        // 扫描线颜色（可调）
        _ScanLineColor("Scan Line Color", Color) = (0,1,0,1)
        // 扫描线宽度：值越大，扫描线带越宽（0～0.5）
        _ScanLineWidth("Scan Line Width", Range(0.0, 0.5)) = 0.1
        // 扫描线移动速度（UV周期/秒）
        _ScanLineSpeed("Scan Line Speed", Range(0.0, 10.0)) = 1.0
        // 扫描线混合强度：0表示不显示扫描线，1表示扫描线颜色完全覆盖原图
        _ScanLineIntensity("Scan Line Intensity", Range(0.0, 1.0)) = 1.0
        // 扫描线数量：控制屏幕内显示多少条扫描线（1～20）
        _ScanLineCount("Scan Line Count", Range(1,20)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            // 定义顶点与片元程序入口
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _ScanLineColor;
            float _ScanLineWidth;
            float _ScanLineSpeed;
            float _ScanLineIntensity;
            float _ScanLineCount; // 扫描线数量

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            // 顶点程序：转换顶点并传递 UV 坐标
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // 片元程序
            fixed4 frag(v2f i) : SV_Target
            {
                // 采样原图颜色
                fixed4 baseCol = tex2D(_MainTex, i.uv);

                // 计算动画相位：随着时间不断变化
                // 原本是x
                float phase = _Time.x * _ScanLineSpeed;

                // 生成重复的扫描线模式：
                // 将 i.uv.y 按 _ScanLineCount 个周期重复，加上 phase 实现动画效果，
                // frac 函数确保结果在 [0,1) 内循环
                // 原本是y
                float scanPattern = frac(i.uv.y * _ScanLineCount + phase);

                // 扫描线在每个周期中居中于 0.5，计算与中心的距离
                float dist = abs(scanPattern - 0.5);

                // 利用 smoothstep 生成软边带效果：
                // 当 dist 为 0（正好在扫描线中心）时值为 1，
                // 当 dist 大于或等于 _ScanLineWidth 时降为 0
                float scanFactor = 1.0 - smoothstep(0.0, _ScanLineWidth, dist);

                // 使用 lerp 将原图颜色与扫描线颜色混合
                fixed4 finalCol = lerp(baseCol, _ScanLineColor, scanFactor * _ScanLineIntensity);

                return finalCol;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
