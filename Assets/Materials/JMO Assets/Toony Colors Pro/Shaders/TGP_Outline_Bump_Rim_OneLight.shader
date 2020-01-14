// Toony Colors Pro+Mobile Shaders
// (c) 2013,2014 Jean Moreno

Shader "Toony Colors Pro/Outline/OneDirLight/Bumped Rim"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_BumpMap ("Normal map (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		
		//COLORS
		_Color ("Highlight Color", Color) = (0.8,0.8,0.8,1)
		_SColor ("Shadow Color", Color) = (0.0,0.0,0.0,1)
		
		//RIM LIGHT
		_RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
		_RimPower ("Rim Power", Range(-2,10)) = 0.5
		
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
		#pragma surface surf ToonyColors nolightmap nodirlightmap noforwardadd approxview 
		
		sampler2D _MainTex;
		sampler2D _BumpMap;
		float _RimPower;
		fixed4 _RimColor;
		
		struct Input
		{
			half2 uv_MainTex : TEXCOORD0;
			half2 uv_BumpMap : TEXCOORD1;
			float3 viewDir;
		};
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = c.rgb;
			//Normal map
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
			//Rim Light
			half rim = 1.0f - saturate( dot(normalize(IN.viewDir), o.Normal) );
			o.Emission = (_RimColor.rgb * pow(rim, _RimPower)) * _RimColor.a;
			
			o.Alpha = c.a;
		}
		ENDCG
		UsePass "Hidden/ToonyColors-Outline/OUTLINE"
	}
	
	Fallback "Toony Colors Pro/Outline/OneDirLight/Basic Rim"
}
