Shader "Unlit/Ray"
{
    Properties
    {
        // [HideInInspector]
        _Noise ("Noise (RGB)", 2D) = "white" {}
        _NoiseSpeed ("Noise Speed", vector) = (0.2333, 0.1, 0, 0)
        _NoiseStrength ("Noise Strength", Range(0, 5)) = 0.333

        _Color ("Color", Color) = (0,0,0,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)

        _DepthFactor("Depth Factor", float) = 0.5
        _DepthPow("Depth Pow", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD1;
                float depth : TEXCOORD2;
            };

            sampler2D _Noise;
            float4 _Noise_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _Noise);

                o.screenPos = ComputeScreenPos(o.vertex);
                COMPUTE_EYEDEPTH(o.depth);

                return o;
            }

            float2 _NoiseSpeed;
            float _NoiseStrength;

            float4 _Color;
            float4 _OutlineColor;

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
            float _DepthFactor;
            float _DepthPow;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = float2(i.uv.x, i.uv.y / _Noise_ST.y + tex2D(_Noise, i.uv - float2(_Time.y * _NoiseSpeed.x, _Time.y * _NoiseSpeed.y)).r * _NoiseStrength);

                float depth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.screenPos)));

                fixed4 col = lerp(_Color, _OutlineColor, smoothstep(0.1, 0.3, abs(uv.y - 0.5) * 1.66));
                col.a = 1 - smoothstep(0, 0.7, abs(uv.y - 0.5) * 2);
                col.a *= saturate(pow(abs(depth - i.depth), _DepthPow) / _DepthFactor);

                return col;
            }
            ENDCG
        }
    }
}
