SAMPLER(sampler_linear_clamp);
SAMPLER(sampler_trilinear_repeat);
SAMPLER(sampler_trilinear_clamp);

void ProperStep_float(float IN, float threshold, out float OUT)
{
    if (IN > threshold) {
        OUT = 1.0f;
    }
    else
    {
        OUT = IN;
    }
}


//void ProperCull_float(float IN, float threshold, out float OUT)
//{
//    if (IN < threshold)
//    {
//        OUT = 0;
//    }
//    else
//    {
//        OUT = IN;
//    }
//}


float4 Ellipse(float2 UV, float Width, float Height)
{
    float2 repeatUV = float2(frac(UV.x), frac(UV.y));
    float d = length((repeatUV * 2 - 1) / float2(Width, Height));
    return saturate((1 - d) / fwidth(d));
}

void Ellipse_float(float2 UV, float Width, float Height, out float4 OUT)
{
    float2 repeatUV = float2(frac(UV.x), frac(UV.y));
    float d = length((repeatUV * 2 - 1) / float2(Width, Height));
    OUT = saturate((1 - d) / fwidth(d));
}


//void TriplanarTextureProper_float(UnityTexture2D Texture,
//    float3 Position, float3 Normal, float Tile, float Blend, out float4 OUT)
//{
//    float3 Node_UV = Position * Tile;
//    //float b = fmod(Node_UV, 1.0);

//    float3 Node_Blend = pow(abs(Normal), Blend);
//    Node_Blend /= dot(Node_Blend, 1.0);

//    float2 Node_UV_x = float2(Node_UV.z, Node_UV.y);
//    float2 Node_UV_y = float2(Node_UV.x, Node_UV.z);
//    float2 Node_UV_z = float2(Node_UV.x, Node_UV.y);
    
//    //All of the ones, above the 1/9 threshold, should modulo back to the 1/9 threshold ... I still want it to be tiled.
    
//    float4 Node_X = SAMPLE_TEXTURE2D(Texture, sampler_trilinear_repeat, Node_UV_x);
//    float4 Node_Y = SAMPLE_TEXTURE2D(Texture, sampler_trilinear_repeat, Node_UV_y);
//    float4 Node_Z = SAMPLE_TEXTURE2D(Texture, sampler_trilinear_repeat, Node_UV_z);

//    OUT = Node_X * Node_Blend.x + Node_Y * Node_Blend.y + Node_Z * Node_Blend.z;
//}

void TriplanarBruh_float(UnityTexture2D Texture, float3 Position, float3 Normal, float Tile, float Blend_bruh, out float4 OUT)
{
    float3 Node_UV = Position * Tile;
    //float b = fmod(Node_UV, 1.0);

    float3 Node_Blend_bruh = pow(abs(Normal), Blend_bruh);
    Node_Blend_bruh /= dot(Node_Blend_bruh, 1.0);
    
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
    
    //float4 Node_X = Ellipse()
    //float4 Node_X = Ellipse(Node_UV_x, Radius, Radius);
    OUT = Node_X * Node_Blend_bruh.x + Node_Y * Node_Blend_bruh.y + Node_Z * Node_Blend_bruh.z;
}

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
    
    //float4 Node_X = Ellipse()
    //float4 Node_X = Ellipse(Node_UV_x, Radius, Radius);
    OUT = Node_X * Node_Blend.x + Node_Y * Node_Blend.y + Node_Z * Node_Blend.z;
}

void TriplanarDot_float(UnityTexture2D Texture, UnityTexture2D DitherTexture,
    float3 Position, float3 Normal, float Tile, float Blend, float Diffuse,
    float Radius, out float4 OUT)
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
    
    //float4 Node_X = SAMPLE_TEXTURE2D(Texture, sampler_point_clamp, Node_UV_x);
    //float4 Node_Y = SAMPLE_TEXTURE2D(Texture, sampler_point_clamp, Node_UV_y);
    //float4 Node_Z = SAMPLE_TEXTURE2D(Texture, sampler_point_clamp, Node_UV_z);
    
    //float4 Node_X = Ellipse()
    float4 Node_X = Ellipse(Node_UV_x, Radius, Radius);
    float4 Node_Y = Ellipse(Node_UV_y, Radius, Radius);
    float4 Node_Z = Ellipse(Node_UV_z, Radius, Radius);
    
    OUT = Node_X * Node_Blend.x + Node_Y * Node_Blend.y + Node_Z * Node_Blend.z;
}

static float epsilon = 0.001f;

