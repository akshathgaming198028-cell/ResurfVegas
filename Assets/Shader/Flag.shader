Shader "Bend/Flag" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color (RGBA)", Color) = (1,1,1,1)
 _AmplitudeX ("Amplitude X", Float) = 0.05
 _AmplitudeY ("Amplitude Y", Float) = 0.05
 _FrequenceyX ("Frequencey X", Float) = 9.5
 _FrequenceyY ("Frequencey Y", Float) = 6.5
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutput o)
		{
			float4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}