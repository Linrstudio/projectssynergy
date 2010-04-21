
struct VSin
{
    float4 Position :POSITION;
    float2 UV		:TEXCOORD;
};

struct PSin
{
    float4 Position	:POSITION;
    float2 UV		:TEXCOORD;
};

texture2D DiffuseMap;
sampler2D DiffuseSampler=sampler_state{Texture = <DiffuseMap>;MinFilter=Linear;MagFilter=Linear;MipFilter=Linear;};

float3x3 View;

PSin MainVS(VSin input)
{
    PSin output;
	output.Position=float4(0,0,1,1);
	output.Position.xy=mul(float3(input.Position.xy,1),View);
	output.UV=input.UV;
    return output;
}

float4 MainPS(PSin input):COLOR
{
	float4 diffuse=tex2D(DiffuseSampler,input.UV);
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