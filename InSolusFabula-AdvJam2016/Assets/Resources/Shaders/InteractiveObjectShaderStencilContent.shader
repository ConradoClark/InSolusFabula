Shader "InSolusFabula/Unlit-StencilContent"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color - Tint", Color) = (1,1,1,1)
		_Saturation("Color - Saturation", Range(-1,1)) = 0
		_Hue("Color - Hue Shifting", Range(0,255)) = 0
		_Luminance("Color - Luminance", Range(-1,1)) = 0
		_Opacity("Color - Opacity", Range(0,1)) = 1
		_Colorize("Color - Colorize",Color) = (1,1,1,0)
		[MaterialToggle] _Invert("Color - Invert Colors",float) = 0
		_HAutoScroll("Scroll - Horizontal Scrolling Speed", float) = 0
	    _VAutoScroll("Scroll - Vertical Scrolling Speed", float) = 0
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent+1" "IgnoreProjector" = "True" "RenderType" = "Transparent+1" }
		ZWrite Off
		Cull Off
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
		Pass
	{
		Stencil{
			Ref 1
			Comp LEqual
		}

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float2 screenPos:TEXCOORD2;
		UNITY_FOG_COORDS(1)
			float4 vertex : SV_POSITION;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float4 _MainTex_TexelSize;
	fixed4 _Color;
	half _Saturation;
	half _Hue;
	half _Luminance;
	half _Opacity;
	fixed4 _Colorize;
	half _Invert;
	float _HAutoScroll;
	float _VAutoScroll;
	half _FadeInStripsHorizontal;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
		o.vertex = UnityPixelSnap(o.vertex);
		o.screenPos = ComputeScreenPos(o.vertex);

		o.uv = TRANSFORM_TEX(v.uv, _MainTex) - float2(_Time[1] * _HAutoScroll, _Time[1] * _VAutoScroll);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		float pi = 3.14159265358979323846;
	float U = cos(_Hue*pi / 180);
	float W = sin(_Hue*pi / 180);

	fixed4 col = tex2D(_MainTex, i.uv) * _Color;
	fixed4 colCopy = col;

	col.r = (.299 + .701*U + .168*W)*colCopy.r
		+ (.587 - .587*U + .330*W)*colCopy.g
		+ (.114 - .114*U - .497*W)*colCopy.b;
	col.g = (.299 - .299*U - .328*W)*colCopy.r
		+ (.587 + .413*U + .035*W)*colCopy.g
		+ (.114 - .114*U + .292*W)*colCopy.b;
	col.b = (.299 - .3*U + 1.25*W)*colCopy.r
		+ (.587 - .588*U - 1.05*W)*colCopy.g
		+ (.114 + .886*U - .203*W)*colCopy.b;

	half sat = saturate(Luminance(col.rgb));
	col = fixed4(sat * _Colorize.r* _Colorize.a + col.r*(1 - _Colorize.a),
		sat * _Colorize.g * _Colorize.a + col.g*(1 - _Colorize.a),
		sat * _Colorize.b * _Colorize.a + col.b*(1 - _Colorize.a), col.a);

	col.rgb = lerp(col.rgb, fixed3(sat, sat, sat), -_Saturation);

	col.rgb = _Luminance >= 0 ? lerp(col.rgb, fixed3(1, 1, 1), _Luminance)
		: lerp(col.rgb, fixed3(0, 0, 0), -_Luminance);

	col.a *= _Opacity;

	col.rgb = !_Invert ? col.rgb : 1 - col.rgb;
	return col;
	}

		ENDCG
	}
	}
}