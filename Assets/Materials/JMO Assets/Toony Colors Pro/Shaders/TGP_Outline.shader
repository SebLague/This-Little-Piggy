// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

// Toony Colors Pro+Mobile Shaders
// (c) 2013,2014 Jean Moreno

Shader "Hidden/ToonyColors-Outline"
{
	Properties
	{
		//OUTLINE
		_Outline ("Outline Width", Float) = 1.0
		_OutlineColor ("Outline Color", Color) = (0.2, 0.2, 0.2, 1)
		
		//Z CORRECT
		_ZSmooth ("Z Correction", Range(-3.0,3.0)) = -0.5
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		//Outline default
		Pass
		{
			Name "OUTLINE"
			
			Cull Front
			Lighting Off
//			ZWrite Off
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
//			#define SMOOTH_Z_ARTEFACTS
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f
			{
				float4 pos : POSITION;
			};
			
			float _Outline;
			sampler2D _MainTex;
			fixed4 _OutlineColor;
			float4 _MainTex_ST;
			
			v2f vert (a2v v)
			{
				v2f o;
				
			#ifdef SMOOTH_Z_ARTEFACTS
				//Correct Z artefacts
				float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
				float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
				normal.z = -0.6;
				
				//Camera-independent size
				#ifdef OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					pos = pos + float4(normalize(normal),0) * _Outline * dist * 1.0;
				#else
					pos = pos + float4(normalize(normal),0) * _Outline;
				#endif
				
			#else
				#ifdef OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline * dist * 1.0);
				#else
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline);
				#endif
			#endif
				
				o.pos = mul(UNITY_MATRIX_P, pos);
				return o;
			}
			
			float4 frag (v2f IN) : COLOR
			{
				return _OutlineColor;
			}
		ENDCG
		}
		
		//Outline Const Size
		Pass
		{
			Name "OUTLINE_CONST"
			
			Cull Front
			Lighting Off
//			ZWrite Off
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#define OUTLINE_CONST_SIZE
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f
			{
				float4 pos : POSITION;
			};
			
			float _Outline;
			sampler2D _MainTex;
			fixed4 _OutlineColor;
			float4 _MainTex_ST;
			
			v2f vert (a2v v)
			{
				v2f o;
				
			#ifdef SMOOTH_Z_ARTEFACTS
				//Correct Z artefacts
				float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
				float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
				normal.z = -1;
				
				//Camera-independent size
				#ifdef OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					pos = pos + float4(normalize(normal),0) * _Outline * dist * 1.0;
				#else
					pos = pos + float4(normalize(normal),0) * _Outline;
				#endif
				
			#else
				#ifdef OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline * dist * 1.0);
				#else
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline);
				#endif
			#endif
				
				o.pos = mul(UNITY_MATRIX_P, pos);
				return o;
			}
			
			float4 frag (v2f IN) : COLOR
			{
				return _OutlineColor;
			}
		ENDCG
		}
		
		//Outline - Z Correct
		Pass
		{
			Name "OUTLINE_Z"
			
			Cull Front
			Lighting Off
//			ZWrite Off
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#define SMOOTH_Z_ARTEFACTS
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			}; 
			
			struct v2f
			{
				float4 pos : POSITION;
			};
			
			float _Outline;
			sampler2D _MainTex;
			fixed4 _OutlineColor;
			float4 _MainTex_ST;
			
			#ifdef SMOOTH_Z_ARTEFACTS
				fixed _ZSmooth;
			#endif
			
			v2f vert (a2v v)
			{
				v2f o;
				
			#ifdef SMOOTH_Z_ARTEFACTS
				//Correct Z artefacts
				float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
				float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
				normal.z = _ZSmooth;
				
				//Camera-independent size
				#ifdef OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					pos = pos + float4(normalize(normal),0) * _Outline * dist * 1.0;
				#else
					pos = pos + float4(normalize(normal),0) * _Outline;
				#endif
				
			#else
				#ifdef OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline * dist * 1.0);
				#else
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline);
				#endif
			#endif
				
				o.pos = mul(UNITY_MATRIX_P, pos);
				return o;
			}
			
			float4 frag (v2f IN) : COLOR
			{
				return _OutlineColor;
			}
		ENDCG
		}
		
		//Outline Const Size - Z Correct
		Pass
		{
			Name "OUTLINE_CONST_Z"
			
			Cull Front
			Lighting Off
//			ZWrite Off
			Tags { "LightMode"="ForwardBase" }
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#define OUTLINE_CONST_SIZE
			#define SMOOTH_Z_ARTEFACTS
			
			struct a2v
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			
			struct v2f
			{
				float4 pos : POSITION;
			};
			
			float _Outline;
			sampler2D _MainTex;
			fixed4 _OutlineColor;
			float4 _MainTex_ST;
			
			#ifdef SMOOTH_Z_ARTEFACTS
				fixed _ZSmooth;
			#endif
			
			v2f vert (a2v v)
			{
				v2f o;
				
			#ifdef SMOOTH_Z_ARTEFACTS
				//Correct Z artefacts
				float4 pos = mul( UNITY_MATRIX_MV, v.vertex);
				float3 normal = mul( (float3x3)UNITY_MATRIX_IT_MV, v.normal);
				normal.z = _ZSmooth;
				
				//Camera-independent size
				#ifdef OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					pos = pos + float4(normalize(normal),0) * _Outline * dist * 1.0;
				#else
					pos = pos + float4(normalize(normal),0) * _Outline;
				#endif
				
			#else
				#ifdef OUTLINE_CONST_SIZE
					float dist = distance(_WorldSpaceCameraPos, mul(unity_ObjectToWorld, v.vertex));
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline * dist * 1.0);
				#else
					float4 pos = mul( UNITY_MATRIX_MV, v.vertex + float4(v.normal,0) * _Outline);
				#endif
			#endif
				
				o.pos = mul(UNITY_MATRIX_P, pos);
				return o;
			}
			
			float4 frag (v2f IN) : COLOR
			{
				return _OutlineColor;
			}
		ENDCG
		}
	}
}
