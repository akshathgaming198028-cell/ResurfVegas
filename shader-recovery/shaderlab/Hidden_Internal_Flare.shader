Shader "Hidden/Internal-Flare" {
SubShader {
 Tags { "RenderType"="Overlay" }
 Pass {
  Tags { "RenderType"="Overlay" }
  ZTest Always
  ZWrite Off
  Cull Off
  Blend One One
  ColorMask RGB
  GpuProgramID 33804
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
P
X
$
X
8
n
'
#ifdef VERTEX
#version 300 es
uniform 	vec4 _Time;
?Sin
?Cos
unity_Delta
3 _WorldSpaceCameraPos[
ProjectionParam!
_Scree
ZBuffer
_Ortho!
ClipPlanes[6].
Zmat4x0
(
?Inv+
"
LightPos0$
iB
ORang
4)
/X0!
Y!
Z!
mediump )
_Atten*
)
Color[8]v
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
OData
FadeCenterAndType|
glstate_matrix_mvp$
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
3low
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
1Fog
?FogI
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
_FlareTexture_~
sin highI
in_POSITION0
#
in_COLOR04
2
TEXCOORD0;
out5
%vs5
6
(vs6
-
t
oid main()
{
    t0 =
.yyyy *
F[1];4
J0] *L
axxxx +{
9
29
Gzzzz9
4gl_
B
3B
GwwwwB
+
( =l
,
2.xy!
*
+
"zwS
return;
}
#endif
9
FRAGMENT;
Qpreci
mfloat;
i
sampler2D
^
.vs^
(
layout(locak
:0) s
SV_TargeX
	%
@t16_C
*
Ot10_s
B = t
(
,
);
"165
=
*
$ =
3
8
E
Z100
B
attribute
glesVertex;
:
MultiTexCoord
wvarying
2xlv
V
(
( *
r
) =
 n
P
(
)
)4
6
~
2
?
W
pmpvar_1
N
<2D Q
	\
6) *
");
@Frag0
=b
8
8
P
Internal-Halo
