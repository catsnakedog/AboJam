Shader "Custom/ParabolicEffect"
{
    Properties
    {
        _Color("Base Color", Color) = (1,1,1,1)
        _Radius("Radius", Float) = 1.0
        _InRadius("InRadius", Float) = 0.5
        _Opacity("Opacity", Float) = 1.0
        _GridSize("Grid Size", Float) = 0.1
        _AngleStart("Angle Start (Degrees)", Float) = 0.0
        _AngleRange("Angle Range (Degrees)", Float) = 90.0
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
            float _Opacity;
            float _GridSize;
            float _AngleStart;
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

                // Check if within the angle range
                float angleStart = _AngleStart;
                float angleEnd = _AngleStart + _AngleRange;
                if (angle < angleStart || angle > angleEnd)
                    return fixed4 (0, 0, 0, 0);

                return _Color;
            }
            ENDCG
        }
    }
}