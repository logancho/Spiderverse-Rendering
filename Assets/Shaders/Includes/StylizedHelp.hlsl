SAMPLER(sampler_trilinear_repeat);

void TriplanarColor_float(UnityTexture2D Texture, UnityTexture2D DitherTexture,
    float3 TR1, float3 TR2, float3 TR3,
    float3 Position, float3 Normal, float Tile, float Blend, float Diffuse, out float4 OUT)
{
    float3 Node_UV = Position * Tile;
    float b = fmod(Node_UV, 1.0);

    float3 Node_Blend = pow(abs(Normal), Blend);
    Node_Blend /= dot(Node_Blend, 1.0);
    
    //int tile_i = int(floor(Diffuse * 9));
    //tile_i = 0;
    
    //float2 Node_UV_x = float2(Node_UV.z / 9.0 + tile_i / 9.0, Node_UV.y);
    //float2 Node_UV_y = float2(Node_UV.x / 9.0 + tile_i / 9.0, Node_UV.z);
    //float2 Node_UV_z = float2(Node_UV.x / 9.0 + tile_i / 9.0, Node_UV.y);
    float2 Node_UV_x = float2(Node_UV.z, Node_UV.y);
    float2 Node_UV_y = float2(Node_UV.x, Node_UV.z);
    float2 Node_UV_z = float2(Node_UV.x, Node_UV.y);
    
    //All of the ones, above the 1/9 threshold, should modulo back to the 1/9 threshold ... I still want it to be tiled.
    
    float4 Node_X = SAMPLE_TEXTURE2D(Texture, sampler_trilinear_repeat, Node_UV_x);
    float4 Node_Y = SAMPLE_TEXTURE2D(Texture, sampler_trilinear_repeat, Node_UV_y);
    float4 Node_Z = SAMPLE_TEXTURE2D(Texture, sampler_trilinear_repeat, Node_UV_z);
    OUT = Node_X * Node_Blend.x + Node_Y * Node_Blend.y + Node_Z * Node_Blend.z;
}

//9 version
//void TriplanarColor_float(UnityTexture2D Texture, UnityTexture2D DitherTexture,
//    float3 TR1, float3 TR2, float3 TR3,
//    float3 Position, float3 Normal, float Tile, float Blend, float Diffuse, out float4 OUT)
//{
//    float3 Node_UV = Position * Tile;
//    float b = fmod(Node_UV, 1.0);
    
//    if (b != 0)
//    {
//        Node_UV = frac(Node_UV);
//    }
//    else
//    {
//        Node_UV = 1.0;
//    }
    
//    //Node_UV /= 9.0;
//    float3 Node_Blend = pow(abs(Normal), Blend);
//    Node_Blend /= dot(Node_Blend, 1.0);

//    float2 Node_UV_x = float2(Node_UV.z, Node_UV.y);
//    float2 Node_UV_y = float2(Node_UV.x, Node_UV.z);
//    float2 Node_UV_z = float2(Node_UV.x, Node_UV.y);
    
//    //All of the ones, above the 1/9 threshold, should modulo back to the 1/9 threshold ... I still want it to be tiled.
    
//    float4 Node_X = SAMPLE_TEXTURE2D(DitherTexture, sampler_point_clamp, Node_UV_x);
//    float4 Node_Y = SAMPLE_TEXTURE2D(DitherTexture, sampler_point_clamp, Node_UV_y);
//    float4 Node_Z = SAMPLE_TEXTURE2D(DitherTexture, sampler_point_clamp, Node_UV_z);
    
//    float4 DitherVal = Node_X * Node_Blend.x + Node_Y * Node_Blend.y + Node_Z * Node_Blend.z;
//    float remap_dither = (DitherVal.x - 0.5) * 0.4;
//    //remap_dither = 0;
//    float d_diffuse = saturate(Diffuse * (1.0 + remap_dither));
    
//    //d_diffuse = Diffuse;
        
//    int tile_i = int(((d_diffuse - 0.0001) * 9));
    
    
    
//    //float3 TR1, float3 TR2, float3 TR3
    
//    if (d_diffuse < TR1.x)
//    {
//        tile_i = 0;
//    }
//    else if (d_diffuse < TR1.y)
//    {
//        tile_i = 1;
//    }
//    else if (d_diffuse < TR1.z)
//    {
//        tile_i = 2;
//    }
//    else if (d_diffuse < TR2.x)
//    {
//        tile_i = 3;
//    }
//    else if (d_diffuse < TR2.y)
//    {
//        tile_i = 4;
//    }
//    else if (d_diffuse < TR2.z)
//    {
//        tile_i = 5;
//    }
//    else if (d_diffuse < TR3.x)
//    {
//        tile_i = 6;
//    }
//    else if (d_diffuse < TR3.y)
//    {
//        tile_i = 7;
//    }
//    else
//    {
//        tile_i = 8;
//    }
    
    
    
    
    
    
//    //tile_i = 0;
    
//    //float2 Node_UV_x = float2(Node_UV.z / 9.0 + tile_i / 9.0, Node_UV.y);
//    //float2 Node_UV_y = float2(Node_UV.x / 9.0 + tile_i / 9.0, Node_UV.z);
//    //float2 Node_UV_z = float2(Node_UV.x / 9.0 + tile_i / 9.0, Node_UV.y);
//    Node_UV_x = float2(Node_UV.z / 9.0 + tile_i / 9.0, Node_UV.y);
//    Node_UV_y = float2(Node_UV.x / 9.0 + tile_i / 9.0, Node_UV.z);
//    Node_UV_z = float2(Node_UV.x / 9.0 + tile_i / 9.0, Node_UV.y);
    
//    Node_X = SAMPLE_TEXTURE2D(Texture, sampler_point_clamp, Node_UV_x);
//    Node_Y = SAMPLE_TEXTURE2D(Texture, sampler_point_clamp, Node_UV_y);
//    Node_Z = SAMPLE_TEXTURE2D(Texture, sampler_point_clamp, Node_UV_z);
    
//    OUT = Node_X * Node_Blend.x + Node_Y * Node_Blend.y + Node_Z * Node_Blend.z;
//}