Shader "Custom/InstancingDemoShader2"
{
    Properties
    {
[NoScaleOffset] _MainTex ("Color (RGB) Alpha (A)", 2D) = "white"
_CutOff("Alpha CutOff", float) = 0.5

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent" } 
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha
        
        Cull Off

        ZWrite On

        ZTest LEqual

        //Blend OneZero
        
        //**AlphaToMask On**

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 uv : TEXCOORD0;
};
                        
            struct MeshProperties {
                float4x4 mat;
                float4 color;
                //float2 UV;
            };


            StructuredBuffer<MeshProperties> _Properties;

            v2f vert (appdata i, uint instanceID: SV_InstanceID)
            {
                v2f o;

                //Move from world space, to object space, by calculating pos
                float4 pos = mul(_Properties[instanceID].mat, i.vertex);

                //Transfer pos to 
                o.vertex = UnityObjectToClipPos(pos);
                o.color = _Properties[instanceID].color;
                //o.vertex = float4(1, 1, 1, 1);
                //o.color = fixed4(1, 1, 1, 1);
                o.uv = i.uv;

                return o;
            }
sampler2D _MainTex;
float _CutOff = 0.23;
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 alpha = tex2D(_MainTex, i.uv);
                fixed4 output = i.color;
                output.a = alpha.a;
    
                float bruh = smoothstep(_CutOff - 0.02, _CutOff + 0.02, output.a);
                clip(bruh - _CutOff);
    
                return output;
            }
            ENDCG
        }
    }
}
