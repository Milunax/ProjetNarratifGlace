Shader "Custom/CRT_MaterialVersion_WithRTChoice"
{
    Properties
    {
        // 开关：0=用 _MainTex, 1=用 _MyRenderTex
        _UseRTOnOff("Use RenderTexture? (0=No,1=Yes)", Range(0,1)) = 0

        // RT 属性（当 _UseRTOnOff=1 时使用）
        _MyRenderTex("Render Texture", 2D) = "white" {}

        // 原图贴图 (当 _UseRTOnOff=0 时使用)
        _MainTex("Texture", 2D) = "white" {}

        // 以下就是你原先的各种特效开关/参数
        // ========== 鱼眼效果 ==========
        _FishEyeOnOff("FishEye OnOff (0=off,1=on)", Range(0,1)) = 0
        _FishEyeStrength("FishEye Strength", Range(0,1)) = 0.3

        // ========== 低分辨率 ==========
        _LowResOnOff("LowRes OnOff (0=off,1=on)", Range(0,1)) = 0
        _LowResMultiple("LowRes Multiple", Range(1,256)) = 2

        // ========== 颜色滤镜 ==========
        _ColorFilterOnOff("ColorFilter OnOff (0=off,1=on)", Range(0,1)) = 0
        _ColorFilterColor("ColorFilter Color", Color) = (1,1,1,1)
        _ColorFilterIntensity("ColorFilter Intensity", Range(0,2)) = 1

        // ========== 以下是原本的各种 CRT 效果开关 + 强度 ==========
        // Screen Jump
        _ScreenJumpOnOff("ScreenJump OnOff", Int) = 0
        _ScreenJumpOffset("ScreenJump Offset", Range(0,1)) = 0.2

        // Flickering
        _FlickeringOnOff("Flickering OnOff", Int) = 0
        _FlickeringStrength("Flickering Strength", Range(0,0.01)) = 0.002
        _FlickeringCycle("Flickering Cycle", Range(1,100)) = 50

        // Slippage
        _SlippageOnOff("Slippage OnOff", Int) = 0
        _SlippageNoiseOnOff("Slippage Noise OnOff", Range(0,1)) = 1
        _SlippageStrength("Slippage Strength", Range(0,0.02)) = 0.005
        _SlippageSize("Slippage Size", Range(1,20)) = 11
        _SlippageInterval("Slippage Interval", Range(0,5)) = 1
        _SlippageScrollSpeed("Slippage ScrollSpeed", Range(0,100)) = 33

        // Chromatic Aberration
        _ChromaticAberrationOnOff("ChromaticAberration OnOff", Int) = 0
        _ChromaticAberrationStrength("ChromaticAberration Strength", Range(0,0.01)) = 0.005

        // Multiple Ghost
        _MultipleGhostOnOff("MultipleGhost OnOff", Int) = 0
        _MultipleGhostStrength("MultipleGhost Strength", Range(0,0.05)) = 0.01

        // Scanline
        _ScanlineOnOff("Scanline OnOff", Int) = 0
        _ScanlineFrequency("Scanline Frequency", Range(0,1000)) = 700
        _ScanlineNoiseOnOff("Scanline Noise OnOff", Int) = 0

        // Monochrome
        _MonochromeOnOff("Monochrome OnOff", Int) = 0

        // --- Bloom ---
        _BloomOnOff("Bloom OnOff (0=off,1=on)", Range(0,1)) = 0
        _BloomThreshold("Bloom Threshold", Range(0,2)) = 1
        _BloomIntensity("Bloom Intensity", Range(0,5)) = 1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"
            #pragma enable_d3d11_debug_symbols
            #pragma vertex vert
            #pragma fragment frag

            //----------------------------------
            // 属性声明
            //----------------------------------

            // 开关: 0=使用 MainTex, 1=使用 MyRenderTex
            float _UseRTOnOff;

            // RT 贴图
            sampler2D _MyRenderTex;
            float4    _MyRenderTex_ST;

            // 原贴图
            sampler2D _MainTex;
            float4    _MainTex_ST;
            float4    _MainTex_TexelSize;

            // 其余特效参数
            float _FishEyeOnOff;
            float _FishEyeStrength;

            float _LowResOnOff;
            float _LowResMultiple;

            float  _ColorFilterOnOff;
            float4 _ColorFilterColor;
            float  _ColorFilterIntensity;

            int   _ScreenJumpOnOff;
            float _ScreenJumpOffset;

            int   _FlickeringOnOff;
            float _FlickeringStrength;
            float _FlickeringCycle;

            int   _SlippageOnOff;
            float _SlippageNoiseOnOff;
            float _SlippageStrength;
            float _SlippageSize;
            float _SlippageInterval;
            float _SlippageScrollSpeed;

            int   _ChromaticAberrationOnOff;
            float _ChromaticAberrationStrength;

            int   _MultipleGhostOnOff;
            float _MultipleGhostStrength;

            int   _ScanlineOnOff;
            float _ScanlineFrequency;
            int   _ScanlineNoiseOnOff;

            int   _MonochromeOnOff;

            // Bloom
            float _BloomOnOff;
            float _BloomThreshold;
            float _BloomIntensity;

            //----------------------------------
            // 辅助函数
            //----------------------------------
            float GetRandom(float value)
            {
                return frac(sin(dot(value, float2(12.9898, 78.233))) * 43758.5453);
            }

            float EaseIn(float t0, float t1, float t)
            {
                return 2.0 * smoothstep(t0, 2.0 * t1 - t0, t);
            }

             // Luminance 用于 Bloom
            float Luminance(float4 c)
            {
                return dot(c.rgb, float3(0.299, 0.587, 0.114));
            }

            // 简易BoxBlur(3x3) => Bloom
            float4 SampleBox9(sampler2D tex, float2 uv, float2 pixelSize)
            {
                float4 sum = 0;
                for (int y=-1; y<=1; y++)
                {
                    for (int x=-1; x<=1; x++)
                    {
                        float2 offset = float2(x, y) * pixelSize;
                        sum += tex2D(tex, uv+offset);
                    }
                }
                return sum / 9.0;
            }

            // 定义一个函数，自动根据开关采样哪个贴图
            float4 SampleBaseTex(float2 uv)
            {
                // if _UseRTOnOff>0.5 => 用 _MyRenderTex
                // else              => 用 _MainTex
                if (_UseRTOnOff > 0.5)
                {
                    return tex2D(_MyRenderTex, uv);
                }
                else
                {
                    return tex2D(_MainTex, uv);
                }
            }

            // 顶点结构
            struct a2v
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            //----------------------------------
            // 顶点着色器
            //----------------------------------
            v2f vert(a2v v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                // 贴图UV
                // 这里对 MainTex 做 transform，也可以对 RT 做 transform
                // 如果你想对 RT 做 offset，就改: TRANSFORM_TEX(v.uv,_MyRenderTex)
                o.uv  = v.uv;

                return o;
            }

            //----------------------------------
            // 片元着色器
            //----------------------------------
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // (A) 鱼眼 (FishEye)
                if (_FishEyeOnOff > 0.5)
                {
                    float2 center = float2(0.5, 0.5);
                    float2 offset = uv - center;
                    float dist    = length(offset) + 1e-8;
                    float newDist = dist * (1.0 + _FishEyeStrength * dist);
                    float2 dir    = offset / dist;
                    uv = center + dir * newDist;
                }

                // (B) 低分辨率
                if (_LowResOnOff > 0.5)
                {
                    float N = _LowResMultiple;
                    uv = floor(uv * N + 0.5) / N;
                }

                // ========== 先采样基础颜色(可能是 _MainTex，也可能是 _MyRenderTex) ==========
                // 这里相当于之前 'col = tex2D(_MainTex, uv);'
                // 但改为: 取决于 _UseRTOnOff
                float4 col = SampleBaseTex(uv);

                // (C) 部分特效对 uv 有进一步修改, 也会对后续采样影响
                // 例如: Flickering/ Slippage 再采一次? 
                // 但是根据你原本的代码逻辑, 你是先调 uv, 再 "color = tex2D(...)" 做多次采样
                // 我们这里保持逻辑: 做多次采样, 只不过都用 SampleBaseTex(uv) 取代 tex2D(_MainTex, uv)

                // Screen Jump (对uv二次修改)
                uv.y = frac(uv.y + _ScreenJumpOffset * _ScreenJumpOnOff);

                // Flickering
                float flickNoise = GetRandom(_Time.y);
                float flickMask  = pow(abs(sin(i.uv.y * _FlickeringCycle + _Time.y)), 10);
                uv.x += (flickNoise * flickMask * _FlickeringStrength * _FlickeringOnOff);

                // Slippage
                float scrollSpeed  = _Time.x * _SlippageScrollSpeed;
                float slippageMask = pow(abs(sin(i.uv.y * _SlippageInterval + scrollSpeed)), _SlippageSize);
                float stepMask     = round(sin(i.uv.y * _SlippageInterval + scrollSpeed - 1));
                uv.x += (slippageMask * stepMask * _SlippageStrength * _SlippageNoiseOnOff) * _SlippageOnOff;

                // chromatic aberration 需要多次采样
                float r = SampleBaseTex(float2(uv.x - _ChromaticAberrationStrength*_ChromaticAberrationOnOff, uv.y)).r;
                float g = SampleBaseTex(float2(uv.x, uv.y)).g;
                float b = SampleBaseTex(float2(uv.x + _ChromaticAberrationStrength*_ChromaticAberrationOnOff, uv.y)).b;
                float4 color = float4(r, g, b, 1);

                // Multiple Ghost
                float4 ghost1 = SampleBaseTex(uv - float2(1,0)*_MultipleGhostStrength*_MultipleGhostOnOff);
                float4 ghost2 = SampleBaseTex(uv - float2(2,0)*_MultipleGhostStrength*_MultipleGhostOnOff);
                color = color*0.8 + ghost1*0.15 + ghost2*0.05;

                // Scanline
                float scanline = sin((i.uv.y + _Time.x)*_ScanlineFrequency)*0.05;
                color -= scanline * _ScanlineOnOff;
                if (pow(sin(uv.y + _Time.y*2),300) * _ScanlineNoiseOnOff >= 0.999)
                {
                    color *= GetRandom(uv.y);
                }

                // Monochrome
                color.rgb += (0.299*color.r + 0.587*color.g + 0.114*color.b) * _MonochromeOnOff;

                // (D) 颜色滤镜
                if (_ColorFilterOnOff > 0.5)
                {
                    color.rgb *= (_ColorFilterColor.rgb * _ColorFilterIntensity);
                }

                // (E) Bloom
                if(_BloomOnOff>0.5)
                {
                    float lum = Luminance(color);
                    if(lum>_BloomThreshold)
                    {
                        float2 texPixSize = float2(_MainTex_TexelSize.z,_MainTex_TexelSize.w);
                        float4 blurCol = SampleBox9(_MainTex, uv, texPixSize);
                        float4 brightPart = max(blurCol - _BloomThreshold,0);
                        color += brightPart*_BloomIntensity;
                    }
                }

                return color;
            }

            ENDCG
        }
    }
}
