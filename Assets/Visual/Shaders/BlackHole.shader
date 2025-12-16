Shader "Unlit/BlackHole"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags {"RenderType"="Opaque" "Queue"="Geometry+1" }
        ColorMask 0
        Stencil
        {
            Ref 1
            Comp always
            Pass replace
        }

        CGINCLUDE
        struct appdata
        {
            float4 vertex : POSITION;
        };

        struct v2f
        {
            float4 pos : SV_POSITION;
        };

        v2f vert (appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);

            return o;
        }

        fixed4 _Color;

        half4 frag (v2f i) : SV_Target
        {
            return _Color;
        }

        ENDCG

        Pass
        {
            Cull Front
            ZTest Less

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }

        Pass
        {
            Cull Back
            ZTest Greater

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
