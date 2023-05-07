Shader "Unlit/occlusionShader"
{
    
    SubShader
    {
        Tags { "RenderType"="Transparent" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma target 5.0

            #include "UnityCG.cginc"


            RWStructuredBuffer<float4> _Writer : register(u1);
            StructuredBuffer<float4> _Reader;

            float4 vert (float4 vertex : POSITION, out uint instance : TEXCOORD0, uint id : SV_VertexID) : SV_POSITION
            {
                instance = _Reader[id].w;
                return mul(UNITY_MATRIX_VP, float4(_Reader[id].xyz, 1.0));
            }

            [earlydepthstencil]
            float4 frag (float4 vertex : SV_POSITION, uint instance : TEXCOORD0) : SV_TARGET
            {
                _Writer[instance] = vertex;
            
                return float4(0.0, 0.0, 1.0, 0.2);
            }
            ENDCG
        }
    }
}
