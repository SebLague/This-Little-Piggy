// Toony Colors Pro+Mobile Shaders
// (c) 2013,2014 Jean Moreno

Shader "Toony Colors Pro/Rim Outline/OneDirLight/Bumped"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normal map (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		
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
		//noforwardadd					ONLY 1 DIR LIGHT (OTHER LIGHTS AS VERTEX-LIT)
		#pragma surface surf ToonyColors nolightmap nodirlightmap noforwardadd 
		#pragma exclude_renderers flash
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		fixed4 _RimColor;
		float _RimMin;
		float _RimMax;
		
		struct Input
		{
			half2 uv_MainTex : TEXCOORD0;
			half2 uv_BumpMap : TEXCOORD1;
			float3 viewDir;
		};
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			
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
