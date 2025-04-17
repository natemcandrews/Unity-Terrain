#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// Textures

float _Smoothness;
float _Metallic;

uniform float _minHeight;
uniform float _maxHeight;

const static int maxColorCount = 8;

int _baseColorCount;
float3 _baseColors[maxColorCount];
float _baseStartHeights[maxColorCount];

struct Attributes {
	float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
	float2 uv : TEXCOORD0;
};

struct Interpolators {
	float4 positionCS : SV_POSITION;
	float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
};

Interpolators Vertex(Attributes input) {
	Interpolators output;

	VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS);
    VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS);

	output.positionCS = posnInputs.positionCS;
    output.normalWS = normInputs.normalWS;
    output.positionWS = posnInputs.positionWS;

	return output;
}

float inverseLerp(float a, float b, float value)
{
    return saturate((value - a) / (b - a));
}


float4 Fragment(Interpolators input) : SV_TARGET {
	float2 uv = input.uv;

	InputData lightingInput = (InputData)0;
    lightingInput.positionWS = input.positionWS;
    lightingInput.normalWS = normalize(input.normalWS);
    lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
    lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS);

	SurfaceData surfaceInput = (SurfaceData)0;

    surfaceInput.specular = 1;
    surfaceInput.smoothness = _Smoothness;
    surfaceInput.metallic = _Metallic;

    float heightPercent = inverseLerp(_minHeight, _maxHeight, input.positionWS.y);
    for(int i = 0; i < _baseColorCount; i++)
    {
        float drawStrength = saturate(sign(heightPercent - _baseStartHeights[i]));
        surfaceInput.albedo = surfaceInput.albedo * (1 - drawStrength) + _baseColors[i] * drawStrength;
    }

	return UniversalFragmentPBR(lightingInput, surfaceInput);
}