#ifndef MY_TOON_SHADER_INCLUDE
#define MY_TOON_SHADER_INCLUDE

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"

// CONSTANT BUFFER
CBUFFER_START(UnityPerMaterial)
    TEXTURE2D(_ColorMap);
    SAMPLER(sampler_ColorMap);
    float4 _ColorMap_ST;
    float3 _Color;
    float _Smoothness;
    float _RimSharpness;
    float _RimColor;
    float3 _WorldColor;
CBUFFER_END

// STRUCTS

// Attributes contains object-space position, normal direction, uv0 coords of the mesh. Passed from the GPU to the vertex function
struct Attributes{
    float4 positionOS : POSITION;
    float3 normalOS : NORMAL;
    float2 uv: TEXCOORD0;

    UNITY_VERTEX_INPUT_INSTANCE_ID
};

// Varyings contains clip space position, uv, world-space normals. Passed from vertex to fragment function
struct Varyings{
    float4 positionHCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float3 positionWS : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    float3 viewDirectionWS : TEXCOORD3;

    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};


// LIGHTING TRANSFORMATIONS

float3 _LightDirection;

float4 GetClipSpacePosition(float3 positionWS, float3 normalWS){
    #if defined(SHADOW_CASTER_PASS)
        float4 positionHCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));

        #if UNITY_REVERSED_Z
            positionHCS.z = min(positionHCS.z, positionHCS.w * UNITY_NEAR_CLIP_VALUE);
        #else
            positionHCS.z = max(positionHCS.z, positionHCS.w * UNITY_NEAR_CLIP_VALUE);
        #endif

        return positionHCS;
    #endif

    return TransformWorldToHClip(positionWS);
}

// get shadow coords depending on whether screen shadows are turned on or not

float4 GetMainLightShadowCoord(float3 positionWS, float4 positionHCS){
    #if defined(_MAIN_LIGHT_SHADOWS_SCREEN)
           return ComputeScreenPos(positionHCS);
       #else
           return TransformWorldToShadowCoord(positionWS);
       #endif
}

float4 GetMainLightShadowCoord(float3 PositionWS)
{
       #if defined(_MAIN_LIGHT_SHADOWS_SCREEN)
           float4 clipPos = TransformWorldToHClip(PositionWS);
           return ComputeScreenPos(clipPos);
       #else
    return TransformWorldToShadowCoord(PositionWS);
       #endif
}

// get the main light 
void GetMainLightData(float3 PositionWS, out Light light)
{
       float4 shadowCoord = GetMainLightShadowCoord(PositionWS);
       light = GetMainLight(shadowCoord);
}

// FUNCTIONS

float easysmoothstep(float min, float x){
    return smoothstep(min, min + 0.01, x);
}

// generate mesh data
Varyings Vertex(Attributes IN){
    Varyings OUT = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

    OUT.positionWS = mul(unity_ObjectToWorld, IN.positionOS).xyz;
    OUT.viewDirectionWS = normalize(GetWorldSpaceViewDir(OUT.positionWS));
    OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
    OUT.positionHCS = GetClipSpacePosition(OUT.positionWS, OUT.normalWS);
    OUT.uv = TRANSFORM_TEX(IN.uv, _ColorMap);
    
    return OUT;
}
 
// handle per-pixel shading during DepthOnly and ShadowCaster pass
float FragmentDepthOnly(Varyings IN) : SV_Target{
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
       
    return 0;
}

// handle per-pixel shading during DepthNormalsOnly pass, for some post process effects
float4 FragmentDepthNormalsOnly(Varyings IN) : SV_Target{
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
       
    return float4(normalize(IN.normalWS), 0);
}

// handle per-pixel shading during ForwardOnly pass
float3 Fragment(Varyings IN) : SV_Target{
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
       
    IN.normalWS = normalize(IN.normalWS);
    IN.viewDirectionWS = normalize(IN.viewDirectionWS); 

    Light light;
    GetMainLightData(IN.positionWS, light);

    float NoL = dot(IN.normalWS, light.direction); // normals * light dir

    float toonLighting = easysmoothstep(0, NoL);
    float toonShadows = easysmoothstep(0.5, light.shadowAttenuation);

    float3 halfVector = normalize(light.direction + IN.viewDirectionWS);
    float NoH = max(dot(IN.normalWS, halfVector), 0); // normals * halfvector, NoH >= 0 always
    float specularTerm = pow(NoH, _Smoothness * _Smoothness);
    specularTerm *= toonLighting * toonShadows; // attenuate 
    specularTerm = easysmoothstep(0.01, specularTerm); // toon-ify

    float NoV = max(dot(IN.normalWS, IN.viewDirectionWS), 0); // normals * view direction
    float rimTerm = pow(1.0 - NoV, _RimSharpness);
    rimTerm *= toonLighting * toonShadows;
    rimTerm = easysmoothstep(0.01, rimTerm);

    float3 surfaceColor = _Color *  SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, IN.uv);

    float3 directionalLighting = toonLighting * toonShadows * light.color;
    float3 specularLighting = specularTerm * light.color;
    float3 rimLighting = rimTerm * _RimColor;

    float3 finalLighting = float3(0,0,0);
    finalLighting += directionalLighting;
    finalLighting += specularLighting;
    finalLighting += rimLighting;
    finalLighting += _WorldColor;

    return surfaceColor * finalLighting;
}
#endif