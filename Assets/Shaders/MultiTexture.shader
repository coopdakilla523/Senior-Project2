Shader "Custom/MultiTexture" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("SpecColor", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.068125
		_DiffTex ("DiffTexture", 2D) = "white" {}
		_BumpMap ("BumpMap", 2D) = "bump" {}
		_SpecTex ("SpecTexture", 2D) = "spec" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 400
		
		CGPROGRAM
		#pragma surface surf BlinnPhong

		sampler2D _DiffTex;
		sampler2D _BumpMap;
		sampler2D _SpecTex;
		fixed4 _Color;
		half _Shininess;
				
		struct Input {
			float2 uv_DiffTex;
			float2 uv_BumpMap;
			float2 uv_SpecTex;
			
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_DiffTex, IN.uv_DiffTex);
			fixed4 specTex = tex2D(_SpecTex, IN.uv_SpecTex);
			
			o.Albedo = tex.rgb * _Color.rgb;
			o.Gloss = specTex.r;
			o.Alpha = tex.a * _Color.a;
			
			o.Specular = _Shininess * specTex.g;
			
			o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
			
			
		}
		ENDCG
	} 
	FallBack "Specular"
}
