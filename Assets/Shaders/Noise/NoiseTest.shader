Shader "NoiseTest/HLSL/NoiseTest"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_ClipAmount ("Clip Amount", Range(0, 1)) = 0.5
		_Frequency ("Frequency", Float) = 2.0
		_EdgeColour1("Edge Colour 1", Color) = (1.0, 1.0, 1.0, 1.0)
		_Level("Dissolution Level", Range(0.0, 1.0)) = 0.1
		_Edges("Edge Width", Range(0.0, 1.0)) = 0.1
	}

		CGINCLUDE

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

	sampler2D _MainTex;
	float _ClipAmount;
	float _Frequency;
	fixed4 _EdgeColour1;
	float _Level;
	float _Edges;

    v2f_img vert(appdata_base v)
    {
		float bump = 0.5;

        v2f_img o;

		o.uv = v.texcoord.xy;

		//v.vertex.xyz = p;
		//v.vertex.y += sin(_Time.y * v.vertex.y);
		//v.vertex.x += cos(_Time.y * o.uv.x);

        o.pos = UnityObjectToClipPos(v.vertex);


        return o;
    }

    float4 frag(v2f_img i) : SV_Target
    {
        const float epsilon = 0.0001;

		//uv movement
		float2 uv = i.uv * 4.0 + float2(.2, 1) * _Time.y;
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
            float w = 0.5; //strength
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
                o += pnoise(coord, period) * w;
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

        #if defined(SNOISE_AGRAD) || defined(SNOISE_NGRAD)
            #if defined(THREED)
                return float4(o, 1);
            #else
                return float4(o, 1, 1);
            #endif
        #else
		//where everything is happening
			fixed4 texCol = tex2D(_MainTex, i.uv);
			float4 noiseCol = float4(o, o, o, 1);

			clip(noiseCol.r - _ClipAmount);


			if (_ClipAmount <= 0.01) 
			{

			}

			//edging stuff
			else if (noiseCol.r < _ClipAmount + _Level + _Edges)
				texCol = _EdgeColour1;



			//texCol.a = .5;
            return texCol;
        #endif
    }

    ENDCG
    SubShader
    {
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		LOD 100

		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
