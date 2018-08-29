Shader "Custom/MistyOutine" {


	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}

		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Specular("Specular", Range(0,1)) = 0.0
		_FresnelExponent("Fresnel Exponent", Range(0.01, 3)) = 1
		_Opacity("Opacity", Range(0,1)) = 0.75

		//Noise Properties
		_Frequency("Frequency", Range(0, 2)) = 1
		_Strength("Strength", Range(0, 2)) = 1
		_Speed("Speed", Range(0, 2)) = 1
	}


	SubShader {
		Tags{ "RenderType" = "Transparent" "Queue" = "Transparent" }
			LOD 200
			/*Pass{
			ColorMask 0
		}*/
			// Render normally

			Cull Front
			ZWrite On
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB

		CGPROGRAM


		#pragma multi_compile CNOISE PNOISE SNOISE SNOISE_AGRAD SNOISE_NGRAD
		#pragma multi_compile _ THREED
		#pragma multi_compile _ FRACTAL

		#include "UnityCG.cginc"


    #if !defined(CNOISE)
        #if defined(THREED)
            #include "SimplexNoise3D.hlsl"
        #else
            #include "SimplexNoise2D.hlsl"
        #endif
    #else
        #if defined(THREED)
            #include "ClassicNoise3D.hlsl"
        #else
            #include "ClassicNoise2D.hlsl"
        #endif
    #endif
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf StandardSpecular fullforwardshadows alpha
 
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMap;

		struct Input {
			float2 uv_MainTex;
			float2 uv_BumpMap;
			float3 viewDir; 
		};

		half _Glossiness;
		half _Specular;
		fixed4 _Color;
		fixed4 _RimColor;
		float _Opacity;
		float _FresnelExponent;

		float _Frequency;
		float _Strength;
		float _Speed;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)


		float4 GenerateNoiseColor(Input IN)
		{
				//Noise
			const float epsilon = 0.0001;

		//uv movement
		float2 uv = IN.uv_MainTex * 4.0 + float2(.2, 1) * _Time.y * _Speed;
		//float2 uv = i.uv * _Time.y;

        #if defined(SNOISE_AGRAD) || defined(SNOISE_NGRAD)
            #if defined(THREED)
                float3 o = 0.5;
            #else
                float2 o = 0.5;
            #endif
        #else
			float o = 0.5; //contrast kinda brightness
        #endif

        float s = _Frequency; //frequency

        #if defined(SNOISE)
            float w = 0.25;
        #else
            float w = _Strength; //strength
        #endif

        #ifdef FRACTAL
        for (int i = 0; i < 6; i++)
        #endif
        {
            #if defined(THREED)
                float3 coord = float3(uv * s, _Time.y);
                float3 period = float3(s, s, 1.0) * 2.0;
            #else
                float2 coord = uv * s;
                float2 period = s * 2.0;
            #endif

				//we are using CNOISE
            #if defined(CNOISE)
                o += cnoise(coord) * w;
            #elif defined(PNOISE)
                //o += pnoise(coord, period) * w;
            #elif defined(SNOISE)
                o += snoise(coord) * w;
            #elif defined(SNOISE_AGRAD)
                o += snoise_grad(coord) * w;
            #else // SNOISE_NGRAD
                #if defined(THREED)
                    float v0 = snoise(coord);
                    float vx = snoise(coord + float3(epsilon, 0, 0));
                    float vy = snoise(coord + float3(0, epsilon, 0));
                    float vz = snoise(coord + float3(0, 0, epsilon));
                    o += w * float3(vx - v0, vy - v0, vz - v0) / epsilon;
                #else
                    float v0 = snoise(coord);
                    float vx = snoise(coord + float2(epsilon, 0));
                    float vy = snoise(coord + float2(0, epsilon));
                    o += w * float2(vx - v0, vy - v0) / epsilon;
                #endif
            #endif

            s *= 2.0;
            w *= 0.5;
        }

		float4 noiseCol = float4(0, 0, 0, 0);

        #if defined(SNOISE_AGRAD) || defined(SNOISE_NGRAD)
            #if defined(THREED)
                //return float4(o, 1);
				noiseCol = float4(o, 1);
            #else
                //return float4(o, 1, 1);
				noiseCol = float4(o, 1, 1);
            #endif
        #else
		//where everything is happening
			noiseCol = float4(o, o, o, o);
        #endif
		
			return noiseCol;
		}

		void surf (Input IN, inout SurfaceOutputStandardSpecular surface)
		{		
			float4 noiseCol = GenerateNoiseColor(IN);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			surface.Albedo = c.rgb;

			surface.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			half factor = dot(normalize(IN.viewDir), surface.Normal);

			// Metallic and smoothness come from slider variables
			surface.Specular = _Specular;
			surface.Smoothness = _Glossiness;

			//surface.Alpha = (factor * _FresnelExponent) + _Opacity; //this made a snowglobe kind of effect
			//surface.Albedo = (factor * _FresnelExponent) * _RimColor + surface.Albedo;

			//get lerp value from dot product then mulitply by negative one because dot product is negative
			float lerpVal = clamp(0, 1, factor * _FresnelExponent * -1);
			fixed4 endColor = lerp(_RimColor * noiseCol, c, lerpVal);

			surface.Albedo = endColor;
			surface.Alpha = endColor.a;
		}
		
		ENDCG
	}
	FallBack "Diffuse"
}
