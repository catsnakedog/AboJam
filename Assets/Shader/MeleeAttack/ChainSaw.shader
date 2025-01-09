Shader "Custom/AttackEffect/ChainSaw"
{
    Properties
    {
        _Radius ("Radius", Float) = 1.0
        _InRadius ("In Radius", Float) = 0.5
        _EffectRadius ("Effect Radius", Float) = 1.5
        _GridSize ("Grid Size", Range(0.01, 1.0)) = 0.1
        _WaveFrequency ("Wave Frequency", Float) = 10.0
        _BaseProbability ("Base Probability", Float) = 0.5
        _Seed ("Seed", Float) = 0.5
        _EffectAngle ("Effect Angle", Float) = 0.0
        _BaseColor ("Base Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _BaseUnActiveColor ("UnActive Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _BaseAlpha ("Base Alpha", Float) = 1
        _AttackFlag ("Active Flag", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _Radius;
            float _InRadius;
            float _EffectRadius;
            float _GridSize;
            float _WaveFrequency;
            float _BaseProbability;
            float _Seed;
            float _EffectAngle;
            float4 _BaseColor;
            float4 _BaseUnActiveColor;
            float _BaseAlpha;
            float _AttackFlag;

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

            // Rotate UV based on angle
            float2 RotateUV(float2 uv, float angle)
            {
                float rad = radians(angle);
                float cosA = cos(rad);
                float sinA = sin(rad);
                return float2(
                    uv.x * cosA - uv.y * sinA,
                    uv.x * sinA + uv.y * cosA
                );
            }

            // Generate random values
            float Random(float2 coord, float seed)
            {
                int n = int(coord.x * 157 + coord.y * 47 + seed * 101);
                n = (n << 13) ^ n;
                n = (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff;
                return float(n) / 2147483647.0;
            }


            // Return active or inactive color with HDR support
            fixed4 IsActiveColor(float alpha)
            {
                float4 activeColor = _AttackFlag > 0.5 ? _BaseColor : _BaseUnActiveColor;
                float clampAlpha = clamp(alpha, 0, 1);
                return float4(activeColor.rgb, clampAlpha);
            }

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;
                float2 snappedUV = floor(uv / _GridSize + 0.5) * _GridSize;

                float distance = length(snappedUV);

                // If outside the maximum effect radius, return transparent
                if (distance > _EffectRadius) return fixed4(0, 0, 0, 0);

                // If within inner radius, return full alpha
                if (distance < _InRadius) return IsActiveColor(_BaseAlpha);

                // Base alpha for transition effect
                float baseAlpha = 1.0 - smoothstep(_InRadius, _Radius, distance);

                // Random effect for Radius and below
                if (distance <= _Radius)
                {
                    float ringIndex = floor(distance / _GridSize);
                    bool isEvenRing = ((uint)ringIndex % 2) == 0;
                    float ringProbability = (isEvenRing ? _BaseProbability : 1.0) * Random(float2(ringIndex, _Seed), _Seed);
                    float alpha = ringProbability * baseAlpha;
                    return IsActiveColor(alpha);
                }

                // Wave effect for Radius ~ EffectRadius
                if (distance > _Radius && distance <= _EffectRadius)
                {
                    // Rotate UV for effect area
                    float2 effectUV = RotateUV(uv, _EffectAngle);
                    float angle = atan2(effectUV.y, effectUV.x); // Calculate angle

                    // Map angle to wave pattern
                    float wave = sin(angle * _WaveFrequency);

                    wave *= (_EffectRadius - _Radius) / 2;

                    if (_AttackFlag == 0.0)
                        return float4(0, 0, 0, 0);
                    // Combine wave effect with base alpha
                    if (wave + (_EffectRadius + _Radius) / 2 >= distance)
                        return IsActiveColor(1);
                }

                return fixed4(0, 0, 0, 0); // Default transparent for safety
            }
            ENDCG
        }
    }
}