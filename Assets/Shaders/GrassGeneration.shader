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
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Geometry"
			"RenderPipeline" = "UniversalPipeline"
		}

		HLSLINCLUDE
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _SHADOWS_SOFT

			struct appdata
			{
				uint vertexID : SV_VertexID;
				uint instanceID : SV_InstanceID;
			};

			struct v2f
			{
				float4 positionCS : SV_Position;
				float4 positionWS : TEXCOORD0;
				float2 uv : TEXCOORD1;
				//float2 worldUV : TEXCOORD2; // Uncomment for Visualization of Wind Offset
			};

			struct GrassData
			{
				float4x4 transformMatrix;
				float2 worldUV;
			};

			StructuredBuffer<float3> _normals;
			StructuredBuffer<float3> _positions;
			StructuredBuffer<float2> _UVs;
			StructuredBuffer<GrassData> _CulledGrassOutputBuffer;

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

				float _Cutoff;
			CBUFFER_END

			float2 CalculateWindOffset(float2 uv, float2 worldUV)
			{
				float noiseValue = tex2Dlod(_NoiseTex, float4(worldUV.xy * _NoiseFrequency, 0, 0)) * _NoiseAmplitude;
				float windOffset = sin((worldUV.x + worldUV.y) * _WindFrequency + _Time * _WindSpeed + noiseValue) * max(uv.y, 0) * max(uv.y, 0) * _WindOffsetMultiplier;
				return windOffset.xx;
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
				float4x4 objectToWorld = _CulledGrassOutputBuffer[v.instanceID].transformMatrix;
				float2 uv = _UVs[v.vertexID];
				float2 worldUV = _CulledGrassOutputBuffer[v.instanceID].worldUV;

				float4 positionWS = mul(objectToWorld, positionOS);
				float windOffset = CalculateWindOffset(uv, worldUV);
				positionWS.xz += windOffset;

				o.positionWS = positionWS;
				o.positionCS = mul(UNITY_MATRIX_VP, o.positionWS);
				o.uv = uv;
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
				//float texNoise = tex2Dlod(_NoiseTex, float4(i.worldUV.xy * _NoiseFrequency, 0, 0)) * _NoiseAmplitude;
				//float value = (sin((i.worldUV.x + i.worldUV.y) * _WindFrequency + _Time * _WindSpeed + texNoise) + 1) / 2;
				//return float4(value, value, value, 1);
				
				float3 lightDir = GetMainLight().direction;
				float ndotl = dot(lightDir, normalize(float3(0, 1, 0)));
				float4 ao = lerp(_AOColor, 1.0f, i.uv.y);
				return color * lerp(_BaseColor, _TipColor, i.uv.y) * ndotl * ao;
			}

            ENDHLSL
        }

		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			ZWrite On
			ZTest LEqual

			HLSLPROGRAM
			#pragma vertex shadowVert
			#pragma fragment shadowFrag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

			float3 _LightDirection;
			float3 _LightPosition;

			v2f shadowVert(uint vertexID : SV_VertexID, uint instanceID : SV_InstanceID)
			{
				v2f o;

				float4 positionOS = float4(_positions[vertexID], 1.0f);
				float3 normalOS = _normals[vertexID];
				float4x4 objectToWorld = _CulledGrassOutputBuffer[instanceID].transformMatrix;
				float2 uv = _UVs[vertexID];
				float2 worldUV = _CulledGrassOutputBuffer[instanceID].worldUV;

				float4 positionWS = mul(objectToWorld, positionOS);

				positionWS.xz += CalculateWindOffset(uv, worldUV);
				o.positionCS = mul(UNITY_MATRIX_VP, positionWS);
				o.uv = uv;

				float3 normalWS = TransformObjectToWorldNormal(normalOS);

				// Code required to account for shadow bias.
#if _CASTING_PUNCTUAL_LIGHT_SHADOW
				float3 lightDirectionWS = normalize(_LightPosition - positionWS);
#else
				float3 lightDirectionWS = _LightDirection;
#endif
				o.positionWS = float4(ApplyShadowBias(positionWS, normalWS, lightDirectionWS), 1.0f);

				return o;
			}

			float4 shadowFrag(v2f i) : SV_Target
			{
				return 0;
			}

			ENDHLSL
		}
    }
	Fallback Off
}
