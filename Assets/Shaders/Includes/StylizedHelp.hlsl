SAMPLER(sampler_trilinear_repeat);

void TriplanarColor_float(UnityTexture2D Texture, float3 Position, float3 Normal, float Tile, float Blend, float Diffuse, out float4 OUT)
{
    float3 Node_UV = Position * Tile;
    float b = fmod(Node_UV, 1.0);
    if (b != 0)
    {
        Node_UV = frac(Node_UV);
    }
    else
    {
        Node_UV = 1.0;
    }
    
    //Node_UV /= 9.0;
    float3 Node_Blend = pow(abs(Normal), Blend);
    Node_Blend /= dot(Node_Blend, 1.0);
    
    int tile_i = int(floor(Diffuse * 9));
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
