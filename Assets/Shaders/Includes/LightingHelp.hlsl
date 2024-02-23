void GetMainLight_float(float3 WorldPos, out float3 Color, out float3 Direction, out float DistanceAtten, out float ShadowAtten)
{
#ifdef SHADERGRAPH_PREVIEW
    Direction = normalize(float3(0.5, 0.5, 0));
    Color = 1;
    DistanceAtten = 1;
    ShadowAtten = 1;
#else
#if SHADOWS_SCREEN
        float4 clipPos = TransformWorldToClip(WorldPos);
        float4 shadowCoord = ComputeScreenPos(clipPos);
#else
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif

    Light mainLight = GetMainLight(shadowCoord);
    Direction = mainLight.direction;
    Color = mainLight.color;
    DistanceAtten = mainLight.distanceAttenuation;
    ShadowAtten = mainLight.shadowAttenuation;
#endif
}

void ComputeAdditionalLighting_float(float3 WorldPosition, float3 WorldNormal,
    float2 Thresholds, float3 RampedDiffuseValues,
    out float3 Color, out float Diffuse)
{
    Color = float3(0, 0, 0);
    Diffuse = 0;

#ifndef SHADERGRAPH_PREVIEW

    uint pixelLightCount = GetAdditionalLightsCount();
    
    for (uint i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, WorldPosition);
        float4 tmp = unity_LightIndices[i / 4];
        uint light_i = tmp[i % 4];

        half shadowAtten = light.shadowAttenuation * AdditionalLightRealtimeShadow(light_i, WorldPosition, light.direction);
        
        half NdotL = saturate(dot(WorldNormal, light.direction));
        half distanceAtten = light.distanceAttenuation;

        half thisDiffuse = distanceAtten * shadowAtten * NdotL;
        
        half rampedDiffuse = 0;
        
        if (thisDiffuse < Thresholds.x)
        {
            rampedDiffuse = RampedDiffuseValues.x;
        }
        else if (thisDiffuse < Thresholds.y)
        {
            rampedDiffuse = RampedDiffuseValues.y;
        }
        else
        {
            rampedDiffuse = RampedDiffuseValues.z;
        }
        
        
        if (shadowAtten * NdotL == 0)
        {
            rampedDiffuse = 0;

        }
        
        if (light.distanceAttenuation <= 0)
        {
            rampedDiffuse = 0.0;
        }

        Color += max(rampedDiffuse, 0) * light.color.rgb;
        Diffuse += rampedDiffuse;
    }
#endif
}

void ChooseColor_float(float3 Highlight, float3 Midtone, float3 Shadow, float Diffuse, float2 Thresholds, out float3 OUT)
{
    if (Diffuse < Thresholds.x)
    {
        OUT = Shadow;
    }
    else if (Diffuse < Thresholds.y)
    {
        OUT = Midtone;
    }
    else
    {
        OUT = Highlight;
    }
}



void ChooseColorSmooth_float(float3 Highlight, float3 Midtone, float3 Shadow, float Diffuse, float2 Thresholds, float2 SmoothValues, out float3 OUT)
{
    //if (Diffuse < Thresholds.x)
    //{
    //    OUT = Shadow;
    //}
    //else if (Diffuse < Thresholds.y)
    //{
    //    OUT = Midtone;
    //}
    //else
    //{
    //    OUT = Highlight;
    //}
    
    //Either, lerp between highlight and midtone, or between midtone and shadow
    //how do we know?
    //I guess we don't know ...
    //But we can use 
    
    //This looks really good... I like how it's unapologetically lerps, and then the highlight is almost specular
    if (Diffuse < Thresholds.y)
    {
        OUT = lerp(Shadow, Midtone, Diffuse / Thresholds.y); //weight is diffuse / Thresholds.y
    }
    else //Diffuse >= Thresholds.y
    {
        OUT = lerp(Midtone, Highlight, (Diffuse - Thresholds.x));
    }
    
    
    //Lerp between 3 colors .. -> I.e. A gradient
    
    
    //lerp(x, y, w) w[0,1]
    
    
    //if (Diffuse < Thresholds.x - SmoothValues.x)
    //{
    //    OUT = Shadow;
    //}
    //else if (Diffuse < Thresholds.x + SmoothValues.x)
    //{
    //    //OUT = lerp(Shadow, Midtone, smoothstep(Thresholds.x - SmoothValues.x, Thresholds.x + SmoothValues.x, Diffuse));
    //    //OUT = lerp(Shadow, Midtone, (Diffuse - (Thresholds.x - SmoothValues.x)) / (SmoothValues.x * 2));
    //    OUT = lerp(Shadow, Midtone, 0.5);

    //}
    //else if (Diffuse < Thresholds.y - SmoothValues.y)
    //{
    //    OUT = Midtone;
    //}
    //else if (Diffuse < Thresholds.y + SmoothValues.y)
    //{
    //    //OUT = lerp(Midtone, Highlight, smoothstep(Thresholds.y - SmoothValues.y, Thresholds.y + SmoothValues.y, Diffuse));
    //    //OUT = lerp(Midtone, Highlight, (Diffuse - (Thresholds.y - SmoothValues.y)) / (SmoothValues.y * 2));
    //    OUT = lerp(Midtone, Highlight, 0.5);
    //}
    //else
    //{
    //    OUT = Highlight;
    //}
    
    
}



//    if (Diffuse < Thresholds.x - SmoothValues.x)
//    {
//        OUT =
//Shadow;
//    }
//    else if (Diffuse < Thresholds.x + SmoothValues.x)
//    {
//        OUT = lerp(Shadow, Midtone, smoothstep(Thresholds.x - SmoothValues.x, Thresholds.x + SmoothValues.x, Diffuse));

//    }
//    else if (Diffuse < Thresholds.y - SmoothValues.y)
//    {
//        OUT =
//Midtone;
//    }
//    else if (Diffuse < Thresholds.y + SmoothValues.y)
//    {
//        OUT = lerp(Midtone, Highlight, smoothstep(Thresholds.y - SmoothValues.y, Thresholds.y + SmoothValues.y, Diffuse));
//    }
//    else
//    {
//        OUT =
//Highlight;
//    }