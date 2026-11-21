Shader "Sprites/Default" {
Properties {
[PerRendererData]  _MainTex ("Sprite Texture", 2D) = "white" { }
 _Color ("Tint", Color) = (1,1,1,1)
[MaterialToggle]  PixelSnap ("Pixel snap", Float) = 0
}
SubShader {
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="true" }
  ZWrite Off
  Cull Off
  Blend One OneMinusSrcAlpha
  GpuProgramID 48048
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
PR
.
>
<$
<
B
l
=
x'
8
,
X
LB
8
X7
P
4=
P
H\
#
P
7
`
B
j
PIXELSNAP_ON
H
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
_
sin high
in_POSITION0
[COLOR
2
TEXCOORD0;
out g
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
Lwwww{
e
% *
H
% =1
B
Y.xy =
2.xy'
return;
}
#endif
FRAGMENT!
Qpreci
mfloat;
i
sampler2D _MainTex+
/in
layout(locaP
:0) X
SV_Targe
Ot10_D
;
texture(
,E
*);0
*
~
D.xyz(
# *
=xyz,
w*
?
ETC1_EXTERNAL_ALPHAT
/
/t1%
/t0
&
/
$ww
*L
(0.5, 0.5)J
)
hEven(t
9
$
0$
"zw$
/zw
!
_Alpha
U
<
%
&
3
4
\
Y
Z
X
L
(
X
E
;
Z100
V
attributeO
glesVert
MultiTexCoord
0
wvarying
2xlv
~
6
 7
X
pmpvar_1
F = (
S
2
(
( *;
4
) =p
/ =<
?}
3
R
I
c
=
 _2
72D
	i
9
w7
8
;
3
3
X
& *
Vc_1.w
43.w
(
*(
&
@Frag
=
4
p#4
$
X
2F
22
2
2
2
1
h
'
@
@
l
'
*
`
 /
t
]
1
^
2pos
'.z
1
4i
4i
X
((floor(Q
5(((h
v
&w)o
K4) +
w)
  ) /"
*<
5
R
5
2A
@
H
Ed
!
8
$
L
/
0B
62
|
<
P
Sprites-Default
*
_DecalTex
?
?
_MainTex
?
?
PixelSnap
_Color
?
?
?
?
FrameDebuggerRenderTargetDisplay
