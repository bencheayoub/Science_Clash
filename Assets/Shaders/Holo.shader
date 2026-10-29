// Original shader for Science Clash (no third-party code).
// UI holographic foil: a rainbow sheen and sparkles that follow the mouse on a hovered card.
// CardHolo.cs drives _HoloAmount (0 = off, 1 = full), _HoloPointer, _HoloCenter and _HoloSize.
Shader "ScienceClash/UI/Holo"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Intensity ("Foil Intensity", Range(0, 2)) = 0.55
        _BandCount ("Rainbow Bands", Range(0.5, 6)) = 2.5
        _TintMix ("Accent Tint Mix", Range(0, 1)) = 0.35
        _SparkleDensity ("Sparkle Density", Range(0, 1)) = 0.5

        _HoloAmount ("Holo Amount (set by script)", Range(0, 1)) = 0
        _HoloPointer ("Pointer (set by script)", Vector) = (0, 0, 0, 0)
        _HoloCenter ("Card Center (set by script)", Vector) = (0, 0, 0, 0)
        _HoloSize ("Card Size (set by script)", Vector) = (1, 1, 0, 0)
        [HDR] _HoloTint ("Accent Tint (set by script)", Color) = (0.24, 0.85, 1, 1)

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
        Blend SrcAlpha OneMinusSrcAlpha

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
                float2 worldXY : TEXCOORD1;
            };

            sampler2D _MainTex;
            float _Intensity;
            float _BandCount;
            float _TintMix;
            float _SparkleDensity;
            float _HoloAmount;
            float4 _HoloPointer;
            float4 _HoloCenter;
            float4 _HoloSize;
            half4 _HoloTint;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                // World position, so every layer of one card shares the same foil pattern.
                o.worldXY = mul(unity_ObjectToWorld, v.vertex).xy;
                return o;
            }

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv) * i.color;

                // Position on the card, about -0.5 .. 0.5 from left/bottom to right/top.
                float2 local = (i.worldXY - _HoloCenter.xy) / max(_HoloSize.xy, 0.0001);
                float2 pointer = _HoloPointer.xy;
                float2 toPointer = local - pointer;

                // Bright zone around the cursor.
                float spot = exp(-dot(toPointer, toPointer) * 7.0);

                // Diagonal rainbow bands that slide when the cursor moves.
                float2 bandDir = float2(0.8, 0.6);
                float phase = dot(local, bandDir) * _BandCount + dot(pointer, bandDir) * _BandCount * 1.5;
                float3 rainbow = 0.5 + 0.5 * cos(6.28318 * (phase + float3(0.0, 0.33, 0.67)));
                float3 foil = lerp(rainbow, _HoloTint.rgb, _TintMix);
                float sheen = 0.3 + 0.7 * spot;

                // Tiny twinkling dots, strongest near the cursor.
                float2 grid = local * 26.0;
                float r = hash21(floor(grid) + 7.0);
                float dotShape = saturate(1.0 - length(frac(grid) - 0.5) * 2.5);
                float twinkle = pow(saturate(sin(_Time.y * (2.0 + r * 5.0) + r * 40.0) * 0.5 + 0.5), 6.0);
                float sparkle = step(1.0 - _SparkleDensity * 0.15, r) * dotShape * twinkle * (0.3 + spot);

                float3 add = foil * sheen + spot * 0.25 + sparkle;
                col.rgb += add * (_Intensity * _HoloAmount) * col.a;

                return col;
            }
            ENDCG
        }
    }
}
