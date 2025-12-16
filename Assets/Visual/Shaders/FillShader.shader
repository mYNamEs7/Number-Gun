Shader "Custom/FillShader" {
    Properties {
        _FillAmount ("Fill Amount", Range(0, 1)) = 0.5 // Уровень заполнения
        _BottomColor ("Bottom Color", Color) = (0, 1, 0, 1) // Цвет заполненной части
        _TopColor ("Top Color", Color) = (1, 0, 0, 1) // Цвет незаполненной части
    }
    SubShader {
        Tags { "Queue" = "Geometry" }
        LOD 200

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _FillAmount;
            fixed4 _BottomColor;
            fixed4 _TopColor;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                // Определяем, как изменять цвет вершины в зависимости от высоты
                float heightFactor = saturate((v.vertex.y + 0.5) / 1.0); // Нормализуем высоту Y от 0 до 1 (для куба)

                // Если вершина ниже уровня заполнения, используем нижний цвет, иначе — верхний
                o.color = lerp(_BottomColor, _TopColor, step(_FillAmount, heightFactor));
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                return i.color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
