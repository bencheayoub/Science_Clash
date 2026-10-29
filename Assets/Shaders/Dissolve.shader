// Original shader for Science Clash (no third-party code).
// UI dissolve: procedural noise eats the image away; a glowing edge burns along the cut.
// Used on played cards and on defeated scientist portraits. _Dissolve goes 0 (whole) to 1 (gone).
Shader "ScienceClash/UI/Dissolve"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Dissolve ("Dissolve", Range(0, 1)) = 0
        [HDR] _EdgeColor ("Edge Color", Color) = (1, 0.8, 0.3, 1)
        _EdgeWidth ("Edge Width", Range(0.02, 0.4)) = 0.12
        _EdgeIntensity ("Edge Intensity", Range(0, 8)) = 3

        _NoiseScale ("Noise Scale (set by script)", Float) = 0.05
        _NoiseOrigin ("Noise Origin (set by script)", Vector) = (0, 0, 0, 0)

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
            float _Dissolve;
            half4 _EdgeColor;
            float _EdgeWidth;
            float _EdgeIntensity;
            float _NoiseScale;
            float4 _NoiseOrigin;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                // World position, so every layer of one card shares the same noise pattern.
                o.worldXY = mul(unity_ObjectToWorld, v.vertex).xy;
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
                return value / 0.875;   // normalise to 0..1
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv) * i.color;

                float2 p = (i.worldXY - _NoiseOrigin.xy) * _NoiseScale;
                float n = smoothstep(0.2, 0.8, fbm(p + 13.7));   // spread values so the cut sweeps evenly

                // The cut line moves from just below 0 (nothing dissolved) to above 1 (everything gone).
                float cut = lerp(-_EdgeWidth, 1.0, _Dissolve);
                float x = n - cut;
                clip(x);

                // Glow on the pixels that are about to burn away.
                float edge = 1.0 - saturate(x / _EdgeWidth);
                edge *= edge;
                col.rgb += _EdgeColor.rgb * (edge * _EdgeIntensity) * col.a;

                return col;
            }
            ENDCG
        }
    }
}
