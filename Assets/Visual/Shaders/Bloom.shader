Shader "Toony/Bloom"
{
	Properties
	{
		//TOONY COLORS
		_Color ("Color", Color) = (0.5,0.5,0.5,1.0)
		
		//DIFFUSE
		// [HideInInspector]
		// _MainTex ("Main Texture (RGB)", 2D) = "white" {}

		_Amount ("Extrusion Amount", Range(-1,1)) = 0.5

		[Header(Rim)]
		_RimNoise ("Rim Noise (RGB)", 2D) = "white" {}
		_RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
		_RimMin ("Rim Min", Range(0,1)) = 0.5
		_RimMax ("Rim Max", Range(0,1)) = 1.0
		_RimAnim ("Rim Animation: Speed(X), Strength Range(YZ)", vector) = (0,0,0,0)

		[Header(Bloom)]
		_OutlineWidth ("Outline Width", Range(0, 1)) = 0.05
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
		_OutlineBloomColor ("Outline Bloom Color", Color) = (0,0,0,1)
		_BloomLength ("Bloom Length", Range(0, 1)) = 0.7

		[Space(10)]
		_FresnelBias ("Fresnel Bias", Range(-3, 3)) = 0
		_FresnelScale ("Fresnel Scale", Range(-3, 3)) = 1
		_FresnelPower ("Fresnel Power", Range(-3, 3)) = 1
	}
	
	SubShader
	{
		Pass
		{
			Tags { "RenderType "= "Transparent" "Queue" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
				float3 viewDir : TEXCOORD2;
			};

			float _Amount;
			
			sampler2D _RimNoise;
			fixed4 _RimNoise_ST;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _Amount);
				o.normal = v.normal;
				o.uv = TRANSFORM_TEX(v.uv, _RimNoise);
				o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
				return o;
			}

			float4 _Color;
			float4 _RimColor;

			fixed _RimMin;
			fixed _RimMax;
			float4 _RimAnim;
			
			fixed4 frag (v2f i) : SV_Target
			{
				float t = _Time.y * _RimAnim.x;
				float3 viewDir = normalize(i.viewDir);
				half rim = 1.0f - saturate(dot(viewDir, i.normal));
				fixed rimNoise = tex2D(_RimNoise, i.vertex.xy * 0.01 * _RimNoise_ST.xy + float2(t, t) * 0.066).r;
				rim = smoothstep(_RimMin, _RimMax, rim * rimNoise + lerp(_RimAnim.y, _RimAnim.z, sin(t)));

				float4 color = lerp(_Color, _RimColor, rim);
				return color;
			}

			ENDCG
		}


		Pass {

			Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 vertexColor : COLOR0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float fresnel : TEXCOORD0;
			};

			half _OutlineWidth;

			fixed _FresnelBias;
			fixed _FresnelScale;
			fixed _FresnelPower;

			v2f vert(appdata v)
			{
				v2f o;
				float4 clipPosition = UnityObjectToClipPos(v.vertex);
				float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, v.vertexColor));

				clipPosition.xyz += normalize(clipNormal) * _OutlineWidth;

				float2 offset = normalize(clipNormal.xy) / _ScreenParams.xy * _OutlineWidth * clipPosition.w * 2;
				clipPosition.xy += offset;

				float3 i = normalize(ObjSpaceViewDir(v.vertex));
				o.fresnel = _FresnelBias + _FresnelScale * pow(1 + dot(i, v.normal), _FresnelPower);

				o.vertex = clipPosition;
				return o;
			}

			float4 _OutlineColor;
			float4 _OutlineBloomColor;
			float _BloomLength;

			// sampler2D _RimNoise;
			// fixed4 _RimNoise_ST;
			// float4 _RimAnim;

			float4 frag(v2f i) : SV_TARGET
			{
				float4 col = _OutlineBloomColor;
				if (i.fresnel > _BloomLength)
				{
					col = lerp(_OutlineBloomColor, _OutlineColor, smoothstep(0, 0.15, i.fresnel - _BloomLength));
					col.a *= smoothstep(0.3, 0.6, 1 - i.fresnel + 0.23);
				}
				// col.a *= smoothstep(0, 0.33, tex2D(_RimNoise, i.vertex * _RimNoise_ST.xy + float2(_Time.y * _RimAnim.x, _Time.y * _RimAnim.x) * 0.01).r);
				return col;
			}

			ENDCG

		}
	}
	
	Fallback "Diffuse"
}
