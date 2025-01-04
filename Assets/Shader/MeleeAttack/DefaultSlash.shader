Shader "Custom/AttackEffect/NormalAttack"
{
    Properties
    {
        _Radius ("Radius", Float) = 1.0
        _InRadius ("InRadius", Float) = 0.5
        _GridSize ("Grid Size", Float) = 0.1
        _BaseAngle ("Base Angle", Range(0, 360)) = 45
        _FadeAngleRange ("Fade Angle Range", Float) = 30
        _VisibleRange ("Visible Range", Float) = 90
        _BaseProbability ("Base Probability", Float) = 0.5
        _DistanceWeight ("Distance Weight", Range(0, 1)) = 1.0
        _AngleWeight ("Angle Weight", Range(0, 1)) = 1.0
        _EvenRingProbability ("Even Ring Probability", Float) = 0.5
        _Seed ("Random Seed", Float) = 0.5
        _AlphaCut ("Alpha Cutting Min Value", Range(0, 1)) = 0.3
        _FlipX ("Flip X Axis", Float) = 0.0 // 0.0 = No Flip, 1.0 = Flip X Axis
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            float _Radius;
            float _InRadius;
            float _GridSize;
            float _BaseAngle;
            float _FadeAngleRange;
            float _VisibleRange;
            float _BaseProbability;
            float _DistanceWeight;
            float _AngleWeight;
            float _Seed;
            float _EvenRingProbability;
            float _AlphaCut;
            float _FlipX; // Add this variable to control X axis flip

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

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5; // Center of UV
                
                // Apply the FlipX transformation if _FlipX is 1
                if (_FlipX > 0.5)
                {
                    uv.x = -uv.x; // Flip along X axis
                }
                
                float2 snappedUV = floor(uv / _GridSize + 0.5) * _GridSize; // UV Snap

                float distance = length(snappedUV);
                float angle = atan2(snappedUV.y, snappedUV.x) * 57.2958; // Calculate angle in degrees
                if(angle < 0) angle += 360;

                if (distance > _Radius) return fixed4(0, 0, 0, 0); // If Grid is Out -> None
                if (distance < _InRadius) return fixed4(0, 0, 0, 0);

                float4 basePixel = fixed4(0, 0, 0, 0); // Initialize as transparent

                float angleDifference = angle - _BaseAngle;
                if (angleDifference < 0) angleDifference += 360;

                if (angleDifference > _VisibleRange) return fixed4(0, 0, 0, 0);
                if (angleDifference > _FadeAngleRange) return fixed4(0, 0, 0, 0);

                float angleScale = (1.0 - abs(angleDifference / _VisibleRange)) * _AngleWeight;
                float ringIndex = floor(distance / _GridSize); 
                float ringProbability = ((uint)ringIndex % 2 == 0) ? _EvenRingProbability : 1.0;
                ringProbability *= Random(float2(ringIndex, _Seed), _Seed);

                float distanceProbability = pow(distance / _Radius, _DistanceWeight);

                float alpha = _BaseProbability * angleScale * ringProbability * distanceProbability;
                if (alpha > _AlphaCut) return fixed4(alpha, alpha, alpha, alpha);

                return fixed4(0, 0, 0, 0); // if alpha < 0.3 return 0 - Reject Small Value
            }
            ENDCG
        }
    }
}
