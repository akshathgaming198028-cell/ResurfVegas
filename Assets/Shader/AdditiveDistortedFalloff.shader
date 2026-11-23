Shader "Custom/Distorted/Additive (falloff)" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
 _Falloff ("Falloff Distance", Float) = 200
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Blend One One
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp vec4 _Distort;
					uniform mediump vec4 _MainTex_ST;
					uniform lowp float _Falloff;
					varying lowp vec4 xlv_COLOR;
					varying highp vec2 xlv_TEXCOORD0;
					void main ()
					{
					  lowp float normalized_distance_1;
					  highp vec4 tmpvar_2;
					  tmpvar_2 = (glstate_matrix_mvp * _glesVertex);
					  tmpvar_2.x = (tmpvar_2.x + ((tmpvar_2.z * tmpvar_2.z) * _Distort.x));
					  tmpvar_2.y = (tmpvar_2.y + ((tmpvar_2.z * tmpvar_2.z) * _Distort.y));
					  highp float tmpvar_3;
					  tmpvar_3 = (tmpvar_2.z / _Falloff);
					  normalized_distance_1 = tmpvar_3;
					  lowp vec4 tmpvar_4;
					  tmpvar_4.yzw = vec3(0.0, 0.0, 0.0);
					  tmpvar_4.x = (1.0 - (normalized_distance_1 * normalized_distance_1));
					  gl_Position = tmpvar_2;
					  xlv_COLOR = tmpvar_4;
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform lowp vec4 _MainColor;
					uniform sampler2D _MainTex;
					varying lowp vec4 xlv_COLOR;
					varying highp vec2 xlv_TEXCOORD0;
					void main ()
					{
					  lowp vec4 tmpvar_1;
					  tmpvar_1 = ((texture2D (_MainTex, xlv_TEXCOORD0) * _MainColor) * xlv_COLOR.x);
					  gl_FragData[0] = tmpvar_1;
					}
					
					
					#endif




ENDGLSL

 }
}
}