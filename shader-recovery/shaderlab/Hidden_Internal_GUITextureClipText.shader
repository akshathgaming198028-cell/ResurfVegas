Shader "Hidden/Internal-GUITextureClipText" {
Properties {
 _MainTex ("Texture", 2D) = "white" { }
}
SubShader {
 Tags { "ForceSupported"="true" }
 Pass {
  Tags { "ForceSupported"="true" }
  ZTest Always
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 29644
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
$
p
8
,6
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
odelviewU
}
MainTex_ST
4low
h
_GUIClipTextureMo
;
varyingD
xlv_COLOR
2
TEXCOORD0"
1;
void main ()
{
  gl_Posit
>= ('
( *
F);
2
# *
%
)
(
X.xy *G
i.xy) +
-zwP
1y
C
O * (
).xy;
}
#endif
FRAGMENT
sampler2D
;
a
j
Pcol_17
f.xyz =
wN
wD
t
D2D (
,
))
(
/
r1).w));q
FragData[0] =
3
D
.
p
/
8
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
Planes[6].
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
P
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
9
FadeCenterAndType|
$
+
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
(inE
in_POSITION0
X
"in
04
2
*
7out5
%vs5
6
,vs6
[
J
!t0
82 tm
l
0  t
@.yyy
`
F[1];4
J0] *L
axxxx +
9
29
Gzzzz9
B
3B
GwwwwB
Q
& =
;
Z
'
#
@
!
@
t_
a = t0.
1[1]n
q
1
)
"0]
&
19
29
9
C
3C
/wwC
I
`return
v
~
Qpreci
mfloat;
;int
!
/inR
/inQ
1;
layout(loca
:0)
SV_TargeX
 t16_0;
+
0
+
$ =
E).w;2
12
O
9
6k
A
*
*S
<
$.w(
<
U
D
D
P
Internal-GUITexture
