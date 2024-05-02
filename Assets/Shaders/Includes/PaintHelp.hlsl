void FoliageInjectSetup_float(float3 A, out float3 Out) 
{
	Out = A;
}

#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

struct MeshProperties {
    float4x4 mat;
    float2 UV;
    float4 color;
    float scaleFactor;
};


#if defined(SHADER_API_GLCORE) || defined(SHADER_API_D3D11) || defined(SHADER_API_GLES3) || defined(SHADER_API_METAL) || defined(SHADER_API_VULKAN) || defined(SHADER_API_PSSL) || defined(SHADER_API_XBOXONE)
uniform StructuredBuffer<MeshProperties> _Properties;
#endif

#endif

void setup()
{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

#ifdef unity_ObjectToWorld
#undef unity_ObjectToWorld
#endif
    
#ifdef unity_WorldToCamera
#undef unity_WorldToCamera
#endif

#ifdef unity_CameraToWorld
#undef unity_CameraToWorld
#endif
    
	unity_ObjectToWorld = _Properties[unity_InstanceID].mat;
#endif
}


void GetColor_float(out float4 OUT)
{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
    OUT = _Properties[unity_InstanceID].color;
#else
    OUT = float4(1, 0, 0, 1);
#endif
}

void GetScaleFactor_float(out float OUT)
{
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
    OUT = _Properties[unity_InstanceID].scaleFactor;
#else
    OUT = 1.0f;
#endif
}
//void FoliageNum_float(out int FoliageNum) {
//    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//    int x = floor(_Properties[unity_InstanceID].seed1 + 0.0);
//    //if x is 0, then unique grass shape (first 5)
//    FoliageNum = x * floor(5.0 + _Properties[unity_InstanceID].seed2 * 9.999) + (1-x) * floor(_Properties[unity_InstanceID].seed2 * 4.999);

//    //FoliageNum = 2;
//    #else
//    FoliageNum = 0;
//    #endif
//}

//void FoliageRandomRadiance_float(float IN, out float OUT) {
//    #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//    int x = floor(_Properties[unity_InstanceID].seed1 + 0.800);
//    OUT = x * IN + (1-x) * _Properties[unity_InstanceID].seed2;
//    #else
//    OUT = IN;
//    #endif
//}

/*
void YFrameUVAnimate_float(float UVYCoord, float WorldHeight, float2 TerrainUV, out float OUT) {
    float uvDir = (TerrainUV.x * 1.0 + TerrainUV.y * 0.0);
    float time = _Time * 12.0;
    float input = time + 30.0 * uvDir;

    float bracket = floor(frac(input) * 4.0);

    //float div = 3.0 + sin(input * 0.5) * 2.0;
    float div = 0.0;
    if (floor(fmod(input, div)) > 0) {
        bracket = 3;
    }
    
    OUT = (UVYCoord + bracket) / 4.0f;
}
*/

//void FoliageYFrameUVAnimate_float(float UVYCoord, float3 viewSpacePos, out float OUT) {
//    float uvDir = viewSpacePos.x;
//    float time = _Time * 12.0;
//    float input = time + 60.0 * uvDir;

//    //float input = time;
//    float bracket = floor(frac(input) * 4.0);
//    //bracket = 0.0;

    
//    OUT = (UVYCoord + bracket) / 4.0;
//}