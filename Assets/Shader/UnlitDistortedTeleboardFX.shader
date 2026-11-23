Shader "Custom/Distorted/Unlit TeleboardFX" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Scale ("Scale", Range(0,1)) = 1
 _Offset ("Offset", Range(0,1000)) = 0
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ColorMask 0
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp vec4 _Distort;
					void main ()
					{
					  highp vec4 tmpvar_1;
					  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
					  tmpvar_1.x = (tmpvar_1.x + ((tmpvar_1.z * tmpvar_1.z) * _Distort.x));
					  tmpvar_1.y = (tmpvar_1.y + ((tmpvar_1.z * tmpvar_1.z) * _Distort.y));
					  gl_Position = tmpvar_1;
					}
					
					
					#endif
					#ifdef FRAGMENT
					void main ()
					{
					  gl_FragData[0] = vec4(1.0, 1.0, 1.0, 1.0);
					}
					
					
					#endif




ENDGLSL

 }
 Pass {
  Tags { "QUEUE"="Transparent" }
  Blend One One
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					uniform highp vec4 _ProjectionParams;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp vec4 _Distort;
					uniform highp float _Scale;
					uniform highp float _Offset;
					varying lowp vec4 xlv_COLOR;
					varying highp vec2 xlv_TEXCOORD0;
					void main ()
					{
					  highp vec4 tmpvar_1;
					  lowp vec4 tmpvar_2;
					  highp vec2 tmpvar_3;
					  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
					  tmpvar_1.x = (tmpvar_1.x + ((tmpvar_1.z * tmpvar_1.z) * _Distort.x));
					  tmpvar_1.y = (tmpvar_1.y + ((tmpvar_1.z * tmpvar_1.z) * _Distort.y));
					  highp vec4 o_4;
					  highp vec4 tmpvar_5;
					  tmpvar_5 = (tmpvar_1 * 0.5);
					  highp vec2 tmpvar_6;
					  tmpvar_6.x = tmpvar_5.x;
					  tmpvar_6.y = (tmpvar_5.y * _ProjectionParams.x);
					  o_4.xy = (tmpvar_6 + tmpvar_5.w);
					  o_4.zw = tmpvar_1.zw;
					  tmpvar_3.x = o_4.x;
					  tmpvar_3.y = ((o_4.y + _Offset) * _Scale);
					  gl_Position = tmpvar_1;
					  xlv_COLOR = tmpvar_2;
					  xlv_TEXCOORD0 = tmpvar_3;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform highp vec4 _MainColor;
					uniform sampler2D _MainTex;
					varying highp vec2 xlv_TEXCOORD0;
					void main ()
					{
					  mediump vec4 texcol_1;
					  lowp vec4 tmpvar_2;
					  tmpvar_2 = texture2D (_MainTex, xlv_TEXCOORD0);
					  highp vec4 tmpvar_3;
					  tmpvar_3 = (tmpvar_2 * _MainColor);
					  texcol_1 = tmpvar_3;
					  gl_FragData[0] = texcol_1;
					}
					
					
					#endif




ENDGLSL

 }
}
}