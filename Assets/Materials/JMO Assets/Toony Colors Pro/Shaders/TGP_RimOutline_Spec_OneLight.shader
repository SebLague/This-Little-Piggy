// Toony Colors Pro+Mobile Shaders
// (c) 2013,2014 Jean Moreno

Shader "Toony Colors Pro/Rim Outline/OneDirLight/Specular"
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
		#pragma surface surf ToonyColorsSpec nolightmap nodirlightmap vertex:vert noforwardadd approxview halfasview 
		#pragma exclude_renderers flash
		
		sampler2D _MainTex;
		fixed4 _RimColor;
		float _RimMin;
		float _RimMax;
		fixed _Shininess;
		
		struct Input
		{
			half2 uv_MainTex : TEXCOORD0;
			fixed3 rim;
		};
		
		void vert (inout appdata_full v, out Input o)
		{
			#if defined(SHADER_API_D3D11) || defined(SHADER_API_D3D11_9X)
			UNITY_INITIALIZE_OUTPUT(Input,o);
			#endif
			
			o.rim = 1.0f - saturate( dot(normalize(ObjSpaceViewDir(v.vertex)), v.normal) );
		}
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			//Specular
			o.Gloss = c.a;
			o.Specular = _Shininess;
			//Rim Outline
			IN.rim = smoothstep(_RimMin, _RimMax, IN.rim);
			o.Albedo = lerp(c.rgb, _RimColor, IN.rim);
			
			o.Alpha = c.a;
		}
		ENDCG
	}
	
	Fallback "Toony Colors Pro/Rim Outline/OneDirLight/Basic"
}
