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
  GpuProgramID 63899
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
H
L
8
8
,
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
O
vDistort
dmedium
MainTex_ST"
lowp float _Falloff;
varying
<
xlv_COLOR
y
2
TEXCOORD0;
void main ()
{
 e
	normalized_distance_1;
tmpvar_2
N = (
( *`
)1
 .x3
6+ (
6z **
Ez) *=
:.x)H
yH
yH
6y))
*
3`
3^
5z /
>
/
) =J
9
4`
p4.yzw =
t3(0.0,
)&
.
1.0 - (u
*
gl_Posit
=
:
) =
	4
}
(
X.xy *
i.xy) +
zw);
}
