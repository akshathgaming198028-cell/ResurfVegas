Shader "Hidden/Unlit/Transparent Masked 2" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" { }
 _Mask ("Alpha (A)", 2D) = "white" { }
}
SubShader {
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  Offset -1, -1
  GpuProgramID 2474
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
SubShader {
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  GpuProgramID 71517
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 2
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
t
|
8
,D
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
_Color
MultiTexCoord0$
1;
uniform highp mat4 glstate_matrix_mvp'
O
ClipRange0
1
wArgs1;
tmpvar_1;
varying4
2 xlv_TEXCOORD0"
D
4"
2"
7low!
COLOR;
void main ()
{
.xy = ((
*
i.zw) +
hxy);
Pret_2
..xV
' *+
i.w) - !
y!
6z))O
yO
z
O
wO
!zwS
_
1
1
gl_Posit
>= (`
( *
4
/ =
<.xy*
1*
1*
72 =k
) =h
}
#endif
FRAGMENT
sampler2D _MainT
/sk
0(
cmedium)
[col_1
factor_2
~
3_
3\
(1.0, 1.0
Iabs(
Y
)
Z
l
I
*zwI
4
4_
texture2D (
,
Z
:
V
) =T
Tfloat
5l
5 = clamp (min
0
5.x,<
Q.y),
dy)), 0
.
*v
6
6
*sk
1g
46.w"
FragData[0] =H
~
P
.
|
8
/j
f
(
?_ST
0G
,1;
2$
2
P
*
4*
Q
91.0
4xyz
z
0
	g
/((i
( *`
%
) +
/zw
)^
$
V
0
^
4
P
