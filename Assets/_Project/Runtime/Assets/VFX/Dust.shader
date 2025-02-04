Shader "Custom/DustParticleShader"
{
    Properties
    {
        // 粒子使用的贴图，建议使用带有透明通道的灰尘纹理
        _MainTex ("Particle Texture", 2D) = "white" {}
        // 用于整体调节颜色，可用于淡化或调整色调
        _TintColor ("Tint Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        // 设置队列为 Transparent 并标记为透明物体，确保正确渲染顺序
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        LOD 100

        // 使用 Alpha 混合，关闭写入深度、背面剔除以及光照计算，提升性能
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            // 指定使用顶点与片元程序
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // 输入结构：从粒子系统中传入顶点位置、颜色和UV
            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color  : COLOR;
                float2 uv     : TEXCOORD0;
            };

            // 输出结构：传递裁剪空间位置、UV和颜色到片元程序
            struct v2f
            {
                float4 pos   : SV_POSITION;
                float2 uv    : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _TintColor;

            // 顶点程序：将顶点转换到裁剪空间，并混合粒子颜色与全局 Tint 颜色
            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _TintColor;
                return o;
            }

            // 片元程序：采样纹理并与传入颜色相乘
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);
                fixed4 finalColor = texColor * i.color;
                return finalColor;
            }
            ENDCG
        }
    }
}
