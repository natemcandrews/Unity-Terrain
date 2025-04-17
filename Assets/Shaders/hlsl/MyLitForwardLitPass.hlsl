#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// Textures

float _smoothness;
float _metallic;

uniform float _minHeight;
uniform float _maxHeight;

const static int maxLayerCount = 8;
const static float epsilon = 1E-4;

int _layerCount;
float3 _baseColors[maxLayerCount];
float _baseStartHeights[maxLayerCount];
float _baseBlends[maxLayerCount];
float _baseColorStrength[maxLayerCount];
float _baseTextureScales[maxLayerCount];

TEXTURE2D(testTexture); SAMPLER(sampler_testTexture);
float testScale;

struct Attributes {
	float3 positionOS : POSITION;
    float3 normalOS : NORMAL;
	float2 uv : TEXCOORD0;
};

struct Interpolators {
    float4 positionCS : SV_POSITION;
    float3 positionWS : TEXCOORD0;
    float3 normalWS : TEXCOORD1;
    float2 xUV : TEXCOORD2;
    float2 yUV : TEXCOORD3;
    float2 zUV : TEXCOORD4;
    float3 blendWeights : TEXCOORD5;
};

Interpolators Vertex(Attributes input) {
	Interpolators output;

	VertexPositionInputs posnInputs = GetVertexPositionInputs(input.positionOS);
    VertexNormalInputs normInputs = GetVertexNormalInputs(input.normalOS);

	output.positionCS = posnInputs.positionCS;
    output.normalWS = normInputs.normalWS;
    output.positionWS = posnInputs.positionWS;

    float3 blend = abs(normInputs.normalWS);
    output.blendWeights = normalize(blend);

    output.xUV = posnInputs.positionWS.zy / testScale;
    output.yUV = posnInputs.positionWS.xz / testScale;
    output.zUV = posnInputs.positionWS.xy / testScale; 

	return output;
}

float inverseLerp(float a, float b, float value)
{
    return saturate((value - a) / (b - a));
}


float4 Fragment(Interpolators input) : SV_TARGET {
	InputData lightingInput = (InputData)0;
    lightingInput.positionWS = input.positionWS;
    lightingInput.normalWS = normalize(input.normalWS);
    lightingInput.viewDirectionWS = GetWorldSpaceNormalizeViewDir(input.positionWS);
    lightingInput.shadowCoord = TransformWorldToShadowCoord(input.positionWS);

	SurfaceData surfaceInput = (SurfaceData)0;

    surfaceInput.specular = 1;
    surfaceInput.smoothness = _smoothness;
    surfaceInput.metallic = _metallic;

    float heightPercent = inverseLerp(_minHeight, _maxHeight, input.positionWS.y);
    for(int i = 0; i < _layerCount; i++)
    {
        float drawStrength = inverseLerp(-_baseBlends[i] / 2 - epsilon, _baseBlends[i] / 2 - epsilon, heightPercent - _baseStartHeights[i]);
        surfaceInput.albedo = surfaceInput.albedo * (1 - drawStrength) + _baseColors[i] * drawStrength;
    }

    float3 xTex = SAMPLE_TEXTURE2D(testTexture, sampler_testTexture, input.xUV).rgb;
    float3 yTex = SAMPLE_TEXTURE2D(testTexture, sampler_testTexture, input.yUV).rgb;
    float3 zTex = SAMPLE_TEXTURE2D(testTexture, sampler_testTexture, input.zUV).rgb;
    
    input.blendWeights /= input.blendWeights.x + input.blendWeights.y + input.blendWeights.z;

    float3 finalColor = xTex * input.blendWeights.x +
                        yTex * input.blendWeights.y +
                        zTex * input.blendWeights.z;
    
    // surfaceInput.albedo = finalColor;

	return UniversalFragmentPBR(lightingInput, surfaceInput);
}