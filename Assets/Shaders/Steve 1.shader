// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-2-RGB,spec-68-OUT,normal-24-OUT;n:type:ShaderForge.SFN_Tex2d,id:2,x:33305,y:32599,ptlb:Steve_Diff,ptin:_Steve_Diff,tex:48b49ddcb0082124e8701a9ce6088056,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:18,x:33380,y:33128,ptlb:Steve_NRM,ptin:_Steve_NRM,tex:a6c3d1ecd171d3e47a921439e0a11db0,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Multiply,id:24,x:33050,y:33120|A-18-RGB,B-25-OUT;n:type:ShaderForge.SFN_Vector3,id:25,x:33243,y:33197,v1:1,v2:1,v3:0.5;n:type:ShaderForge.SFN_Tex2d,id:61,x:33356,y:32792,ptlb:node_61,ptin:_node_61,tex:a2ed72c817c49b34fbd17adc04ffa73d,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:67,x:33356,y:32965,ptlb:node_67,ptin:_node_67,tex:89a402e94c47eb040b1d85c2584c844b,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:68,x:33080,y:32808|A-61-RGB,B-67-RGB;n:type:ShaderForge.SFN_TexCoord,id:94,x:33659,y:32559,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:95,x:33693,y:32755,ptlb:node_95,ptin:_node_95,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:96,x:33479,y:32636|A-94-UVOUT,B-95-OUT;n:type:ShaderForge.SFN_ValueProperty,id:103,x:33907,y:32962,ptlb:node_95_copy,ptin:_node_95_copy,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:105,x:33693,y:32843|A-107-UVOUT,B-103-OUT;n:type:ShaderForge.SFN_TexCoord,id:107,x:33873,y:32766,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:114,x:33987,y:33385,ptlb:node_95_copy_copy,ptin:_node_95_copy_copy,glob:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:116,x:33773,y:33266|A-118-UVOUT,B-114-OUT;n:type:ShaderForge.SFN_TexCoord,id:118,x:33953,y:33189,uv:0;proporder:2-18-61-67-95-103-114;pass:END;sub:END;*/

Shader "Shader Forge/Steve" {
    Properties {
        _Steve_Diff ("Steve_Diff", 2D) = "white" {}
        _Steve_NRM ("Steve_NRM", 2D) = "bump" {}
        _node_61 ("node_61", 2D) = "white" {}
        _node_67 ("node_67", 2D) = "white" {}
        _node_95 ("node_95", Float ) = 1
        _node_95_copy ("node_95_copy", Float ) = 1
        _node_95_copy_copy ("node_95_copy_copy", Float ) = 1
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
            uniform sampler2D _Steve_Diff; uniform float4 _Steve_Diff_ST;
            uniform sampler2D _Steve_NRM; uniform float4 _Steve_NRM_ST;
            uniform sampler2D _node_61; uniform float4 _node_61_ST;
            uniform sampler2D _node_67; uniform float4 _node_67_ST;
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
                float2 node_124 = i.uv0;
                float3 normalLocal = (UnpackNormal(tex2D(_Steve_NRM,TRANSFORM_TEX(node_124.rg, _Steve_NRM))).rgb*float3(1,1,0.5));
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
                float3 specularColor = (tex2D(_node_61,TRANSFORM_TEX(node_124.rg, _node_61)).rgb*tex2D(_node_67,TRANSFORM_TEX(node_124.rg, _node_67)).rgb);
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * tex2D(_Steve_Diff,TRANSFORM_TEX(node_124.rg, _Steve_Diff)).rgb;
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
            uniform sampler2D _Steve_Diff; uniform float4 _Steve_Diff_ST;
            uniform sampler2D _Steve_NRM; uniform float4 _Steve_NRM_ST;
            uniform sampler2D _node_61; uniform float4 _node_61_ST;
            uniform sampler2D _node_67; uniform float4 _node_67_ST;
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
                float2 node_125 = i.uv0;
                float3 normalLocal = (UnpackNormal(tex2D(_Steve_NRM,TRANSFORM_TEX(node_125.rg, _Steve_NRM))).rgb*float3(1,1,0.5));
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
                float3 specularColor = (tex2D(_node_61,TRANSFORM_TEX(node_125.rg, _node_61)).rgb*tex2D(_node_67,TRANSFORM_TEX(node_125.rg, _node_67)).rgb);
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * tex2D(_Steve_Diff,TRANSFORM_TEX(node_125.rg, _Steve_Diff)).rgb;
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
