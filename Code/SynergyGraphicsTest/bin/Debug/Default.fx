
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

float2x3 View;

PSin MainVS(VSin input)
{
    PSin output;
    //output.Position = mul(float4(input.Position.xyz,1),mul(World,mul(View,Projection)));
	//output.Position=input.Position*float4(1,-1,1,1);
	output.Position=float4(0,0,1,1);
	output.Position.xy=mul(input.Position.xy,View).xy;
	output.UV=input.UV;
    return output;
}

float4 MainPS(PSin input):COLOR
{
	float4 diffuse=tex2D(DiffuseSampler,input.UV);
	
	//diffuse.rgb*=1+(max(0,diffuse.r+diffuse.g+diffuse.b-2)*3);
	
	return diffuse;
}

technique Main
{
    pass Pass1
    {
        VertexShader= compile vs_3_0 MainVS();
        PixelShader = compile ps_3_0 MainPS();
    }
}