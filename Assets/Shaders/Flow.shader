Shader "Custom/Flow" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_FlowTex ("Flow", 2D) = "white" {}
		_FlowSpeed("FlowSpeed", Float) = 0.3
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _FlowTex;

		struct Input {
			float2 uv_MainTex;
			float2 uv_FlowTex;
		};

		float _FlowSpeed;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {

			//get flow of tex
			fixed4 flow = tex2D(_FlowTex, IN.uv_FlowTex);

			float flowR = flow.r;
			float flowG = flow.g;

			//float flowG = 1;
			//float flowR = 1;

			//flow inbetween 0 and 1
			float xVal = ((flowR * 2.0f) - 1.0f);
			float yVal = ((flowG * 2.0f) - 1.0f);

			float2 uvDisplacement = float2(xVal, yVal);

			//IN.uv_MainTex.y += (flow.r - 0.22) * _Time.y;


			//IN.uv_MainTex.x += _Time.y;
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex + uvDisplacement) * _Color;

			
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
