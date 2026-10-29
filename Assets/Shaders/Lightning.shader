Shader "ScienceClash/Lightning"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Intensity ("Intensity", Range(0, 1)) = 1
        _Seed ("Seed", Float) = 0
        _Thickness ("Thickness", Range(0.005, 0.08)) = 0.02
        _Amplitude ("Wiggle", Range(0, 0.5)) = 0.4
        _FlickerRate ("Flicker rate", Float) = 24
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha One   // additive: the bolt glows on top of the scene

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
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _Color;
            float _Intensity;
            float _Seed;
            float _Thickness;
            float _Amplitude;
            float _FlickerRate;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            float Hash(float n) { return frac(sin(n * 127.1) * 43758.5453); }

            float Noise(float x)
            {
                float i = floor(x);
                float f = frac(x);
                f = f * f * (3.0 - 2.0 * f);
                return lerp(Hash(i), Hash(i + 1.0), f);
            }

            // Distance from this pixel to a jagged line that runs from the left edge to the right edge.
            float BoltDistance(float2 uv, float seed, float wiggle)
            {
                float envelope = sin(uv.x * 3.14159265);   // 0 at both ends, so the bolt touches both portraits
                float shape = (Noise(uv.x * 7.0 + seed) - 0.5) * 1.2
                            + (Noise(uv.x * 19.0 + seed * 1.7) - 0.5) * 0.6
                            + (Noise(uv.x * 47.0 + seed * 2.3) - 0.5) * 0.25;
                float y = 0.5 + shape * wiggle * envelope;
                return abs(uv.y - y);
            }

            float4 frag(v2f i) : SV_Target
            {
                // The shape changes a few times per second, which makes it crackle.
                float seed = _Seed + floor(_Time.y * _FlickerRate) * 3.7;

                float d1 = BoltDistance(i.uv, seed, _Amplitude);
                float d2 = BoltDistance(i.uv, seed + 17.3, _Amplitude * 1.4);   // thin second strand

                float core = pow(saturate(1.0 - d1 / _Thickness), 2.0);
                float glow = exp(-d1 / (_Thickness * 5.0)) * 0.45;
                float strand = pow(saturate(1.0 - d2 / (_Thickness * 0.6)), 2.0) * 0.6;

                float total = core + glow + strand;
                float3 rgb = lerp(i.color.rgb, float3(1, 1, 1), saturate(core + strand * 0.5));   // white-hot center, colored edge
                return float4(rgb, saturate(total) * _Intensity * i.color.a);
            }
            ENDCG
        }
    }
}
