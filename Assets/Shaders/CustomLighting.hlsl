#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
#pragma multi_compile _ _ADDITIONAL_LIGHTS
#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
#pragma multi_compile _ _SHADOWS_SOFT

#ifndef SHADERGRAPH_PREVIEW
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#endif

void MainLight(in float3 WorldPos, out float3 Direction, out float3 LightColor, out float3 LightPos, out float DistanceAtten, out float ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = float3(0.5, 0.5, 0);
    LightPos = half3(0.5, 0.5, 0);
    LightColor = float3(0.5, 0.5, 0);
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    Light mainLight = GetMainLight(shadowCoord);
    
    Direction = mainLight.direction;
    LightColor = mainLight.color;
    LightPos = _MainLightPosition.xyz;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
#endif
}

void AdditionalLights(in float3 WorldPos, in float3 WorldNormal, out float3 AdditionalLightColor, out float3 AdditionalLightDirection)
{
    AdditionalLightColor = float3(0, 0, 0);
    AdditionalLightDirection = float3(0, 0, 0);

#ifndef SHADERGRAPH_PREVIEW
    int additionalLightsCount = GetAdditionalLightsCount();
    
    for (int i = 0; i < additionalLightsCount; i++)
    {
        Light light = GetAdditionalLight(i, WorldPos);
        
        // Calculate light contribution (you can modify this calculation)
        float lightIntensity = saturate(dot(WorldNormal, light.direction));
        
        AdditionalLightColor += light.color * light.distanceAttenuation * light.shadowAttenuation * lightIntensity;
        AdditionalLightDirection += light.direction;
    }
    
    // Normalize direction if you want an average direction
    if (additionalLightsCount > 0)
    {
        AdditionalLightDirection = normalize(AdditionalLightDirection);
    }
#endif
}

float CalculateSpecularHighlight(
    float3 WorldPos, 
    float3 WorldNormal, 
    float3 ViewDirection, 
    float Smoothness)
{
#ifndef SHADERGRAPH_PREVIEW
    Light mainLight = GetMainLight(TransformWorldToShadowCoord(WorldPos));
    
    // Blinn-Phong specular calculation
    float3 halfVector = normalize(mainLight.direction + ViewDirection);
    float NdotH = saturate(dot(WorldNormal, halfVector));
    
    // Specular intensity based on smoothness
    float specularIntensity = pow(NdotH, Smoothness * 256) * mainLight.shadowAttenuation;
    
    return specularIntensity;
#else
    return 0;
#endif
}


float3 CalculateRimLighting(
    float3 WorldPos, 
    float3 WorldNormal, 
    float3 ViewDirection, 
    float RimPower, 
    float RimIntensity)
{
#ifndef SHADERGRAPH_PREVIEW
    Light mainLight = GetMainLight(TransformWorldToShadowCoord(WorldPos));
    
    // Rim lighting calculation
    float rimDot = 1 - saturate(dot(WorldNormal, ViewDirection));
    float rimIntensity = pow(rimDot, RimPower) * RimIntensity;
    
    // Optional: Add light direction influence
    float lightInfluence = saturate(dot(WorldNormal, mainLight.direction));
    
    return mainLight.color * rimIntensity * mainLight.shadowAttenuation * lightInfluence;
#else
    return float3(0, 0, 0);
#endif
}

void AdvancedLighting(
    in float3 WorldPos, 
    in float3 WorldNormal, 
    in float3 ViewDirection,
    in float Smoothness,
    in float RimPower,
    in float RimIntensity,
    out float Highlight,
    out float3 RimLight)
{
#ifndef SHADERGRAPH_PREVIEW
    Light mainLight = GetMainLight(TransformWorldToShadowCoord(WorldPos));
    
    // Highlight calculation
    float3 halfVector = normalize(mainLight.direction + ViewDirection);
    float NdotH = saturate(dot(WorldNormal, halfVector));
    Highlight = pow(NdotH, Smoothness * 256) * mainLight.shadowAttenuation;
    
    // Rim lighting calculation
    float rimDot = 1 - saturate(dot(WorldNormal, ViewDirection));
    float rimIntensity = pow(rimDot, RimPower) * RimIntensity;
    float lightInfluence = saturate(dot(WorldNormal, mainLight.direction));
    
    RimLight = mainLight.color * rimIntensity * mainLight.shadowAttenuation * lightInfluence;
#else
    Highlight = 0;
    RimLight = float3(0, 0, 0);
#endif
}