Shader "Custom/Distorted/Unlit with overlay and Rim and SPmask" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Mask ("Mask (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
 _RimColor ("Reflection Color", Color) = (1,1,1,0.5)
}
SubShader { 
 Pass {
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 glstate_matrix_modelview0;
					uniform highp mat4 glstate_matrix_invtrans_modelview0;
					uniform lowp vec4 _SkyGradientTopColor;
					uniform lowp vec4 _SkyGradientBottomColor;
					uniform lowp vec4 _FogSilhouetteColor;
					uniform lowp float _SkyGradientOffset;
					uniform highp vec4 _Distort;
					uniform mediump vec4 _MainTex_ST;
					varying lowp vec4 xlv_COLOR;
					varying mediump vec2 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD0;
					varying lowp vec3 xlv_TEXCOORD1;
					varying highp vec2 xlv_TEXCOORD3;
					varying lowp vec4 xlv_TEXCOORD4;
					void main ()
					{
					  lowp float screenPosY_1;
					  highp vec4 tmpvar_2;
					  lowp vec4 tmpvar_3;
					  lowp vec3 tmpvar_4;
					  lowp vec3 tmpvar_5;
					  highp vec2 tmpvar_6;
					  lowp vec4 tmpvar_7;
					  tmpvar_5 = (glstate_matrix_modelview0 * _glesVertex).xyz;
					  lowp vec4 tmpvar_8;
					  tmpvar_8.w = 0.0;
					  tmpvar_8.xyz = _glesNormal;
					  highp vec3 tmpvar_9;
					  tmpvar_9 = (glstate_matrix_invtrans_modelview0 * tmpvar_8).xyz;
					  tmpvar_4 = tmpvar_9;
					  tmpvar_2 = (glstate_matrix_mvp * _glesVertex);
					  tmpvar_2.x = (tmpvar_2.x + ((tmpvar_2.z * tmpvar_2.z) * _Distort.x));
					  tmpvar_2.y = (tmpvar_2.y + ((tmpvar_2.z * tmpvar_2.z) * _Distort.y));
					  tmpvar_6.x = (tmpvar_2.x / tmpvar_2.w);
					  tmpvar_6.y = (tmpvar_2.y / tmpvar_2.w);
					  tmpvar_6 = clamp (tmpvar_6, vec2(-1.0, -1.0), vec2(1.0, 1.0));
					  tmpvar_6 = ((tmpvar_6 * 0.5) + 0.5);
					  highp float tmpvar_10;
					  tmpvar_10 = (tmpvar_2.y / tmpvar_2.w);
					  screenPosY_1 = tmpvar_10;
					  tmpvar_7.xyz = mix (_SkyGradientBottomColor.xyz, _SkyGradientTopColor.xyz, vec3(clamp ((
					    (screenPosY_1 - _SkyGradientOffset)
					   / 
					    (1.0 - _SkyGradientOffset)
					  ), 0.0, 1.0)));
					  lowp float tmpvar_11;
					  highp float tmpvar_12;
					  tmpvar_12 = clamp (((1100.0 - tmpvar_2.w) / 300.0), 0.0, 1.0);
					  tmpvar_11 = tmpvar_12;
					  tmpvar_7.xyz = mix (tmpvar_7.xyz, _FogSilhouetteColor.xyz, vec3(tmpvar_11));
					  highp float tmpvar_13;
					  tmpvar_13 = clamp (((800.0 - tmpvar_2.w) / 500.0), 0.0, 1.0);
					  tmpvar_7.w = tmpvar_13;
					  gl_Position = tmpvar_2;
					  xlv_COLOR = tmpvar_3;
					  xlv_TEXCOORD2 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  xlv_TEXCOORD0 = tmpvar_4;
					  xlv_TEXCOORD1 = tmpvar_5;
					  xlv_TEXCOORD3 = tmpvar_6;
					  xlv_TEXCOORD4 = tmpvar_7;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform lowp vec4 _Color;
					uniform sampler2D _MainTex;
					uniform sampler2D _Mask;
					uniform bool _DebugShaders;
					varying mediump vec2 xlv_TEXCOORD2;
					varying highp vec2 xlv_TEXCOORD3;
					varying lowp vec4 xlv_TEXCOORD4;
					void main ()
					{
					  lowp vec4 tmpvar_1;
					  lowp vec2 maskCoord_2;
					  maskCoord_2 = xlv_TEXCOORD3;
					  lowp vec4 tmpvar_3;
					  tmpvar_3 = texture2D (_Mask, maskCoord_2);
					  if (_DebugShaders) {
					    tmpvar_1 = tmpvar_3;
					  } else {
					    lowp vec4 c_4;
					    lowp vec4 tmpvar_5;
					    tmpvar_5 = ((texture2D (_MainTex, xlv_TEXCOORD2) * _Color) + tmpvar_3);
					    c_4.w = tmpvar_5.w;
					    c_4.xyz = mix (xlv_TEXCOORD4.xyz, tmpvar_5.xyz, xlv_TEXCOORD4.www);
					    tmpvar_1 = c_4;
					  };
					  gl_FragData[0] = tmpvar_1;
					}
					
					
					#endif




ENDGLSL

 }
}
}