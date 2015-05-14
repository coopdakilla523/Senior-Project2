// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32464,y:32631|diff-625-RGB,spec-629-RGB,normal-647-OUT;n:type:ShaderForge.SFN_Tex2d,id:625,x:32784,y:32505,ptlb:node_diffuse,ptin:_node_diffuse,tex:914d7a8eb6038f649869adcf372dfa69,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:627,x:33195,y:33262,ptlb:Nrm_Structure,ptin:_Nrm_Structure,tex:16028b23eeef94246acd7caa117ac861,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:629,x:33029,y:32712,ptlb:Node_spec,ptin:_Node_spec,tex:0c6f2fed58735bc4899165438d96bbe6,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector3,id:631,x:33259,y:33032,v1:1,v2:1,v3:2;n:type:ShaderForge.SFN_Multiply,id:633,x:33061,y:32924|A-635-RGB,B-631-OUT;n:type:ShaderForge.SFN_Tex2d,id:635,x:33281,y:32664,ptlb:Nrm_Noise,ptin:_Nrm_Noise,tex:6d4bf8d475fb823459f90225844677b7,ntxv:3,isnm:True|UVIN-643-OUT;n:type:ShaderForge.SFN_TexCoord,id:637,x:33544,y:33011,uv:0;n:type:ShaderForge.SFN_Multiply,id:639,x:32980,y:33279|A-627-RGB,B-641-OUT;n:type:ShaderForge.SFN_Vector3,id:641,x:33169,y:33471,v1:4,v2:4,v3:0.2;n:type:ShaderForge.SFN_Multiply,id:643,x:33399,y:32839|A-637-UVOUT,B-645-OUT;n:type:ShaderForge.SFN_Vector1,id:645,x:33496,y:33174,v1:4;n:type:ShaderForge.SFN_Add,id:647,x:32821,y:33042|A-633-OUT,B-639-OUT;proporder:625-629-627-635;pass:END;sub:END;*/

Shader "Shader Forge/TombFancy_1_Mat" {
    Properties {
        _node_diffuse ("node_diffuse", 2D) = "white" {}
        _Node_spec ("Node_spec", 2D) = "white" {}
        _Nrm_Structure ("Nrm_Structure", 2D) = "bump" {}
        _Nrm_Noise ("Nrm_Noise", 2D) = "bump" {}
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
                float2 node_643 = (i.uv0.rg*4.0);
                float2 node_653 = i.uv0;
                float3 normalLocal = ((UnpackNormal(tex2D(_Nrm_Noise,TRANSFORM_TEX(node_643, _Nrm_Noise))).rgb*float3(1,1,2))+(UnpackNormal(tex2D(_Nrm_Structure,TRANSFORM_TEX(node_653.rg, _Nrm_Structure))).rgb*float3(4,4,0.2)));
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
                float3 specularColor = tex2D(_Node_spec,TRANSFORM_TEX(node_653.rg, _Node_spec)).rgb;
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * tex2D(_node_diffuse,TRANSFORM_TEX(node_653.rg, _node_diffuse)).rgb;
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
                float2 node_643 = (i.uv0.rg*4.0);
                float2 node_654 = i.uv0;
                float3 normalLocal = ((UnpackNormal(tex2D(_Nrm_Noise,TRANSFORM_TEX(node_643, _Nrm_Noise))).rgb*float3(1,1,2))+(UnpackNormal(tex2D(_Nrm_Structure,TRANSFORM_TEX(node_654.rg, _Nrm_Structure))).rgb*float3(4,4,0.2)));
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
                float3 specularColor = tex2D(_Node_spec,TRANSFORM_TEX(node_654.rg, _Node_spec)).rgb;
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * tex2D(_node_diffuse,TRANSFORM_TEX(node_654.rg, _node_diffuse)).rgb;
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
