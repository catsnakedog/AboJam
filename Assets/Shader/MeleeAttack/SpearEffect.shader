Shader "Custom/GlitchArrow"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _GridSize ("Grid Size", Float) = 0.05
        _ShaftWidth ("Shaft Width", Float) = 0.1
        _ShaftHeight ("Shaft Height", Float) = 0.5
        _HeadHeight ("Head Height", Float) = 0.2
        _HeadWidth ("Head Width", Float) = 0.3
        _OffsetY ("Y Offset", Float) = 0.0
        _NoiseX ("Noise X Intensity", Range(0, 1)) = 0.05
        _NoiseY ("Noise Y Intensity", Range(0, 1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;
            float _GridSize;
            float _ShaftWidth;
            float _ShaftHeight;
            float _HeadHeight;
            float _HeadWidth;
            float _OffsetY;
            float _NoiseX;
            float _NoiseY;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Transform UV to range [-0.5, 0.5]
                float2 uv = i.uv * 2.0 - 1.0;

                // Apply pixel snapping
                uv = floor(uv / _GridSize + 0.5) * _GridSize;

                // Apply Y Offset
                uv.y += _OffsetY;

                // Generate random noise values for X and Y
                float noiseX = frac(sin(dot(uv * 100.0, float2(12.9898, 78.233))) * 43758.5453);
                float noiseY = frac(sin(dot(uv * 100.0, float2(42.3943, 92.8471))) * 43758.5453);

                // Apply noise-based offset
                float2 glitchUV = uv + float2(noiseX * _NoiseX, noiseY * _NoiseY);

                // Define the main shaft of the arrow (centered)
                float shaft = step(-_ShaftWidth * 0.5, glitchUV.x) * step(glitchUV.x, _ShaftWidth * 0.5) * step(-_ShaftHeight * 0.5, glitchUV.y) * step(glitchUV.y, _ShaftHeight * 0.5);

                // Define the arrowhead starting from the top of the shaft
                float headBaseY = _ShaftHeight * 0.5; // Top of the shaft
                float head = step(headBaseY, glitchUV.y) * step(-_HeadWidth * 0.5 * ((headBaseY + _HeadHeight - glitchUV.y) / _HeadHeight), glitchUV.x) * step(glitchUV.x, _HeadWidth * 0.5 * ((headBaseY + _HeadHeight - glitchUV.y) / _HeadHeight)) * step(glitchUV.y, headBaseY + _HeadHeight);

                // Combine all parts into a single arrow shape
                float arrowShape = max(shaft, head);

                // Apply color and glow
                fixed4 arrowColor = _Color * arrowShape;

                // Ensure transparent background for non-arrow areas
                arrowColor.a = arrowShape;

                return arrowShape;
            }
            ENDCG
        }
    }
}
