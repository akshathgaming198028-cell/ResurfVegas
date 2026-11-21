Shader "Custom/Distorted/Transparent" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _MainColor ("Color (RGBC)", Color) = (1,1,1,0)
}
SubShader {
 Tags { "QUEUE"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 12160
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 1
}
}
 }
}
}
8
&
,
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
O
vDistort
dmedium
MainTex_ST;
varying low
xlv_COLOR
?
2
TEXCOORD0;
void main ()
{
tmpvar_1;
 a
2
	Z
3
;
N = (
( *P
)1
 .x3
6+ (
6z **
Ez) *-
:.x)H
yH
yH
yH
3F
(
X.xy *
i.xy) +
!zwK
gl_Positi
=U
) =W
' =Z
}
#endif
FRAGMENT-
sampler2D
$
1
texture2D (k
,V
FragData[0]
8
P
AdditiveConstantColor
