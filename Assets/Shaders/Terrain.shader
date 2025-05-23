Shader "natemcandrews/Terrain"
{
    Properties{
        testTexture("Texture", 2D) = "white"{}
        testScale("Scale", Float) = 1
    }

    SubShader{
        // These tags are shared by all passes in this sub shader
        Tags{"RenderPipeline" = "UniversalPipeline"}
        
        Pass {
            Name "ForwardLit" // For debugging
            Tags{"LightMode" = "UniversalForward"} // Pass specific tags. 
            // "UniversalForward" tells Unity this is the main lighting pass of this shader

            HLSLPROGRAM // Begin HLSL code
            // Register our programmable stage functions

            #define _SPECULAR_COLOR


            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma multi_compile _ _FORWARD_PLUS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            

            // Include our code file
            #include "hlsl/MyLitForwardLitPass.hlsl"
            ENDHLSL
        }

        Pass {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}

            HLSLPROGRAM

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "hlsl/MyLitShadowCasterPass.hlsl"

            ENDHLSL
        }
        
    }
}