Shader "Custom/GrassGeneration"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (0, 0, 0, 1)
		_TipColor("Tip Color", Color) = (1, 1, 1, 1)
		_AOColor("Ambient Occlusion Color", Color) = (1, 1, 1, 1)
		_WindFrequency("Wind Frequency", float) = 1
		_WindOffsetMultiplier("Wind Offset Multiplier", float) = 1
		_WindSpeed("Wind Speed", float) = 1
		_NoiseTex("Noise Texture", 2D) = "white" {}
		_NoiseFrequency("Noise Frequency", float) = 1
		_NoiseAmplitude("Noise Amplitude", float) = 1
	}

	SubShader
	{
		Cull off
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
			"RenderPipeline" = "UniversalPipeline"
		}

		HLSLINCLUDE
			#define TWO_PI 6.28318530718f

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile_fog

			struct appdata
			{
				uint vertexID : SV_VertexID;
				uint instanceID : SV_InstanceID;
			};

			struct v2f
			{
				float4 positionCS : SV_Position;
				float3 positionWS : TEXCOORD0;
				float2 uv : TEXCOORD1;
				float fogCoord : TEXCOORD2;
				//float2 worldUV : TEXCOORD3; // Uncomment for Visualization of Wind Offset
			};

			struct GrassData
			{
				float3 positionWS;
				float2 worldUV;
			};

			StructuredBuffer<float3> _positions;
			StructuredBuffer<float2> _UVs;
			StructuredBuffer<GrassData> _culledGrassOutputBuffer;

			CBUFFER_START(UnityPerMaterial)
				float4 _BaseColor;
				float4 _TipColor;
				float4 _AOColor;

				float _WindOffsetMultiplier;
				float _WindFrequency;
				float _WindSpeed;

				sampler2D _NoiseTex;
				float4 _NoiseTex_ST;
				float _NoiseFrequency;
				float _NoiseAmplitude;
			CBUFFER_END

			float CalculateWindOffset(float2 uv, float2 worldUV)
			{
				float noiseValue = tex2Dlod(_NoiseTex, float4(worldUV.xy * _NoiseFrequency, 0, 0)).x * _NoiseAmplitude;
				float yUV = max(uv.y, 0);
				float windOffset = sin((worldUV.x + worldUV.y) * _WindFrequency + _Time.y * _WindSpeed + noiseValue) * yUV * yUV * _WindOffsetMultiplier;
				return windOffset;
			}

			float4x4 rotationMatrixY(float angle)
			{
				float s, c;
				sincos(angle, s, c);

				return float4x4
				(
					 c, 0, s, 0,
					 0, 1, 0, 0,
					-s, 0, c, 0,
					 0, 0, 0, 1
				);
			}

			float randomRange(float2 seed, float min, float max)
			{
				float randnum = frac(sin(dot(seed, float2(12.9898, 78.233)))*43758.5453);
				return lerp(min, max, randnum);
			}
		ENDHLSL

		Pass
		{
			Name "GrassPass"
			Tags { "LightMode" = "UniversalForward" }

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			v2f vert(appdata v)
			{
				v2f o;

				float4 positionOS = float4(_positions[v.vertexID], 1.0f);
				float3 objectToWorld = _culledGrassOutputBuffer[v.instanceID].positionWS;
				float2 uv = _UVs[v.vertexID];
				float2 worldUV = _culledGrassOutputBuffer[v.instanceID].worldUV;
				
				// Apply random rotation

				float randomRotation = randomRange(worldUV, 0, TWO_PI);
				positionOS = mul(rotationMatrixY(randomRotation), positionOS);

				float3 positionWS = positionOS.xyz + objectToWorld;
				float windOffset = CalculateWindOffset(uv, worldUV);
				positionWS.xz += windOffset;

				o.positionWS = positionWS;
				o.positionCS = mul(UNITY_MATRIX_VP, float4(positionWS, 1));
				o.uv = uv;
				o.fogCoord = ComputeFogFactor(o.positionCS.z);
				//o.worldUV = worldUV; // Uncomment for Visualization of Wind Offset
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 color = float4(1, 1, 1, 1);

//#ifdef _MAIN_LIGHT_SHADOWS
				VertexPositionInputs vertexInput = (VertexPositionInputs)0;
				vertexInput.positionWS = i.positionWS;

				float4 shadowCoord = GetShadowCoord(vertexInput);
				float shadowAttenuation = saturate(MainLightRealtimeShadow(shadowCoord) + 0.25f);
				float4 shadowColor = lerp(0.0f, 1.0f, shadowAttenuation);
				color *= shadowColor;
//#endif
				// Uncomment for Visualization of Wind Offset
				//float value = (CalculateWindOffset(1, i.worldUV) / _WindOffsetMultiplier + 1) / 2;
				//return float4(value, value, value, 1);
				
				float3 lightDir = GetMainLight().direction;
				float ndotl = dot(lightDir, normalize(float3(0, 1, 0)));
				float4 ao = lerp(_AOColor, 1.0f, i.uv.y);
				color *= lerp(_BaseColor, _TipColor, i.uv.y) * ndotl * ao;
				float fogCoord;

				fogCoord = i.fogCoord.x;

				color.rgb = MixFog(color.rgb, fogCoord);
				return color;
			}

            ENDHLSL
        }
    }
	Fallback Off
}
