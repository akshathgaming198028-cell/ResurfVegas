Shader "Custom/Additive Constant Color" {
Properties {
 _MainTex ("Main Texture", 2D) = "white" { }
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  Blend One One
  // Port of the original GLES program: no bend, out = _MainColor * tex (vertex colour unused).
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  fixed4 _MainColor;

  struct appdata { float4 vertex : POSITION; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target { return _MainColor * tex2D(_MainTex, i.uv); }
  ENDCG
 }
}
}
