// Toony Colors Pro+Mobile Shaders
// (c) 2013,2014 Jean Moreno

Shader "Toony Colors Pro/Rim Outline/OneDirLight/Bumped Specular"
{
	Properties
	{
		_MainTex ("Base (RGB) Gloss (A) ", 2D) = "white" {}
		_BumpMap ("Normal map (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		
		//SPECULAR
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		
		//COLORS
		_Color ("Highlight Color", Color) = (0.8,0.8,0.8,1)
		_SColor ("Shadow Color", Color) = (0.0,0.0,0.0,1)
		
		//RIM OUTLINE
		_RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
		_RimMin ("Rim min", Range(0,1)) = 0.4
		_RimMax ("Rim max", Range(0,1)) = 0.6
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#include "TGP_Include.cginc"
		
		//nolightmap nodirlightmap		LIGHTMAP
		//approxview halfasview			SPECULAR/VIEW DIR
		//noforwardadd					ONLY 1 DIR LIGHT (OTHER LIGHTS AS VERTEX-LIT)
		#pragma surface surf ToonyColorsSpec nolightmap nodirlightmap noforwardadd approxview 
		#pragma exclude_renderers flash
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _RimColor;
		float _RimMin;
		float _RimMax;
		fixed _Shininess;
		
		struct Input
		{
			half2 uv_MainTex : TEXCOORD0;
			half2 uv_BumpMap : TEXCOORD1;
			float3 viewDir;
		};
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			//Specular
			o.Gloss = c.a;
			o.Specular = _Shininess;
			//Normal map
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			//Rim Outline
			half rim = 1.0f - saturate( dot(normalize(IN.viewDir), o.Normal) );
			rim = smoothstep(_RimMin, _RimMax, rim);
			o.Albedo = lerp(c.rgb, _RimColor, rim);
			
			o.Alpha = c.a;
		}
		ENDCG
	}
	
	Fallback "Toony Colors Pro/Rim Outline/OneDirLight/Basic"
}
