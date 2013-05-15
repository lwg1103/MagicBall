float4x4 gWorld;
float4x4 gView;
float4x4 gProj;
float4x4 gTrans;
float gOpacity;

struct VS_IN
{
	float4 pos : POSITION;
};

struct PS_IN
{
	float4 pos : SV_POSITION;
};

PS_IN vs_main(VS_IN input)
{
	PS_IN output = (PS_IN)0;

	input.pos.w = 1.0f;

	output.pos = mul(mul(mul(input.pos, mul(gWorld, gTrans)), gView), gProj);

	return output;
}

// SOLID TECHNIQUES

float4 ps_white(PS_IN input) : SV_Target
{
	return float4(1, 1, 1, gOpacity);
}

float4 ps_yellow(PS_IN input) : SV_Target
{
	return float4(1, 1, 0, gOpacity);
}

float4 ps_red(PS_IN input) : SV_Target
{
	return float4(1, 0, 0, gOpacity);
}

float4 ps_blue(PS_IN input) : SV_Target
{
	return float4(0, 0, 1, gOpacity);
}

technique11 SolidWhite
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, vs_main()));
		SetPixelShader(CompileShader(ps_4_0, ps_white()));
	}
}

technique11 SolidYellow
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, vs_main()));
		SetPixelShader(CompileShader(ps_4_0, ps_yellow()));
	}
}

technique11 SolidRed
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, vs_main()));
		SetPixelShader(CompileShader(ps_4_0, ps_red()));
	}
}

technique11 SolidBlue
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, vs_main()));
		SetPixelShader(CompileShader(ps_4_0, ps_blue()));
	}
}

// POINT LIGHT TECHNIQUES

float4 ps_blue_pl(PS_IN input) : SV_Target
{
	// TODO: Implement Point Light
	return float4(0, 0, 1, gOpacity);
}

technique11 PointLightBlue
{
	pass P0
	{
		SetGeometryShader(0);
		SetVertexShader(CompileShader(vs_4_0, vs_main()));
		SetPixelShader(CompileShader(ps_4_0, ps_blue_pl()));
	}
}