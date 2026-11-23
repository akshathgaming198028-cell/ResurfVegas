Shader "Custom/Distorted/Additive (fog falloff)" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
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
					uniform highp mat4 glstate_matrix_modelview0;
					uniform highp vec4 _Distort;
					uniform mediump vec4 _MainTex_ST;
					varying lowp vec4 xlv_COLOR;
					varying highp vec2 xlv_TEXCOORD0;
					void main ()
					{
					  highp vec4 tmpvar_1;
					  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
					  tmpvar_1.x = (tmpvar_1.x + ((tmpvar_1.z * tmpvar_1.z) * _Distort.x));
					  tmpvar_1.y = (tmpvar_1.y + ((tmpvar_1.z * tmpvar_1.z) * _Distort.y));
					  lowp float tmpvar_2;
					  highp float tmpvar_3;
					  tmpvar_3 = clamp (((
					    -((glstate_matrix_modelview0 * _glesVertex).z)
					   - 300.0) / 500.0), 0.0, 1.0);
					  tmpvar_2 = tmpvar_3;
					  lowp vec4 tmpvar_4;
					  tmpvar_4.yzw = vec3(0.0, 0.0, 0.0);
					  tmpvar_4.x = (1.0 - (tmpvar_2 * tmpvar_2));
					  gl_Position = tmpvar_1;
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