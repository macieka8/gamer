Shader "Custom/GrassGeneration"
{
	Properties
	{
		_BaseColor("Base Color", Color) = (0, 0, 0, 1)
		_TipColor("Tip Color", Color) = (1, 1, 1, 1)
		_WindFrequency("Wind Frequency", float) = 1
		_WindOffsetMultiplier("Wind Offset Multiplier", float) = 1
		_WindSpeed("Wind Speed", float) = 1
		_NoiseTex("Noise Texture", 2D) = "white" {}
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
			};

			StructuredBuffer<float3> _normals;
			StructuredBuffer<float3> _positions;
			StructuredBuffer<float2> _UVs;
			StructuredBuffer<float4x4> _transformMatrices;
			StructuredBuffer<float2> _worldUVBuffer;

			CBUFFER_START(UnityPerMaterial)
				float4 _BaseColor;
				float4 _TipColor;
				float _WindOffsetMultiplier;
				float _WindFrequency;
				float _WindSpeed;
				sampler2D _NoiseTex;
				float4 _NoiseTex_ST;

				float _Cutoff;
			CBUFFER_END

			float2 CalculateWindOffset(float2 uv, float2 worldUV)
			{
				float windOffset = sin((worldUV.x + worldUV.y) * _WindFrequency + _Time * _WindSpeed) * tex2Dlod(_NoiseTex, float4(worldUV.xy, 0, 0)) * max(uv.y, 0) * _WindOffsetMultiplier;
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
				float4x4 objectToWorld = _transformMatrices[v.instanceID];
				float2 uv = _UVs[v.vertexID];
				float2 worldUV = _worldUVBuffer[v.instanceID];

				float4 positionWS = mul(objectToWorld, positionOS);
				float windOffset = CalculateWindOffset(uv, worldUV);
				positionWS.xz += windOffset;

				o.positionWS = positionWS;
				o.positionCS = mul(UNITY_MATRIX_VP, o.positionWS);
				o.uv = uv;
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
				return color * lerp(_BaseColor, _TipColor, i.uv.y);
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
				float4x4 objectToWorld = _transformMatrices[instanceID];
				float2 uv = _UVs[vertexID];
				float2 worldUV = _worldUVBuffer[instanceID];

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
