Shader "StrayTech/Grid" 
{
	Properties 
	{
		_Glossiness ("Smoothness", Range(0, 1)) = 0.5
		_Metallic ("Metallic",  Range(0,1)) = 0.5

		_GridThickness ("Grid Thickness", Range(0, 10)) = 0.01
		_GridSpacing ("Grid Spacing", Float) = 10.0
		_GridColor ("Grid Color", Color) = (0.5, 1.0, 1.0, 1.0)
		_BaseColor ("Base Color", Color) = (0.0, 0.0, 0.0, 0.0)
	}

	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		struct Input 
		{
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;

		half _GridThickness;
		half _GridSpacing;
		fixed3 _GridColor;
		fixed3 _BaseColor;

		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			float3 grid = abs(frac(IN.worldPos * _GridSpacing - 0.5001) - 0.5001) / fwidth(IN.worldPos * _GridSpacing * _GridThickness);
			float gridLine = min(min(grid.x, grid.y), grid.z);
			float c = 1.0 - min(gridLine, 1.0);
			o.Albedo = lerp(_BaseColor, _GridColor, c);

			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1.0f;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}