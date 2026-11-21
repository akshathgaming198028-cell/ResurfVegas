Shader "Unlit/Transparent" {
Properties {
 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }
}
SubShader {
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Blend SrcAlpha OneMinusSrcAlpha
  GpuProgramID 50158
Program "vp" {
SubProgram "gles " {
GpuProgramIndex 0
}
SubProgram "gles " {
GpuProgramIndex 1
}
}
Program "fp" {
SubProgram "gles " {
GpuProgramIndex 2
}
SubProgram "gles " {
GpuProgramIndex 3
}
}
 }
}
}
$
d
l
8
H
&
,d
#version 100
#ifdef VERTEX
attribute vec4 _glesVertex;
&MultiTexCoord0;
uniform highp mat4 glstate_matrix_mvp'
O
MainTex_ST;
varying medium"
2 xlv_TEXCOORD0;
void main ()
{
 -
tmpvar_1;
_ = ((
X.xy *
i.xy) +
0zw)K
gl_Posit?
>= (
( *5
4
' =
}
#endif
FRAGMENT"
sampler2D
$3lowj
texture2D (k
,V
FragData[0]
	|
FOG_LINEAR
unity_FogParams
4
$
D
Yfloat#
1
p
2
=
;
>
!
2
:
L.z *
 .z}
/w)k
"
oColor;
G
2col(
2
Q
'.wj
 .w
3b
3 = clamp (
, 0.0, 1.^
xyz = mix (N
,y
Dvec3
/3)g
d
d
H
/
8
P
