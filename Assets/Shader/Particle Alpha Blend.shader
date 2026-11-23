Shader "Custom/Distorted/Alpha Blended" {
Properties {
 _TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
 _MainTex ("Particle Texture", 2D) = "white" { }
 _InvFade ("Soft Particles Factor", Range(0.01,3)) = 1
}
SubShader { 
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
GLSLPROGRAM
Keywords { "SOFTPARTICLES_OFF" }
					
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec4 _glesColor;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp mat4 glstate_matrix_mvp;
					uniform lowp vec4 _SkyGradientTopColor;
					uniform lowp vec4 _SkyGradientBottomColor;
					uniform lowp vec4 _FogSilhouetteColor;
					uniform lowp float _SkyGradientOffset;
					uniform highp vec4 _Distort;
					uniform highp vec4 _MainTex_ST;
					varying lowp vec4 xlv_COLOR;
					varying mediump vec2 xlv_TEXCOORD0;
					varying lowp vec4 xlv_TEXCOORD1;
					void main ()
					{
					  lowp float screenPosY_1;
					  highp vec4 tmpvar_2;
					  mediump vec2 tmpvar_3;
					  lowp vec4 tmpvar_4;
					  tmpvar_2 = (glstate_matrix_mvp * _glesVertex);
					  tmpvar_2.x = (tmpvar_2.x + ((tmpvar_2.z * tmpvar_2.z) * _Distort.x));
					  tmpvar_2.y = (tmpvar_2.y + ((tmpvar_2.z * tmpvar_2.z) * _Distort.y));
					  tmpvar_3 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  highp float tmpvar_5;
					  tmpvar_5 = (tmpvar_2.y / tmpvar_2.w);
					  screenPosY_1 = tmpvar_5;
					  tmpvar_4.xyz = mix (_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, vec3(clamp ((
					    (screenPosY_1 - _SkyGradientOffset)
					   / 
					    (1.0 - _SkyGradientOffset)
					  ), 0.0, 1.0)));
					  lowp float tmpvar_6;
					  highp float tmpvar_7;
					  tmpvar_7 = clamp (((1100.0 - tmpvar_2.w) / 300.0), 0.0, 1.0);
					  tmpvar_6 = tmpvar_7;
					  tmpvar_4.xyz = mix (tmpvar_4.xyz, _FogSilhouetteColor.xyz, vec3(tmpvar_6));
					  highp float tmpvar_8;
					  tmpvar_8 = clamp (((800.0 - tmpvar_2.w) / 500.0), 0.0, 1.0);
					  tmpvar_4.w = tmpvar_8;
					  gl_Position = tmpvar_2;
					  xlv_COLOR = _glesColor;
					  xlv_TEXCOORD0 = tmpvar_3;
					  xlv_TEXCOORD1 = tmpvar_4;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform sampler2D _MainTex;
					uniform lowp vec4 _TintColor;
					varying lowp vec4 xlv_COLOR;
					varying mediump vec2 xlv_TEXCOORD0;
					varying lowp vec4 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec4 tmpvar_2;
					  tmpvar_2 = ((2.0 * xlv_COLOR) * (_TintColor * texture2D (_MainTex, xlv_TEXCOORD0)));
					  c_1.w = tmpvar_2.w;
					  c_1.xyz = mix (xlv_TEXCOORD1.xyz, tmpvar_2.xyz, xlv_TEXCOORD1.www);
					  gl_FragData[0] = c_1;
					}
					
					
					#endif


Keywords { "SOFTPARTICLES_OFF" }
					

ENDGLSL

 }
}
}