bool equality(float a, float b)
{
    return (abs(a - b) <= epsilon);
}
void PainterlyNormal_float(UnityTexture2D neg_x_face, UnityTexture2D pos_x_face, 
                            UnityTexture2D pos_z_face, UnityTexture2D neg_z_face,
                            UnityTexture2D pos_y_face, UnityTexture2D neg_y_face,
float3 normal, out float3 OUT)
{
    //OUT = normalize(float3(1, 1, 1));
    float v = 1 / sqrt(3);
    
    if (normal.x < 0 && abs(normal.x) >= abs(normal.y) + epsilon && abs(normal.x) >= abs(normal.z) + epsilon)
    {
        float x = (normal.g / normal.r + 1.0f) / 2;
        float y = (normal.b / normal.r + 1.0f) / 2;
        float2 UV = float2(1.0, 1.0) + float2(-y, -x);

        //UV *= 1.0f;
        float4 rawNormal = SAMPLE_TEXTURE2D(neg_x_face, sampler_trilinear_clamp, UV);
        OUT = normalize(rawNormal.rgb - float3(0.5, 0.5, 0.5));
        OUT = 0.3 * normal + 0.7 * OUT;
    }
    else if (normal.x > 0 && abs(normal.x) >= abs(normal.y) + epsilon && abs(normal.x) >= abs(normal.z) + epsilon)
    {
        float x = (normal.g / normal.r + 1.0f) / 2;
        float y = (normal.b / normal.r + 1.0f) / 2;
        //float2 UV = float2(1.0, 1.0) + float2(-x, -y);
        float2 UV = float2(1.0f - y, x);

        //UV *= 1.0f;
        float4 rawNormal = SAMPLE_TEXTURE2D(pos_x_face, sampler_trilinear_clamp, UV);
        OUT = normalize(rawNormal.rgb - float3(0.5, 0.5, 0.5));
        OUT = 0.3 * normal + 0.7 * OUT;
    }
    else if (normal.z > 0 && abs(normal.z) >= abs(normal.y) + epsilon && abs(normal.z) >= abs(normal.x) + epsilon)
    {
        float x = (normal.r / normal.b + 1.0f) / 2;
        float y = (normal.g / normal.b + 1.0f) / 2;
        //float2 UV = float2(1.0, 1.0) + float2(-y, -x);
        float2 UV = float2(x, y);

        //UV *= 1.0f;
        float4 rawNormal = SAMPLE_TEXTURE2D(pos_z_face, sampler_trilinear_clamp, UV);
        OUT = normalize(rawNormal.rgb - float3(0.5, 0.5, 0.5));
        OUT = 0.3 * normal + 0.7 * OUT;
    }
    else if (normal.z < 0 && abs(normal.z) >= abs(normal.y) + epsilon && abs(normal.z) >= abs(normal.x) + epsilon)
    {
        float x = (normal.r / normal.b + 1.0f) / 2;
        float y = (normal.g / normal.b + 1.0f) / 2;
        //float2 UV = float2(1.0, 1.0) + float2(-y, -x);
        float2 UV = float2(1.0 - x, y);

        //UV *= 1.0f;
        float4 rawNormal = SAMPLE_TEXTURE2D(neg_z_face, sampler_trilinear_clamp, UV);
        OUT = normalize(rawNormal.rgb - float3(0.5, 0.5, 0.5));
        OUT = 0.3 * normal + 0.7 * OUT;
    }
    else if (normal.y > 0 && abs(normal.y) >= abs(normal.x) + epsilon && abs(normal.y) >= abs(normal.z) + epsilon)
    {
        float x = (normal.r / normal.g + 1.0f) / 2;
        float y = (normal.b / normal.g + 1.0f) / 2;
        //float2 UV = float2(1.0, 1.0) + float2(-y, -x);
        float2 UV = float2(x, 1.0 - y);

        //UV *= 1.0f;
        float4 rawNormal = SAMPLE_TEXTURE2D(pos_y_face, sampler_trilinear_clamp, UV);
        OUT = normalize(rawNormal.rgb - float3(0.5, 0.5, 0.5));
        OUT = 0.3 * normal + 0.7 * OUT;
    }
    else if (normal.y < 0 && abs(normal.y) >= abs(normal.x) + epsilon && abs(normal.y) >= abs(normal.z)+ epsilon)
    {
        float x = (normal.r / normal.g + 1.0f) / 2;
        float y = (normal.b / normal.g + 1.0f) / 2;
        //float2 UV = float2(1.0, 1.0) + float2(-y, -x);
        float2 UV = float2(1.0 - x, 1.0 - y);

        //UV *= 1.0f;
        float4 rawNormal = SAMPLE_TEXTURE2D(neg_y_face, sampler_trilinear_clamp, UV);
        OUT = normalize(rawNormal.rgb - float3(0.5, 0.5, 0.5));
        OUT = 0.3 * normal + 0.7 * OUT;
    }
    else
    {
        OUT = normal;
    }
    
    
}


void TestUV_float(UnityTexture2D left_face, float2 UV, out float4 OUT)
{
    OUT = SAMPLE_TEXTURE2D(left_face, sampler_linear_clamp, UV);
}

//void PainterlyNormal_float(UnityTexture2D left_face, float3 Normal, out float3 OUT)
//{
//    OUT = float3(0);
//}

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