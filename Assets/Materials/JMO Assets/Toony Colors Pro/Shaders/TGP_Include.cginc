// Toony Colors Pro+Mobile Shaders
// (c) 2013,2014 Jean Moreno

#ifndef TOONYCOLORS_INCLUDED
	#define TOONYCOLORS_INCLUDED
	
	//Lighting Ramp
	sampler2D _Ramp;
	
	//Highlight/Shadow Colors
	fixed4 _Color;
	fixed4 _SColor;
	
#endif

// TOONY COLORS
#pragma lighting ToonRamp exclude_path:prepass
inline half4 LightingToonyColors (SurfaceOutput s, half3 lightDir, half atten)
{
	#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = normalize(lightDir);
	#endif
	
	//Ramp shading
	fixed ndl = dot(s.Normal, lightDir)*0.5 + 0.5;
	fixed3 ramp = tex2D(_Ramp, fixed2(ndl,ndl));
	
	//Gooch shading
	ramp = lerp(_SColor,_Color,ramp);
	
	fixed4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
	c.a = s.Alpha;
	
	return c;
}

// TOONY COLORS + SPECULAR
#pragma lighting ToonRamp exclude_path:prepass
inline half4 LightingToonyColorsSpec (SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
{
	fixed diff = max (0, dot (s.Normal, lightDir)*0.5 + 0.5);
	
	//Ramp shading
	fixed3 ramp = tex2D(_Ramp, fixed2(diff,diff));
	
	//Gooch shading
	ramp = lerp(_SColor,_Color,ramp);
	
	//Specular
	half3 h = normalize (lightDir + viewDir);
	float ndh = max (0, dot (s.Normal, h));
	float spec = pow (ndh, s.Specular*128.0) * s.Gloss;
	
	fixed4 c;
	c.rgb = (s.Albedo * _LightColor0.rgb * ramp + _LightColor0.rgb * _SpecColor.rgb * spec) * (atten * 2);
	c.a = s.Alpha + _LightColor0.a * _SpecColor.a * spec * atten;
	
	return c;
}