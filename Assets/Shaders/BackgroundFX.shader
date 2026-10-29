// Original shader for Science Clash (no third-party code).
// A UI overlay: drifting energy haze, a slow light sweep, faint scanlines and a vignette.
// Put it on a full-screen UI Image (with NO source image) above the lab background.
Shader "ScienceClash/UI/BackgroundFX"
{
    Properties
    {
        [HideInInspector] _MainTex ("Texture", 2D) = "white" {}

        [Header(Colors)]
        _PrimaryColor ("Primary", Color) = (0.24, 0.85, 1.0, 1)
        _SecondaryColor ("Secondary", Color) = (1.0, 0.70, 0.28, 1)

        [Header(Haze)]
        _Aspect ("Aspect Ratio", Float) = 1.78
        _Speed ("Speed", Range(0, 2)) = 0.12
        _HazeScale ("Haze Scale", Range(0.5, 8)) = 2.5
        _HazeStrength ("Haze Strength", Range(0, 1)) = 0.35

        [Header(Sweep)]
        _SweepStrength ("Sweep Strength", Range(0, 0.5)) = 0.08

        [Header(Scanlines and Vignette)]
        _ScanlineCount ("Scanline Count", Range(50, 600)) = 220
        _ScanlineStrength ("Scanline Strength", Range(0, 0.5)) = 0.10
        _VignetteStrength ("Vignette Strength", Range(0, 1)) = 0.55

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        ColorMask [_ColorMask]

        // Premultiplied alpha: rgb ADDS light, alpha DARKENS what is behind.
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            half4 _PrimaryColor;
            half4 _SecondaryColor;
            float _Aspect;
            float _Speed;
            float _HazeScale;
            float _HazeStrength;
            float _SweepStrength;
            float _ScanlineCount;
            float _ScanlineStrength;
            float _VignetteStrength;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float valueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash21(i);
                float b = hash21(i + float2(1, 0));
                float c = hash21(i + float2(0, 1));
                float d = hash21(i + float2(1, 1));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                for (int k = 0; k < 3; k++)
                {
                    value += amplitude * valueNoise(p);
                    p *= 2.0;
                    amplitude *= 0.5;
                }
                return value;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float t = _Time.y * _Speed;

                // Drifting haze: two layers of noise moving in different directions.
                float2 p = float2(uv.x * _Aspect, uv.y) * _HazeScale;
                float n1 = fbm(p + float2(t, t * 0.6));
                float n2 = fbm(p * 1.7 - float2(t * 0.8, t));
                float haze = smoothstep(0.35, 0.85, n1 * n2 * 2.0);
                float3 glow = lerp(_PrimaryColor.rgb, _SecondaryColor.rgb, n2) * haze * _HazeStrength;

                // A thin band of light slowly travelling upward.
                float bandWave = abs(frac(uv.y * 0.5 - t * 0.7) - 0.5) * 2.0;
                float sweep = pow(saturate(1.0 - bandWave), 8.0);
                glow += _PrimaryColor.rgb * sweep * _SweepStrength;

                // Faint scanlines and darker edges.
                float scan = 0.5 + 0.5 * sin(uv.y * _ScanlineCount * 6.2831853);
                float vignette = smoothstep(0.35, 0.9, length(uv - 0.5));
                float dark = saturate(scan * _ScanlineStrength + vignette * _VignetteStrength);

                half4 result = half4(glow, dark);
                return result * i.color.a;   // the Image's alpha acts as overall intensity
            }
            ENDCG
        }
    }
}
