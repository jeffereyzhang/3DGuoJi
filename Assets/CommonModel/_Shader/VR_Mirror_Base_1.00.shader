//2012.03.09
//lightMap


Shader "VR_Mirror_Base" 
{
	Properties 
	{
	
		_SpecColor0("Specular Color", Color) = (0,0,0,1)
		_Shininess("Shininess", Range(0,1) ) = 0.5
		
//		_tile("Tile Factor", Float) = 1
		
		_Color ("Main Color", Color) = (0,0,0,1)
		_MainTex ("DiffuseMap", 2D) = "white" {}
		
		_brightness("Brightness", Range(1,5) ) = 1
		_contrast("Contrast", Range(1,2.5) ) = 1

		_lightmap_color("Lighting Color", Color) = (0,0,0,1)
		_LightMap ("SecondMap (CompleteMap or LightMap)", 2D) = "black" {}
		
		_changeType ("<<<HDR Type    ***    NoHDR>>>", Range(0,1)) =0
		
		_BumpMap ("Normalmap", 2D) = "bump" {}
		
//		_Cube ("Reflection Cubemap", Cube) = "black" {}


		_MirrorDistortion  ("Mirror Distortion", range (0,1)) = 0
		_reflect_blender("Reflect Blender", range (0,1)) = 0.5
		_fresnel_ctrl("Fresnel", range (-5,0)) = -1
		_FresnelBias ("Fresnel Bias", Range(0.015, 1)) = 0.1
		_ReflectionTex("Reflection Tex", 2D) = "black" {}
		
//		_Parallax ("Height", Range (-0.05, 0.05)) = 0.02
//		_relaxedcone_relief_map ("relaxedcone", 2D	) = "white" {}
		
//		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5

		_SelectColor ("Outline Color", Color) = (0.2,0.6,0.8,0)
		_SelectColorAlpha("Select Color Alpha",Range(0,1) ) = 0
		
	
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BlinnPhong_wjm  finalcolor:mycolor
		#pragma target 3.0
		#pragma debug
//		#pragma profileoption MaxTexIndirections=64

		float _GLOBALBRIGHTNESS;
		float _GLOBALCONTRASR;


		
		float4 _SpecColor0;
		
		float _Parallax;
		float _Shininess;
		float _brightness;
		float _contrast;
		float _MirrorDistortion;
		
		float _changeType;
		
		float4 _Color;
		float4 _lightmap_color;
		
		
		float4 screenPos;
		float _reflect_blender;
		float _fresnel_ctrl;
		float _FresnelBias;
		float _SelectColorAlpha;

		float4 _SelectColor;


		sampler2D _MainTex;
		sampler2D _BumpMap;
		sampler2D _LightMap;


		sampler2D _ReflectionTex;
		
		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
//			float2 uv_BumpMap;
			float2 uv2_LightMap;
//			float3 worldRefl;
			float3 viewDir;

			INTERNAL_DATA
		};

		struct SurfaceOutput_wjm 
		{
			half3 Albedo;
			float3 Normal;
			half3 Emission;
			half3 Gloss;
			half Specular;
			half Alpha;
		};
		
					void mycolor (Input IN, SurfaceOutput_wjm o, inout fixed4 color)
			{
				color += _SelectColor*_SelectColorAlpha;
			}


		
		inline half4 LightingBlinnPhong_wjm_PrePass (SurfaceOutput_wjm s, half4 light)
		{
			half3 spec = light.a * s.Gloss;
			half4 c;
			c.rgb = (s.Albedo * light.rgb + light.rgb * spec);
			c.a = s.Alpha;
			return c;
		}
		

inline float3 DecodeLightmap2( float4 color )
{
#if defined(SHADER_API_GLES) && defined(SHADER_API_MOBILE)
	return 2.0 * color.rgb;
#else
	// potentially faster to do the scalar multiplication
	// in parenthesis for scalar GPUs
//	return pow((8.0 * color.a),2.2) * color.rgb;
	return pow((_GLOBALBRIGHTNESS+_brightness)* lerp(8.0 * color.a,1,_changeType)* color.rgb,_contrast+_GLOBALCONTRASR);
	
#endif
}		
				
						
								
										
												
																
		inline half4 LightingBlinnPhong_wjm (SurfaceOutput_wjm s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 h = normalize (lightDir + viewDir);
			
			half diff = max (0, dot ( lightDir, s.Normal ));
				
			float nh = max (0, dot (s.Normal, h));
			float spec = pow (nh, s.Specular*128.0);
				
			half4 lightPower;
			lightPower.rgb = _LightColor0.rgb * diff;
			lightPower.w = spec * Luminance (_LightColor0.rgb);
			lightPower *= atten * 2.0;
				
			half4 c;
			c.rgb = (s.Albedo * lightPower.rgb + lightPower.rgb * spec*s.Gloss);
			c.a = s.Alpha;
			return c;
		}
		
		

	
		void surf (Input IN, inout SurfaceOutput_wjm o)
		{	


			o=(SurfaceOutput_wjm)0;
			
//			o.Normal = float3(0.0,0.0,1.0);
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex)).xyz;


			
			float2 mirrordistortionuv=0.1*o.Normal.xy*_MirrorDistortion;
			mirrordistortionuv=((IN.screenPos.xy/IN.screenPos.w).xyxy).xy+mirrordistortionuv;
			
			
			float4 Tex2D0=tex2D(_MainTex,IN.uv_MainTex);

			

//			float4 Tex2D1=pow(_brightness.xxxx +tex2D(_LightMap,IN.uv2_LightMap),_contrast.xxxx)- _brightness.xxxx;
			float4	Tex2D1=_lightmap_color*float4(DecodeLightmap2(tex2D(_LightMap,(IN.uv2_LightMap).xy)),1);


			float4 mirrorcolor=tex2D(_ReflectionTex,mirrordistortionuv);

			float fresnel_intensity=1-clamp(pow((max(0,dot (normalize(IN.viewDir), o.Normal))),-_fresnel_ctrl).xxxx,0,1);
  			fresnel_intensity = min(fresnel_intensity + _FresnelBias, 1.0f);
			
			float3 diffuseColor=Tex2D0.rgb*Tex2D1.rgb;
			
			float3 diffuseFinalcolor=lerp(diffuseColor,mirrorcolor.rgb,_reflect_blender*fresnel_intensity);
			
			o.Albedo=_Color.rgb*diffuseFinalcolor;
//			o.Albedo=_Color.rgb*diffuseColor;
			
			o.Emission=_lightmap_color.rrr*diffuseFinalcolor;
//			o.Emission=Tex2D0;
			o.Specular=_Shininess ;
			
			o.Gloss =Tex2D0.a*_SpecColor0.rgb;
			
			o.Alpha = Tex2D0.a * _Color.a;
			
			

			
			
				// border clamp
//float alpha=1;	

			
			
			
			
			
			o.Normal=normalize(o.Normal);

		}
	
		ENDCG
		
	} 
	FallBack "Diffuse"
}
