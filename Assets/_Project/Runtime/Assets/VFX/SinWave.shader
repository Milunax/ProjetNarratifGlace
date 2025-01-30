Shader "Custom/SineWave"
{
    Properties
    {
        // === Wave1: 不同频率、相位、振幅 ===
        [Header(Wave 1)]
        _Wave1Amplitude("Wave1/Amplitude", Range(0,1)) = 0.3
        _Wave1Frequency("Wave1/Frequency", Range(0,10)) = 1
        _Wave1Phase("Wave1/Phase", Range(0,6.2831853)) = 0

        // === Wave2
        [Header(Wave 2)]
        _Wave2Amplitude("Wave2/Amplitude", Range(0,1)) = 0.3
        _Wave2Frequency("Wave2/Frequency", Range(0,10)) = 2
        _Wave2Phase("Wave2/Phase", Range(0,6.2831853)) = 1.5708  // ~ π/2

        // === Wave3
        [Header(Wave 3)]
        _Wave3Amplitude("Wave3/Amplitude", Range(0,1)) = 0.3
        _Wave3Frequency("Wave3/Frequency", Range(0,10)) = 3
        _Wave3Phase("Wave3/Phase", Range(0,6.2831853)) = 3.14159 // ~ π

        // === 共同控制 ===
        [Header(MovementAndVisual)]
        _MovementSpeed("Movement Speed", Range(0,2)) = 1
        _LineThickness("Line Thickness", Range(0,0.1)) = 0.02
        _LineColor("Line Color", Color) = (1,1,1,1)
        _BGColor("Background Color", Color) = (0,0,0,0)
    }

    SubShader
    {
        // 使用 Transparent Queue，让背景可带Alpha
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            // 开启Alpha混合，并且关闭深度写
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma vertex vert
            #pragma fragment frag

            // Wave1
            float _Wave1Amplitude;
            float _Wave1Frequency;
            float _Wave1Phase;

            // Wave2
            float _Wave2Amplitude;
            float _Wave2Frequency;
            float _Wave2Phase;

            // Wave3
            float _Wave3Amplitude;
            float _Wave3Frequency;
            float _Wave3Phase;

            // 共同
            float _MovementSpeed;
            float _LineThickness;
            float4 _LineColor;
            float4 _BGColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // 时间(秒)
                float t = _Time.x;
                float PI2 = 6.2831853; // 2π

                // --- 计算三条波 ---
                // wave1:
                float wave1 = _Wave1Amplitude
                    * sin(PI2*_Wave1Frequency*(uv.x + t*_MovementSpeed) + _Wave1Phase);

                // wave2:
                float wave2 = _Wave2Amplitude
                    * sin(PI2*_Wave2Frequency*(uv.x + t*_MovementSpeed) + _Wave2Phase);

                // wave3:
                float wave3 = _Wave3Amplitude
                    * sin(PI2*_Wave3Frequency*(uv.x + t*_MovementSpeed) + _Wave3Phase);

                // 合成
                float sum = wave1 + wave2 + wave3;

                // 基线设为 y=0.5
                float waveY = 0.5 + sum;

                // 距离线中心
                float dist = abs(uv.y - waveY);
                float halfT = _LineThickness * 0.5;

                // step(edge,x): x<edge =>0, x>edge =>1
                // lineMask=1 => 在曲线上
                float lineMask = 1.0 - step(halfT, dist);

                // lineMask=1 => _LineColor, else =>_BGColor
                fixed4 finalColor = lerp(_BGColor, _LineColor, lineMask);

                return finalColor;
            }
            ENDCG
        }
    }
}
