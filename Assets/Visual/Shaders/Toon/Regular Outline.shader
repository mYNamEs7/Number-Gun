Shader "Toony/Default + Outline"
{
	Properties
	{
		//TOONY COLORS
		_Color ("Color", Color) = (0.5,0.5,0.5,1.0)
		_SColor ("Shadow Color", Color) = (0.3,0.3,0.3,1.0)
		
		//DIFFUSE
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		
		[Space(10)]
		[KeywordEnum(Texture, Range)] _RampType ("Ramp Type", Float) = 0
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		_RampThreshold ("Ramp Threshold", Range(0.01, 1)) = 0.5
		_RampSmoothing ("Ramp Smoothing", Range(0, 1)) = 0.1

		[Header(Specular)]
		[Toggle(SPECULAR)] _Specular ("", float) = 0
		_SpecularIntensity ("Specular Intensity", Range(0, 256)) = 16
		_SpecularSmooth ("Specular Smooth", Range(0, 0.5)) = 0
		_SpecularColor ("Specular Color", Color) = (1,1,1,0.6666)

		[Header(Highlight)]
		[Toggle(HIGHLIGHT)] _Highlight ("", float) = 0
		_HighlightScale ("Highlight Scale", Range(0, 1)) = 0.975
		_HighlightColor ("Highlight Color", Color) = (1,1,1,1)

		[Header(Emission)]
		[Toggle(EMISSION)] _Emission ("", float) = 0
		_EmissionColor ("Emission Color", Color) = (0,0,0,0)

		[Header(Global Illumination)]
		_GIStrength ("Strength", Range(0, 1)) = 1

		[Header(Outline)]
		_OutlineWidth ("Outline Width", Range(0, 1)) = 0.05
		_OutlineColor ("Outline Color", Color) = (0,0,0,1)
	}

	CustomEditor "ToonShaderEditor"
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		
		#pragma surface surf ToonyColorsCustom fullforwardshadows
		#pragma multi_compile_local _RAMPTYPE_TEXTURE _RAMPTYPE_RANGE
		#pragma multi_compile_local _ SPECULAR
		#pragma multi_compile_local _ HIGHLIGHT
		#pragma multi_compile_local _ EMISSION
		//#pragma target 2.0
		//#pragma glsl
		
		//================================================================
		// VARIABLES
		
		fixed4 _Color;
		sampler2D _MainTex;
		
		
		struct Input
		{
			half2 uv_MainTex;
		};
		
		//================================================================
		// CUSTOM LIGHTING
		
		//Lighting-related variables
		fixed4 _SColor;

		#if _RAMPTYPE_TEXTURE
			sampler2D _Ramp;
		#else
			float _RampThreshold;
			float _RampSmoothing;
		#endif

		#if SPECULAR
			float _SpecularIntensity;
			float _SpecularSmooth;
			float4 _SpecularColor;
		#endif

		#if HIGHLIGHT
			float4 _HighlightColor;
			float _HighlightScale;
		#endif

		half _GIStrength;
		
		//Custom SurfaceOutput
		struct SurfaceOutputCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			fixed3 Emission;
			half Specular;
			fixed Alpha;
		};
		
		inline half4 LightingToonyColorsCustom (SurfaceOutputCustom s, half3 lightDir, half3 viewDir, half atten)
		{
			s.Normal = normalize(s.Normal);
			fixed ndl = max(0, dot(s.Normal, lightDir)*0.5 + 0.5);
			
			#if _RAMPTYPE_TEXTURE
				fixed3 ramp = tex2D(_Ramp, fixed2(ndl,ndl));
			#else
				fixed3 ramp = float3(1, 1, 1) * smoothstep(_RampThreshold - _RampSmoothing * 0.5, _RampThreshold + _RampSmoothing * 0.5, ndl);
			#endif

			#if !(POINT) && !(SPOT)
				ramp *= atten;
			#endif

			#if SPECULAR
				float3 halfVector = normalize(lightDir + viewDir);
				float NdotH = saturate(dot(s.Normal, halfVector));
				float specularIntensity = smoothstep(0.5 - _SpecularSmooth, 0.5 + _SpecularSmooth, pow(NdotH * ramp, _SpecularIntensity));
				float3 specular = specularIntensity * _SpecularColor.rgb * _SpecularColor.a;
			#endif

			#if HIGHLIGHT
				float3 highlight = step(_HighlightScale, dot(lightDir, s.Normal)) * _HighlightColor.rgb * _HighlightColor.a;
			#endif

			_SColor = lerp(fixed4(1, 1, 1, 1), _SColor, _SColor.a);
			ramp = lerp(_SColor.rgb, fixed3(1, 1, 1), ramp);

			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * ramp;

			if (_GIStrength < 1)
			{
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
				gi.indirect.diffuse = 0;
				gi.indirect.specular = 0;
				gi.light.color = _LightColor0;
				gi.light.dir = lightDir;
				
				UnityGIInput giInput;
				UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
				giInput.light = gi.light;
				giInput.worldViewDir = viewDir;
				giInput.atten = atten;
				giInput.lightmapUV = 0.0;
				giInput.ambient.rgb = 0.0;
				
				gi = UnityGlobalIllumination(giInput, 1, s.Normal);
				gi.light.color = _LightColor0.rgb;

				c.rgb -= gi.indirect.diffuse * s.Albedo * (1 - _GIStrength);
			}

			#if SPECULAR
				c.rgb += _LightColor0.rgb * specular;
			#endif

			#if HIGHLIGHT
				c.rgb += highlight;
			#endif

			#if (POINT || SPOT)
				c.rgb *= atten;
			#endif

			c.a = s.Alpha;

			return c;
		}
		
		
		//================================================================
		// SURFACE FUNCTION
		
		half3 _EmissionColor;
		
		void surf (Input IN, inout SurfaceOutputCustom o)
		{
			fixed4 mainTex = tex2D(_MainTex, IN.uv_MainTex);

			#if EMISSION
			o.Emission = _EmissionColor;
			#endif
			
			o.Albedo = mainTex.rgb * _Color.rgb;
			o.Alpha = mainTex.a * _Color.a;	
		}
		
		ENDCG

		Pass {

			Cull Front

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};

			half _OutlineWidth;

			v2f vert(appdata v)
			{
				v2f o;
				float4 clipPosition = UnityObjectToClipPos(v.vertex);
				float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, v.normal));

				clipPosition.xyz += normalize(clipNormal) * _OutlineWidth;

				float2 offset = normalize(clipNormal.xy) / _ScreenParams.xy * _OutlineWidth * clipPosition.w * 2;
				clipPosition.xy += offset;

				o.vertex = clipPosition;
				return o;

				// position.xyz += normal * _OutlineWidth;

				// return UnityObjectToClipPos(position);
			}

			half4 _OutlineColor;

			half4 frag(v2f i) : SV_TARGET
			{				
				return _OutlineColor;
			}

			ENDCG

		}
	}
	
	Fallback "Diffuse"
}
