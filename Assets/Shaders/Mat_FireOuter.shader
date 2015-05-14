// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:2,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32593,y:32648|diff-29-OUT,emission-81-OUT,alpha-8-A;n:type:ShaderForge.SFN_Tex2d,id:2,x:33071,y:32408,ptlb:node_2,ptin:_node_2,tex:5ad472f4cdd67444fa5dd21419783ac5,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:8,x:32947,y:33075,ptlb:node_8,ptin:_node_8,tex:68eb4e87997d6364d92eb4dc0f17a1ae,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:29,x:32773,y:32470|A-2-G,B-30-OUT;n:type:ShaderForge.SFN_Vector3,id:30,x:32959,y:32624,v1:0.5220588,v2:0.2781843,v3:0.1458694;n:type:ShaderForge.SFN_Tex2d,id:68,x:33164,y:32691,ptlb:Diff_copy,ptin:_Diff_copy,tex:5ad472f4cdd67444fa5dd21419783ac5,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector3,id:75,x:33111,y:32957,v1:0.5514706,v2:0.2746167,v3:0.1500324;n:type:ShaderForge.SFN_Multiply,id:81,x:32902,y:32815|A-68-RGB,B-75-OUT;proporder:2-8-68;pass:END;sub:END;*/

Shader "Shader Forge/Mat_FireOuter" {
    Properties {
        _node_2 ("node_2", 2D) = "white" {}
        _node_8 ("node_8", 2D) = "white" {}
        _Diff_copy ("Diff_copy", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _node_2; uniform float4 _node_2_ST;
            uniform sampler2D _node_8; uniform float4 _node_8_ST;
            uniform sampler2D _Diff_copy; uniform float4 _Diff_copy_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb*2;
////// Emissive:
                float2 node_102 = i.uv0;
                float3 emissive = (tex2D(_Diff_copy,TRANSFORM_TEX(node_102.rg, _Diff_copy)).rgb*float3(0.5514706,0.2746167,0.1500324));
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * (tex2D(_node_2,TRANSFORM_TEX(node_102.rg, _node_2)).g*float3(0.5220588,0.2781843,0.1458694));
                finalColor += emissive;
/// Final Color:
                return fixed4(finalColor,tex2D(_node_8,TRANSFORM_TEX(node_102.rg, _node_8)).a);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _node_2; uniform float4 _node_2_ST;
            uniform sampler2D _node_8; uniform float4 _node_8_ST;
            uniform sampler2D _Diff_copy; uniform float4 _Diff_copy_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float2 node_103 = i.uv0;
                finalColor += diffuseLight * (tex2D(_node_2,TRANSFORM_TEX(node_103.rg, _node_2)).g*float3(0.5220588,0.2781843,0.1458694));
/// Final Color:
                return fixed4(finalColor * tex2D(_node_8,TRANSFORM_TEX(node_103.rg, _node_8)).a,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
