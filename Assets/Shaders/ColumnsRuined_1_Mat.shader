// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-624-RGB,spec-628-RGB,normal-646-OUT;n:type:ShaderForge.SFN_Tex2d,id:624,x:33114,y:32568,ptlb:node_diffuse,ptin:_node_diffuse,tex:79a45fd4c0ad4cc48b2e5fb2814a5bac,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:626,x:33583,y:33281,ptlb:Nrm_Structure,ptin:_Nrm_Structure,tex:6ccb25d20978de3488bba648401a5a03,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:628,x:33263,y:32753,ptlb:Node_spec,ptin:_Node_spec,tex:d344079e3f1725643b8f8dc8b8d1b0aa,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector3,id:630,x:33647,y:33051,v1:1,v2:1,v3:2;n:type:ShaderForge.SFN_Multiply,id:632,x:33449,y:32943|A-634-RGB,B-630-OUT;n:type:ShaderForge.SFN_Tex2d,id:634,x:33627,y:32813,ptlb:Nrm_Noise,ptin:_Nrm_Noise,tex:6d4bf8d475fb823459f90225844677b7,ntxv:3,isnm:True|UVIN-642-OUT;n:type:ShaderForge.SFN_TexCoord,id:636,x:34112,y:32768,uv:0;n:type:ShaderForge.SFN_Multiply,id:638,x:33334,y:33387|A-626-RGB,B-640-OUT;n:type:ShaderForge.SFN_Vector3,id:640,x:33557,y:33490,v1:4,v2:4,v3:0.2;n:type:ShaderForge.SFN_Multiply,id:642,x:33869,y:32789|A-636-UVOUT,B-644-OUT;n:type:ShaderForge.SFN_Vector1,id:644,x:34100,y:32952,v1:3;n:type:ShaderForge.SFN_Add,id:646,x:33135,y:33098|A-632-OUT,B-638-OUT;proporder:626-634-628-624;pass:END;sub:END;*/

Shader "Shader Forge/ColumnsRuined_1_Mat" {
    Properties {
        _Nrm_Structure ("Nrm_Structure", 2D) = "bump" {}
        _Nrm_Noise ("Nrm_Noise", 2D) = "bump" {}
        _Node_spec ("Node_spec", 2D) = "white" {}
        _node_diffuse ("node_diffuse", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _node_diffuse; uniform float4 _node_diffuse_ST;
            uniform sampler2D _Nrm_Structure; uniform float4 _Nrm_Structure_ST;
            uniform sampler2D _Node_spec; uniform float4 _Node_spec_ST;
            uniform sampler2D _Nrm_Noise; uniform float4 _Nrm_Noise_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_642 = (i.uv0.rg*3.0);
                float2 node_652 = i.uv0;
                float3 normalLocal = ((UnpackNormal(tex2D(_Nrm_Noise,TRANSFORM_TEX(node_642, _Nrm_Noise))).rgb*float3(1,1,2))+(UnpackNormal(tex2D(_Nrm_Structure,TRANSFORM_TEX(node_652.rg, _Nrm_Structure))).rgb*float3(4,4,0.2)));
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb*2;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = tex2D(_Node_spec,TRANSFORM_TEX(node_652.rg, _Node_spec)).rgb;
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * tex2D(_node_diffuse,TRANSFORM_TEX(node_652.rg, _node_diffuse)).rgb;
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            uniform sampler2D _node_diffuse; uniform float4 _node_diffuse_ST;
            uniform sampler2D _Nrm_Structure; uniform float4 _Nrm_Structure_ST;
            uniform sampler2D _Node_spec; uniform float4 _Node_spec_ST;
            uniform sampler2D _Nrm_Noise; uniform float4 _Nrm_Noise_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 binormalDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.tangentDir = normalize( mul( _Object2World, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.binormalDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.binormalDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float2 node_642 = (i.uv0.rg*3.0);
                float2 node_653 = i.uv0;
                float3 normalLocal = ((UnpackNormal(tex2D(_Nrm_Noise,TRANSFORM_TEX(node_642, _Nrm_Noise))).rgb*float3(1,1,2))+(UnpackNormal(tex2D(_Nrm_Structure,TRANSFORM_TEX(node_653.rg, _Nrm_Structure))).rgb*float3(4,4,0.2)));
                float3 normalDirection =  normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
///////// Gloss:
                float gloss = 0.5;
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                NdotL = max(0.0, NdotL);
                float3 specularColor = tex2D(_Node_spec,TRANSFORM_TEX(node_653.rg, _Node_spec)).rgb;
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * tex2D(_node_diffuse,TRANSFORM_TEX(node_653.rg, _node_diffuse)).rgb;
                finalColor += specular;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}