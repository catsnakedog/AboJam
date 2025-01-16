Shader "Custom/ParabolicEffect"
{
    Properties
    {
        _Color("Base Color", Color) = (1,1,1,1)
        _Radius("Radius", Float) = 1.0
        _InRadius("InRadius", Float) = 0.5
        _CorrectRadius("Correct Radius", Float) = 0.5
        _CorrectInRadius("Correct InRadius", Float) = 0.5
        _CorrectY("Correct Y", Float) = 1
        _Opacity("Opacity", Float) = 1.0
        _GridSize("Grid Size", Float) = 0.1
        _AngleCenter("Angle Center", Float) = 0.0
        _AngleRange("Angle Range", Float) = 90.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            fixed4 _Color;
            float _Radius;
            float _InRadius;
            float _CorrectRadius;
            float _CorrectInRadius;
            float _CorrectY;
            float _Opacity;
            float _GridSize;
            float _AngleCenter;
            float _AngleRange;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv - 0.5;
                float2 snappedUV = floor(uv / _GridSize + 0.5) * _GridSize;

                // Calculate radial distance and angle from the center
                float distance = length(snappedUV);

                if (distance > _Radius) return fixed4(0, 0, 0, 0); // If Grid is Out -> None
                if (distance < _InRadius) return fixed4(0, 0, 0, 0);

                float angle = degrees(atan2(snappedUV.y, snappedUV.x));

                // Normalize angle to 0-360 range
                angle = (angle < 0) ? angle + 360.0 : angle;

                float angleRad = _AngleCenter * (3.14159265 / 180.0);

                float2 direction = float2(cos(angleRad), sin(angleRad) * _CorrectY);
                float2 centerPoint = direction * _InRadius;
                float correctDistance = length(snappedUV - centerPoint);

                if (correctDistance > _CorrectRadius) return fixed4(0, 0, 0, 0);
                if (correctDistance < _CorrectInRadius) return fixed4(0, 0, 0, 0);

                float halfRange = _AngleRange * 0.5;

                float angleStart = fmod(_AngleCenter - halfRange + 360.0, 360.0);
                float angleEnd = fmod(_AngleCenter + halfRange, 360.0);

                if (angleStart < angleEnd) {
                    if (angle < angleStart || angle > angleEnd)
                        return fixed4(0, 0, 0, 0);
                } else {
                    if (angle > angleEnd && angle < angleStart)
                        return fixed4(0, 0, 0, 0);
                }

                return fixed4(_Color.xyz, _Opacity);
            }
            ENDCG
        }
    }
}