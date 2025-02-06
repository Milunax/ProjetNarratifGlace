Shader "Custom/Dust"
{
    Properties
    {
        // 粒子使用的贴图，通常由粒子系统提供
        _MainTex ("Particle Texture", 2D) = "white" {}
        // 整体调色（粒子系统的 Color over Lifetime 效果会与此相乘）
        _TintColor ("Tint Color", Color) = (1,1,1,1)
        // 指定区域的最小坐标（X, Y, Z），例如 (-5, -5, -5)
        _RegionMin ("Region Min (X, Y, Z)", Vector) = (-5, -5, -5, 0)
        // 指定区域的最大坐标（X, Y, Z），例如 (5, 5, 5)
        _RegionMax ("Region Max (X, Y, Z)", Vector) = (5, 5, 5, 0)
        // 区域内用于混合的颜色（例如希望调成红色调）
        _RegionColor ("Region Color", Color) = (1,0,0,1)
        // 混合强度：0 表示不影响，1 表示区域内完全替换为 _RegionColor
        _RegionBlendIntensity ("Region Blend Intensity", Range(0,1)) = 1.0
        // 边缘软化距离：控制区域边缘过渡的平滑度（单位为世界坐标距离）
        _EdgeSoftness ("Edge Softness", Range(0,5)) = 0.5
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        LOD 200

        Pass
        {
            CGPROGRAM
            // 顶点和片元程序入口
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TintColor;
            // _RegionMin 与 _RegionMax 分别定义三维区域的最小和最大值（只取 XYZ 分量）
            float4 _RegionMin;
            float4 _RegionMax;
            fixed4 _RegionColor;
            float _RegionBlendIntensity;
            float _EdgeSoftness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv       : TEXCOORD0;
                float4 vertex   : SV_POSITION;
                // 粒子的世界坐标（需从 Custom Vertex Streams 中传递）
                float3 worldPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float4 worldPos4 = mul(unity_ObjectToWorld, v.vertex);
                o.worldPos = worldPos4.xyz;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // 采样原图颜色并乘上整体调色（包含 Color over Lifetime 效果）
                fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;

                // 分别计算粒子在 X、Y、Z 三轴上与区域边界的距离
                float distXMin = i.worldPos.x - _RegionMin.x;
                float distXMax = _RegionMax.x - i.worldPos.x;
                float distYMin = i.worldPos.y - _RegionMin.y;
                float distYMax = _RegionMax.y - i.worldPos.y;
                float distZMin = i.worldPos.z - _RegionMin.z;
                float distZMax = _RegionMax.z - i.worldPos.z;

                // 如果粒子完全位于区域外，至少有一个轴的距离为负
                float inside = step(0.0, distXMin) * step(0.0, distXMax) *
                               step(0.0, distYMin) * step(0.0, distYMax) *
                               step(0.0, distZMin) * step(0.0, distZMax);

                // 找出粒子距离各轴边界中的最小值
                float minDist = min( min(distXMin, distXMax),
                                     min( min(distYMin, distYMax),
                                          min(distZMin, distZMax) ) );

                // 使用 smoothstep 实现边缘软化：当距离小于 _EdgeSoftness 时逐渐过渡
                float smoothFactor = smoothstep(0.0, _EdgeSoftness, minDist);

                // 最终混合因子：粒子完全在区域内部（inside==1 且平滑因子==1）时达到 1，
                // 边缘处根据 _EdgeSoftness 平滑过渡，区域外则为 0
                float blendFactor = inside * smoothFactor;

                // 根据混合因子和混合强度，将原始颜色与区域颜色线性混合
                col.rgb = lerp(col.rgb, _RegionColor.rgb, blendFactor * _RegionBlendIntensity);

                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
