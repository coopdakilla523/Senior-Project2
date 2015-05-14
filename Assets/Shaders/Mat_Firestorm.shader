// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:2,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:False,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32597,y:32702|diff-2-RGB,emission-285-RGB,clip-3-A;n:type:ShaderForge.SFN_Tex2d,id:2,x:32894,y:32655,ptlb:Diff,ptin:_Diff,tex:ab2d82cdda40a0743b26bf08a8d4f5a8,ntxv:0,isnm:False|UVIN-306-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:3,x:32913,y:33007,ptlb:Alpha,ptin:_Alpha,tex:8675073f9a37d7d43b0b9fab2b84dea8,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:241,x:33369,y:32823,uv:0;n:type:ShaderForge.SFN_Tex2d,id:285,x:33053,y:32845,ptlb:Diff_copy,ptin:_Diff_copy,tex:396572dc580085949909d34c4e671f4e,ntxv:0,isnm:False|UVIN-403-UVOUT;n:type:ShaderForge.SFN_Panner,id:306,x:33203,y:32604,spu:1,spv:1|UVIN-365-UVOUT;n:type:ShaderForge.SFN_Rotator,id:365,x:33403,y:32548|UVIN-366-UVOUT;n:type:ShaderForge.SFN_TexCoord,id:366,x:32661,y:32314,uv:0;n:type:ShaderForge.SFN_Rotator,id:403,x:33277,y:32978;proporder:2-3-285;pass:END;sub:END;*/

Shader "Shader Forge/Mat_Firestorm" {
    Properties {
        _Diff ("Diff", 2D) = "white" {}
        _Alpha ("Alpha", 2D) = "white" {}
        _Diff_copy ("Diff_copy", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            
            
            Fog {Mode Off}
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
            uniform float4 _TimeEditor;
            uniform sampler2D _Diff; uniform float4 _Diff_ST;
            uniform sampler2D _Alpha; uniform float4 _Alpha_ST;
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
                float2 node_422 = i.uv0;
                clip(tex2D(_Alpha,TRANSFORM_TEX(node_422.rg, _Alpha)).a - 0.5);
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor + UNITY_LIGHTMODEL_AMBIENT.rgb*2;
////// Emissive:
                float4 node_423 = _Time + _TimeEditor;
                float node_403_ang = node_423.g;
                float node_403_spd = 1.0;
                float node_403_cos = cos(node_403_spd*node_403_ang);
                float node_403_sin = sin(node_403_spd*node_403_ang);
                float2 node_403_piv = float2(0.5,0.5);
                float2 node_403 = (mul(node_422.rg-node_403_piv,float2x2( node_403_cos, -node_403_sin, node_403_sin, node_403_cos))+node_403_piv);
                float3 emissive = tex2D(_Diff_copy,TRANSFORM_TEX(node_403, _Diff_copy)).rgb;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float node_365_ang = node_423.g;
                float node_365_spd = 1.0;
                float node_365_cos = cos(node_365_spd*node_365_ang);
                float node_365_sin = sin(node_365_spd*node_365_ang);
                float2 node_365_piv = float2(0.5,0.5);
                float2 node_365 = (mul(i.uv0.rg-node_365_piv,float2x2( node_365_cos, -node_365_sin, node_365_sin, node_365_cos))+node_365_piv);
                float2 node_306 = (node_365+node_423.g*float2(1,1));
                finalColor += diffuseLight * tex2D(_Diff,TRANSFORM_TEX(node_306, _Diff)).rgb;
                finalColor += emissive;
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
            uniform float4 _TimeEditor;
            uniform sampler2D _Diff; uniform float4 _Diff_ST;
            uniform sampler2D _Alpha; uniform float4 _Alpha_ST;
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
                float2 node_424 = i.uv0;
                clip(tex2D(_Alpha,TRANSFORM_TEX(node_424.rg, _Alpha)).a - 0.5);
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i)*2;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = dot( normalDirection, lightDirection );
                float3 diffuse = max( 0.0, NdotL) * attenColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                float4 node_425 = _Time + _TimeEditor;
                float node_365_ang = node_425.g;
                float node_365_spd = 1.0;
                float node_365_cos = cos(node_365_spd*node_365_ang);
                float node_365_sin = sin(node_365_spd*node_365_ang);
                float2 node_365_piv = float2(0.5,0.5);
                float2 node_365 = (mul(i.uv0.rg-node_365_piv,float2x2( node_365_cos, -node_365_sin, node_365_sin, node_365_cos))+node_365_piv);
                float2 node_306 = (node_365+node_425.g*float2(1,1));
                finalColor += diffuseLight * tex2D(_Diff,TRANSFORM_TEX(node_306, _Diff)).rgb;
/// Final Color:
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
        Pass {
            Name "ShadowCollector"
            Tags {
                "LightMode"="ShadowCollector"
            }
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCOLLECTOR
            #define SHADOW_COLLECTOR_PASS
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcollector
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _Alpha; uniform float4 _Alpha_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_COLLECTOR;
                float2 uv0 : TEXCOORD5;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_COLLECTOR(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_426 = i.uv0;
                clip(tex2D(_Alpha,TRANSFORM_TEX(node_426.rg, _Alpha)).a - 0.5);
                SHADOW_COLLECTOR_FRAGMENT(i)
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Cull Off
            Offset 1, 1
            
            Fog {Mode Off}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform sampler2D _Alpha; uniform float4 _Alpha_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                float2 node_427 = i.uv0;
                clip(tex2D(_Alpha,TRANSFORM_TEX(node_427.rg, _Alpha)).a - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
