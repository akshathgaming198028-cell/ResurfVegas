Shader "Custom/Distorted/Additive" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
}
SubShader { 
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  Blend One One
GLSLPROGRAM

					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp vec4 _Distort;
					uniform mediump vec4 _MainTex_ST;
					varying lowp vec4 xlv_COLOR;
					varying mediump vec2 xlv_TEXCOORD0;
					void main ()
					{
					  highp vec4 tmpvar_1;
					  lowp vec4 tmpvar_2;
					  mediump vec2 tmpvar_3;
					  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
					  tmpvar_1.x = (tmpvar_1.x + ((tmpvar_1.z * tmpvar_1.z) * _Distort.x));
					  tmpvar_1.y = (tmpvar_1.y + ((tmpvar_1.z * tmpvar_1.z) * _Distort.y));
					  tmpvar_3 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  gl_Position = tmpvar_1;
					  xlv_COLOR = tmpvar_2;
					  xlv_TEXCOORD0 = tmpvar_3;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform lowp vec4 _MainColor;
					uniform sampler2D _MainTex;
					varying mediump vec2 xlv_TEXCOORD0;
					void main ()
					{
					  lowp vec4 tmpvar_1;
					  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * _MainColor);
					  gl_FragData[0] = tmpvar_1;
					}
					
					
					#endif




ENDGLSL

 }
}
}