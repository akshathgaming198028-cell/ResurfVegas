Shader "Custom/Distorted/Unlit with overlay" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Color ("Color Tint", Color) = (0,0,0,0)
}
SubShader {
 Pass {
  GpuProgramID 60849
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
(
8
&
,
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
ClowpN
SkyGradientTopColor(
oBottom+
FogSilhouette'
YfloatS
lOffset
O
gDistor
dmedium
MainTex_ST;
varying
xlv_COLOR
?
2
TEXCOORD0A
!
1;
void main ()
{
screenPosY_1;
tmpvar_2;2
3
4/
5
Q
N = (
( *C
)1
 .x3
6+ (
6z **
Ez) *
:.x)H
yH
yH
yH
4F
(
X.xy *
i.xy) +
6zw)h
W
6c
6
/
@
/ =C
5.xyz = mix (
#
,P
vec3(clamp ((
    (y
-
)(
"/ .
?1.0%
), 0.0, 1.0)`
7,
8
D8 =
Q(1100t
0
@ / 3
q
@
7@
8@
,.
0
W
)
9f
9
8
5
Y5.w =X
gl_Posit
=
W
) =
	O
4
h
5;
}
