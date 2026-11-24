Shader "Mobile/Particles/Additive" {
Properties {
 _MainTex ("Particle Texture", 2D) = "white" { }
}
SubShader {
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha One
  // Port of the original base variant (scene fog is off). out = tex * clamp(vertexColor, 0, 1). No curve.
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;

  struct appdata { float4 vertex : POSITION; float4 texcoord : TEXCOORD0; float4 color : COLOR; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 color : COLOR; };

  v2f vert(appdata v) {
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.color = saturate(v.color);
    o.uv = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    return tex2D(_MainTex, i.uv) * i.color;
  }
  ENDCG
 }
}
}
