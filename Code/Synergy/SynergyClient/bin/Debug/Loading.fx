struct VSin
{
    float4 Position :POSITION;
    float4 Color	:COLOR;
};

struct PSin
{
    float4 Position	:POSITION;
    float4 Color	:TEXCOORD;
};

texture2D DiffuseMap;
sampler2D DiffuseSampler=sampler_state{Texture=<DiffuseMap>;MinFilter=Linear;MagFilter=Linear;MipFilter=Linear;};
float Progress;
float3x3 View;

PSin MainVS(VSin input)
{
    PSin output;
	output.Position=float4(0,0,1,1);
	output.Position.xy=mul(float3(input.Position.xy,1),View);
	output.Color=input.Color;
    return output;
}

float4 MainPS(PSin input):COLOR
{
	float4 diffuse=input.Color;
	return diffuse;
}

technique Main
{
    pass Pass1
    {
        VertexShader= compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();
    }
}
