// Toony Colors Pro+Mobile Shaders
// (c) 2013,2014 Jean Moreno

Shader "Toony Colors Pro/OutlineConst/OneDirLight/Specular"
{
	Properties
	{
		_MainTex ("Base (RGB) Gloss (A) ", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		
		//SPECULAR
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
		
		//COLORS
		_Color ("Highlight Color", Color) = (0.8,0.8,0.8,1)
		_SColor ("Shadow Color", Color) = (0.0,0.0,0.0,1)
		
		//OUTLINE
		_Outline ("Outline Width", Range(0,0.05)) = 0.005
		_OutlineColor ("Outline Color", Color) = (0.2, 0.2, 0.2, 1)
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
		#pragma surface surf ToonyColorsSpec nolightmap nodirlightmap noforwardadd approxview halfasview 
		
		sampler2D _MainTex;
		fixed _Shininess;
		
		struct Input
		{
			half2 uv_MainTex : TEXCOORD0;
		};
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = c.rgb;
			//Specular
			o.Gloss = c.a;
			o.Specular = _Shininess;
			
			o.Alpha = c.a;
		}
		ENDCG
		UsePass "Hidden/ToonyColors-Outline/OUTLINE_CONST"
	}
	
	Fallback "Toony Colors Pro/OutlineConst/OneDirLight/Basic"
}
