Shader "Custom/AttackEffect/Bat"
{
    Properties
    {
        _Radius ("OutRadius", Float) = 1.0
        _InRadius ("InRadius", Float) = 0.5
        _LineRadius ("Line Radius", Float) = 1.0
        _RadiusScale ("Radius Scale", Float) = 1.0
        _GridSize ("Grid Size", Range(0.01, 1.0)) = 0.1
        _BaseProbability ("Base Probability", Float) = 0.5
        _Seed ("Seed", Float) = 0.5
        _RingProbabilityVariation ("Ring Probability Variation", Float) = 0.5
        _BaseColor ("Base Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _BaseUnActiveColor ("Base UnActive Color",Color) = (0.5, 0.5, 0.5, 0.5)
        _LineColor ("Line Color", Color) = (0.5, 0.5, 0.5, 0.5)
        _LineUnActiveColor ("Line UnActive Color",Color) = (0.5, 0.5, 0.5, 0.5)
        _BaseAlpha ("Base Alpha", Float) = 1
        _LineThickness ("Line Thickness", Float) = 0.05
        _LineAngle ("Line Angle", Range(0.0, 360.0)) = 45.0
        _AttackFlag ("Attack Flag", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _Radius;
            float _InRadius;
            float _LineRadius;
            float _RadiusScale;
            float _GridSize;
            float _BaseProbability;
            float _Seed;
            float _RingProbabilityVariation;
            float4 _BaseColor;
            float4 _BaseUnActiveColor;
            float4 _LineColor;
            float4 _LineUnActiveColor;
            float _BaseAlpha;
            float _LineThickness;
            float _LineAngle;
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

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float Random(float2 coord, float seed)
            {
                int n = int(coord.x * 157 + coord.y * 47 + seed * 101);
                n = (n << 13) ^ n;
                n = (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff;
                return float(n) / 2147483647.0;
            }

            float NormalizeAngle(float angle)
            {
                angle = fmod(angle, 360.0);
                return angle < 0 ? angle + 360.0 : angle;
            }

            bool IsAngleInRange(float angle, float minAngle, float maxAngle)
            {
                angle = NormalizeAngle(angle);
                minAngle = NormalizeAngle(minAngle);
                maxAngle = NormalizeAngle(maxAngle);

                if (minAngle <= maxAngle)
                {
                    return angle >= minAngle && angle <= maxAngle;
                }
                else
                {
                    return angle >= minAngle || angle <= maxAngle;
                }
            }

            fixed4 IsActiveColor(fixed3 activeColor, fixed3 unactiveColor ,float alpha)
            {
                if (_AttackFlag) return fixed4 (activeColor.rgb, alpha);
                else return fixed4 (unactiveColor.rgb, alpha);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;
                float distance = length(uv);

                float angle = atan2(uv.y, uv.x);
                angle = degrees(angle < 0 ? angle + 6.28318530718 : angle);

                float correctedThickness = _LineThickness / (smoothstep(_InRadius * _RadiusScale, _LineRadius * _RadiusScale, distance) * (2 - _RadiusScale));
                float minAngle = _LineAngle - correctedThickness;
                float maxAngle = _LineAngle + correctedThickness;

                if (IsAngleInRange(angle, minAngle, maxAngle) && distance > _Radius * _RadiusScale && distance <= _LineRadius)
                    return IsActiveColor(_LineColor.rgb, _LineUnActiveColor.rgb, 1.0);


                if (distance < _InRadius * _RadiusScale) 
                    return IsActiveColor(_BaseColor.rgb, _BaseUnActiveColor.rgb, _BaseAlpha);
                if (distance > _Radius * _RadiusScale) 
                    return fixed4(0, 0, 0, 0);

                float ringIndex = floor(distance / (_GridSize * _RadiusScale));
                bool isEvenRing = ((uint)ringIndex % 2) == 0;
                float ringProbability = (isEvenRing ? _RingProbabilityVariation : 1.0) * Random(float2(ringIndex, _Seed), _Seed);
                float alpha = ringProbability * smoothstep(_InRadius * _RadiusScale, _Radius * _RadiusScale, distance);

                return IsActiveColor(_BaseColor.rgb, _BaseUnActiveColor.rgb, alpha);
            }
            ENDCG
        }
    }
}