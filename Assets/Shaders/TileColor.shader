Shader "Jatewit/Tile Color" {
	Properties {
		_Color ("Color", Color) = (1.0,1.0,1.0,1.0)
		_PathColor ("Path Color", Color) = (0.25,0.5,1.0,1.0)
		_MainTex ("Base (RGBA)", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Geometry" "IgnoreProjector"="True" "RenderType"="Opaque"}
		LOD 200

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		fixed4 _PathColor;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			float3 newColor;
			if (c.a <= 0.8f) {
				newColor = (c.a*_PathColor) + (_Color.rgb - 0.5f);
			} else {
				newColor = c.rgb + (_Color.rgb - 0.5f);
			}
			o.Albedo = newColor;
			if (c.a <= 0.75f) {
				o.Alpha = _Color.a*c.a;
			} else {
				o.Alpha = _Color.a;
			}
		}
		ENDCG
	}

	Fallback "Diffuse"
}