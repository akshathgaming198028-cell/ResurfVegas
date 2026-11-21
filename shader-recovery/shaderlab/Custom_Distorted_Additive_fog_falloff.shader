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
  GpuProgramID 3207
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
l
4
8
&
,
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
odelviewU
}
vDistort
dmedium
MainTex_ST;
varying low
xlv_COLOR
\
2
TEXCOORD0;
void main ()
{
tmpvar_1;
N = (
( *M
)1
 .x3
6+ (
6z **
Ez) *
:.x)H
yH
yH
yH
Tfloat)
%2;
3w
3 = clamp (((
    -((
0.z)3
- 300.0) / 5
, 0.0, 1.0
92 =
4-
p4.yzw =
!3(N
)&
}
U1.0 -<
2+
2
gl_Posit6
=
K
) =
	E
c
(0
X.xy *
i.xy) +
zw);
}
