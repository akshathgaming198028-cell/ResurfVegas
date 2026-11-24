Shader "Unlit/Transparent" {
Properties {
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
}
SubShader {
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  // Port of the original GLES program: plain texture, no bend, no vertex colour.
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;

  struct appdata { float4 vertex : POSITION; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target { return tex2D(_MainTex, i.uv); }
  ENDCG
 }
}
}
