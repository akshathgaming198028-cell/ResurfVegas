// Port of the original "Custom/Distorted/Projector Multiply" shader (Unity 5.3 blob,
// sharedassets1.assets, shader path id 381), reconstructed from the decoded GLES
// program blob and ShaderLab text. Used by effects_tex_mat_blob shadow.mat.
// Not compiled against Unity 2019 yet; verify on device.

Shader "Custom/Distorted/Projector Multiply" {
Properties {
 _ShadowTex ("Cookie", 2D) = "gray" { }
 _Falloff ("Falloff distance", Float) = 0.01
}
SubShader {
 Tags { "RenderType"="Transparent-1" }
 Pass {
  Tags { "RenderType"="Transparent-1" }
  ZWrite Off
  Blend DstColor Zero
  ColorMask RGB
  Offset -1, -1
  CGPROGRAM
  #pragma vertex vert
  #pragma fragment frag
  #include "UnityCG.cginc"

  sampler2D _ShadowTex;
  float _Falloff;
  // Not in the original Properties block: set per material (saved as _Distort)
  // or globally by ThemeManager (Shader.SetGlobalVector).
  float4 _Distort;
  // Projector matrix (object space -> projector space), saved in the material as _Projector_0.._3.
  float4x4 _Projector;

  struct appdata {
    float4 vertex : POSITION;
  };

  struct v2f {
    float4 pos : SV_POSITION;
    float4 proj : TEXCOORD0;
    fixed4 color : COLOR;
  };

  v2f vert(appdata v) {
    v2f o;
    // Distortion is applied in clip space, as in the original (tmpvar_2.x/y += z*z*_Distort.xy).
    float4 clipPos = mul(UNITY_MATRIX_MVP, v.vertex);
    clipPos.x += clipPos.z * clipPos.z * _Distort.x;
    clipPos.y += clipPos.z * clipPos.z * _Distort.y;
    o.pos = clipPos;

    float4 p = mul(_Projector, v.vertex);
    float behind = (p.z < 0.0) ? 1.0 : 0.0;
    float fade = clamp(p.z * _Falloff, 0.0, 1.0);
    o.proj = p;
    float k = behind + fade;
    o.color = fixed4(k, k, k, k);
    return o;
  }

  fixed4 frag(v2f i) : SV_Target {
    fixed4 t = tex2Dproj(_ShadowTex, i.proj);
    // mix(shadowSample, white, k): white behind the projector, fading to white with distance.
    return lerp(t, fixed4(1.0, 1.0, 1.0, 1.0), i.color.x);
  }
  ENDCG
 }
}
}
