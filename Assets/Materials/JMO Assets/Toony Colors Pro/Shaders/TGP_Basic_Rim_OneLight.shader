// Toony Colors Pro+Mobile Shaders
// (c) 2013,2014 Jean Moreno

Shader "Toony Colors Pro/Normal/OneDirLight/Basic Rim"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {}
		
		//COLORS
		_Color ("Highlight Color", Color) = (0.8,0.8,0.8,1)
		_SColor ("Shadow Color", Color) = (0.0,0.0,0.0,1)
		
		//RIM LIGHT
		_RimColor ("Rim Color", Color) = (0.8,0.8,0.8,0.6)
		_RimPower ("Rim Power", Range(-2,10)) = 0.5
		
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
		#pragma surface surf ToonyColors nolightmap nodirlightmap vertex:vert noforwardadd approxview 
		
		sampler2D _MainTex;
		float _RimPower;
		fixed4 _RimColor;
		
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
			
			half rim = 1.0f - saturate( dot(normalize(ObjSpaceViewDir(v.vertex)), v.normal) );
			o.rim = (_RimColor.rgb * pow(rim, _RimPower)) * _RimColor.a;
		}
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			
			o.Albedo = c.rgb;
			//Rim Light
			o.Emission = IN.rim;
			
			o.Alpha = c.a;
		}
		ENDCG
	}
	
	Fallback "Toony Colors Pro/Normal/OneDirLight/Basic"
}
