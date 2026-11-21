Shader "Hidden/Internal-Colored" {
Properties {
 _Color ("Color", Color) = (1,1,1,1)
 _SrcBlend ("SrcBlend", Float) = 5
 _DstBlend ("DstBlend", Float) = 10
 _ZWrite ("ZWrite", Float) = 1
 _ZTest ("ZTest", Float) = 4
 _Cull ("Cull", Float) = 0
 _ZBias ("ZBias", Float) = 0
}
SubShader {
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZTest [_ZTest]
  ZWrite [_ZWrite]
  Cull [_Cull]
  Blend [_SrcBlend] [_DstBlend]
  Offset [_ZBias], [_ZBias]
  GpuProgramID 4385
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
SubProgram "gles3 " {
GpuProgramIndex 1
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 2
}
SubProgram "gles3 " {
GpuProgramIndex 3
}
}
 }
}
}
H
$
8
n
,
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
Color;
uniform highp mat4 glstate_matrix_mvp'
F
B
varying low
xlv_COLOR;
void main ()
{
 &
tmpvar_1;
F = (
# *e
)$
V
) =<
gl_Posit
>= (
( *
);
}
#endifD
FRAGMENT
gl_FragData[0] =-
e
J
e300 es
\Time;
?Sin
?Cos
unity_Delta
3 _WorldSpaceCameraPos[
`Projec
^Param!
_Scree
ZBuffer
_Ortho!
ClipPlanes[6].
x0
(
?Inv+
"
LightPos0$
ORang
4)
/X0!
Y!
Z!
mediump )
_Atten*
)
?[8]v
&
Q
z
Q
SpotDirecQ
OSHAr"
g"
b"
Bf
Bf
Bf
C!
3
0)
1)
2)
3
hadowSplitSpheres[4]+
oqRadii[
-
OBias[
G
_sNear
	?Far
2g
/[4o
FadeCenterAndType|
x
$
odelview0+
invtrans4
0_Ob
2
%
 OD
?
T
Opose
p
.
l
W
_ambient
MX
V
Pn
#
Aa
?Sky%
Equator)
oGround(
;Fog
I
_mapST)
oynamic'
SpecCube0_BoxMax&
/in&
TProbef
F
5
?HDRX
1
1
1
!
1
a
OGrey)
oDouble
.
ielectric
6
Luminance
;map
*
\
(in
in_POSITION0
g0;
out
%vs
0
f  t0 =D
;
C
)0;1
.yyyy *
7[1]4
J0] *L
jxxxx +m
9
29
Gzzzz9
B
B
3B
GwwwwB
`return
X
Qpreci
mfloat;
1int
layout(loca
:0)
SV_Targe
( =Y
!
8
8
P
Internal-SplashShadowCaster
\
