Shader "Hidden/Internal-Halo" {
SubShader {
 Tags { "RenderType"="Overlay" }
 Pass {
  Tags { "RenderType"="Overlay" }
  ZWrite Off
  Cull Off
  Blend OneMinusDstColor One
  ColorMask RGB
  GpuProgramID 31992
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
SubProgram "gles3 " {
GpuProgramIndex 1
}
SubProgram "gles " {
GpuProgramIndex 2
}
SubProgram "gles3 " {
GpuProgramIndex 3
}
SubProgram "gles " {
GpuProgramIndex 4
}
SubProgram "gles3 " {
GpuProgramIndex 5
}
SubProgram "gles " {
GpuProgramIndex 6
}
SubProgram "gles3 " {
GpuProgramIndex 7
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 8
}
SubProgram "gles3 " {
GpuProgramIndex 9
}
SubProgram "gles " {
GpuProgramIndex 10
}
SubProgram "gles3 " {
GpuProgramIndex 11
}
SubProgram "gles " {
GpuProgramIndex 12
}
SubProgram "gles3 " {
GpuProgramIndex 13
}
SubProgram "gles " {
GpuProgramIndex 14
}
SubProgram "gles3 " {
GpuProgramIndex 15
}
}
 }
}
}
pS
?
@
L
#
h(
C
=
x-
C
8
=
H
=
D
"<C
"4-
j
FOG_EXP
D
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
_HaloFalloff_}
sin highH
in_POSITION0
"
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
"zwQ
return;
}
#endif
6
FRAGMENT8
Qpreci
mfloat;
i
sampler2D
[
.vs[
%
layout(locah
:0) p
SV_TargeU
_ t10_]
 = texture(
,x
#).?
f
a.xyz =
(L
6) *
2
4w =
O
(
@
LINEAR
1*
/t0!
$
c
t
1
L.z *
.
wx
/in
B
)
3
2
x
clamp(t0, 0.0, 1.0)
?1.w
A
/ww
)t1
<
L
1
0
H
8
E
Z100
z
attribute
glesVertex;
r
MultiTexCoord
T
wvarying
2xlv
%
(
( *
r
) =
 n
P
(
)
)
3
J
/
?
H
qtmpvar_2
;2D
	Z
/);L
2L
(
*l
9.w)/
F
;
@Frag
*
=g
R
/
L
/
\
P	"
h
#
1t
u
K
]
X
(
)
k
k
_col_1
Q
,
3L
3Q
2Q
3Q
2Q
3
4K
4
	) (!
^
*
O
/))
S
"
'
/
exp2(-((
y
?z))
p
"
28
D
4+
x%
`x * (-
)
k
L
/
4=0
H
x
$(-
'2)
3
%
D
y
P
Internal-GUITextureClip
