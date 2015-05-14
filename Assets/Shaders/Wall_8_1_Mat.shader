// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:True,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32719,y:32712|diff-620-RGB,spec-624-RGB,normal-642-OUT;n:type:ShaderForge.SFN_Tex2d,id:620,x:33154,y:32639,ptlb:node_diffuse,ptin:_node_diffuse,tex:6c46c6d422e53934ea031bcef849ced9,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:622,x:33569,y:33453,ptlb:node_3,ptin:_node_3,tex:447f665826273eb499a629221755f95e,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Tex2d,id:624,x:33248,y:32830,ptlb:node_4,ptin:_node_4,tex:704881cd4e7ebc5438bd74459abbe5f2,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Vector3,id:626,x:33543,y:33283,v1:1,v2:1,v3:2;n:type:ShaderForge.SFN_Multiply,id:628,x:33435,y:33115|A-630-RGB,B-626-OUT;n:type:ShaderForge.SFN_Tex2d,id:630,x:33655,y:32855,ptlb:node_11m,ptin:_node_11m,tex:6d4bf8d475fb823459f90225844677b7,ntxv:3,isnm:True|UVIN-638-OUT;n:type:ShaderForge.SFN_TexCoord,id:632,x:33918,y:33202,uv:0;n:type:ShaderForge.SFN_Multiply,id:634,x:33320,y:33559|A-622-RGB,B-636-OUT;n:type:ShaderForge.SFN_Vector3,id:636,x:33543,y:33662,v1:4,v2:4,v3:0.2;n:type:ShaderForge.SFN_Multiply,id:638,x:33773,y:33030|A-632-UVOUT,B-640-OUT;n:type:ShaderForge.SFN_Vector1,id:640,x:33870,y:33365,v1:4;n:type:ShaderForge.SFN_Add,id:642,x:33121,y:33270|A-628-OUT,B-634-OUT;proporder:620-624-622-630;pass:END;sub:END;*/

Shader "Shader Forge/Wall_8_1_Mat" {
    Properties {
        _node_diffuse ("node_diffuse", 2D) = "white" {}
        _node_4 ("node_4", 2D) = "white" {}
        _node_3 ("node_3", 2D) = "bump" {}
        _node_11m ("node_11m", 2D) = "bump" {}
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
            uniform sampler2D _node_3; uniform float4 _node_3_ST;
            uniform sampler2D _node_4; uniform float4 _node_4_ST;
            uniform sampler2D _node_11m; uniform float4 _node_11m_ST;
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
                float2 node_638 = (i.uv0.rg*4.0);
                float2 node_648 = i.uv0;
                float3 normalLocal = ((UnpackNormal(tex2D(_node_11m,TRANSFORM_TEX(node_638, _node_11m))).rgb*float3(1,1,2))+(UnpackNormal(tex2D(_node_3,TRANSFORM_TEX(node_648.rg, _node_3))).rgb*float3(4,4,0.2)));
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
                float3 specularColor = tex2D(_node_4,TRANSFORM_TEX(node_648.rg, _node_4)).rgb;
                float3 specular = (floor(attenuation) * _LightColor0.xyz) * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * tex2D(_node_diffuse,TRANSFORM_TEX(node_648.rg, _node_diffuse)).rgb;
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
            uniform sampler2D _node_3; uniform float4 _node_3_ST;
            uniform sampler2D _node_4; uniform float4 _node_4_ST;
            uniform sampler2D _node_11m; uniform float4 _node_11m_ST;
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
                float2 node_638 = (i.uv0.rg*4.0);
                float2 node_649 = i.uv0;
                float3 normalLocal = ((UnpackNormal(tex2D(_node_11m,TRANSFORM_TEX(node_638, _node_11m))).rgb*float3(1,1,2))+(UnpackNormal(tex2D(_node_3,TRANSFORM_TEX(node_649.rg, _node_3))).rgb*float3(4,4,0.2)));
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
                float3 specularColor = tex2D(_node_4,TRANSFORM_TEX(node_649.rg, _node_4)).rgb;
                float3 specular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow) * specularColor;
                float3 finalColor = 0;
                float3 diffuseLight = diffuse;
                finalColor += diffuseLight * tex2D(_node_diffuse,TRANSFORM_TEX(node_649.rg, _node_diffuse)).rgb;
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
