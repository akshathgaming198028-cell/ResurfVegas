Shader "Bend/UV Scroll (Screen)/Simple" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _ScrollSpeed ("Scroll speed (xy)", Vector) = (1,1,0,0)
}
SubShader {
 Tags { "QUEUE"="Geometry" }
 Pass {
  // Port of the first SubShader of the original (the one Unity selects on this hardware).
  // Hard-coded bend constants, scrolling UV, and the sky/fog overlay. Default render states.
  Tags { "QUEUE"="Geometry" }
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _MainTex;
  float4 _MainTex_ST;
  float4 _ScrollSpeed;

  // Globals set by ThemeManager.SetShaderStates.
  float4 _SkyGradientTopColor;
  float4 _SkyGradientBottomColor;
  float4 _FogSilhouetteColor;
  float _SkyGradientOffset;

  struct appdata { float4 vertex : POSITION; float4 texcoord : TEXCOORD0; };
  struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; fixed4 fog : TEXCOORD1; };

  v2f vert(appdata v) {
    v2f o;
    float4 clip = UnityObjectToClipPos(v.vertex);
    float dd = clip.z * clip.z;
    o.pos = clip;
    o.pos.x = clip.x + dd * -0.0005;
    o.pos.y = clip.y + dd * -0.0003;
    float screenPosY = o.pos.y / clip.w;
    float3 sky = lerp(_SkyGradientBottomColor.rgb, _SkyGradientTopColor.rgb,
                      saturate((screenPosY - _SkyGradientOffset) / (1.0 - _SkyGradientOffset)));
    sky = lerp(sky, _FogSilhouetteColor.rgb, saturate((1100.0 - clip.w) / 300.0));
    o.fog = fixed4(sky, saturate((800.0 - clip.w) / 500.0));
    o.uv = (v.texcoord.xy + ((_Time.yy * _ScrollSpeed.xy) * _MainTex_ST.xy)) + _MainTex_ST.zw;
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    fixed4 t = tex2D(_MainTex, i.uv);
    fixed4 c;
    c.a = t.a;
    c.rgb = lerp(i.fog.rgb, t.rgb, i.fog.a);
    return c;
  }
  ENDCG
 }
}
}
