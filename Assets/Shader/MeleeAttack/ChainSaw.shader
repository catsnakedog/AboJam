Shader "Custom/AttackEffect/ChainSaw"
{
    Properties
    {
        _Radius ("OutRadius", Float) = 1.0
        _InRadius ("InRadius", Float) = 0.5
        _GridSize ("Grid Size", Range(0.01, 1.0)) = 0.1
        _BaseProbability ("Base Probability", Float) = 0.5
        _Seed ("Seed", Float) = 0.5
        _RingProbabilityVariation ("Ring Probability Variation", Float) = 0.5
        _BaseColor ("Base Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _BaseAlpha ("Base Alpha", Float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _Radius;
            float _InRadius;
            float _GridSize;
            float _BaseProbability;
            float _Seed;
            float _RingProbabilityVariation;
            float4 _BaseColor;
            float _BaseAlpha;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float Random(float2 coord, float seed)
            {
                // Get Big Value - To Get Random Value
                int n = int(coord.x * 157 + coord.y * 47 + seed * 101);
    
                // bitCalc To add Chaos
                n = (n << 13) ^ n;
                n = (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff; // Make +

                // Normalize to [0, 1]
                return float(n) / 2147483647.0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5; // Center of UV
                float2 snappedUV = floor(uv / _GridSize + 0.5) * _GridSize; // UV Snap

                float distance = length(snappedUV);

                // Check Radius
                if (distance > _Radius) return fixed4(0, 0, 0, 0); // If Grid is Out -> None
                if (distance < _InRadius) return fixed4(_BaseColor.rgb, _BaseAlpha);


                float baseAlpha = 1.0 - smoothstep(_InRadius, _Radius, distance); // Distance Correct - Close : Down, Far : Up

                fixed4 baseColor = fixed4(_BaseColor.rgb, baseAlpha); // Gray background


                // Ring-based random
                float ringIndex = floor(distance / _GridSize); // Ring Idx
                bool isEvenRing = ((uint)ringIndex % 2) == 0; // Check if ring index is even
                float ringProbability = (isEvenRing ? _RingProbabilityVariation : 1.0) * Random(float2(ringIndex, _Seed), _Seed);
                // Ring Correct by Even
                float alpha = ringProbability * smoothstep(_InRadius, _Radius, distance);

                // Combine base color with generated alpha
                return lerp(baseColor, fixed4(baseColor.rgb, alpha), alpha);
            }
            ENDCG
        }
    }
}