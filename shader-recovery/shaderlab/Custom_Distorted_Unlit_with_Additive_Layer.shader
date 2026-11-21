Shader "Custom/Distorted/Unlit with Additive Layer" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
 _ShiftColor ("Shift Color", Float) = 0
}
SubShader {
 Pass {
  GpuProgramID 39063
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
=
4
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
z_Color;
float _Shift
sampler2D
D
$
-
1
mix (texture2D (p
,[
#),
,N
(
)[
FragData[0]^
*
#
.
8
P
MultiplyDistortedTeleboardFX
