//#################################################################################################
//
//
// Simple shader for drawing texture mapped and colored primitives. Nuff said.
//
//
//#################################################################################################

//#################################################################################################
// Uniform variables
//#################################################################################################

/// Transform matrix: converts from object space to the cameras screen space

uniform extern float4x4 WorldViewProjection;

/// Texture to draw

uniform extern texture Texture;

//#################################################################################################
// Samplers
//#################################################################################################

sampler Sampler_Texture = sampler_state
{
	texture	= <Texture>;
};	

//#################################################################################################
// Structures
//#################################################################################################

struct PixelShaderOutput
{
	float4	color		: COLOR0;
};

struct PixelShaderInput
{	
	float2	texcoord	: TEXCOORD0;
	float4	color		: COLOR0;
};

struct VertexShaderOutput
{
	float4	position	: POSITION;	
	float2	texcoord	: TEXCOORD0;
	float4	color		: COLOR0;
};

struct VertexShaderInput
{
	float4	position	: POSITION0;
	float2	texcoord	: TEXCOORD0;
	float4  color		: COLOR0;
};

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// Vertex shader
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

VertexShaderOutput vertexShader( VertexShaderInput input )
{
	// Declare output 

	VertexShaderOutput output = (VertexShaderOutput)(0);
	
	// Transform vertex by the camera transform matrix and save:
	
	output.position = mul( input.position , WorldViewProjection );
	
	// Save texture coordinate and color:
	
	output.texcoord = input.texcoord;
	output.color	= input.color;
	
	// Return output
	
	return output;
}

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// Pixel shader
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

PixelShaderOutput pixelShader( PixelShaderInput input )
{
	// Declare output 

	PixelShaderOutput output = (PixelShaderOutput)(0);

	// Save output texture mapped pixel:
	
	output.color = tex2D( Sampler_Texture , input.texcoord ) * input.color;
	
	// Return output:
	
	return output;
}

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// Shader techniques
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

technique ShaderTechnique
{
	//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
	// Pass 1
	//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
	
	pass Pass1 
	{
		vertexShader	= compile vs_2_0 vertexShader();
		pixelShader		= compile ps_2_0 pixelShader();
	}	

};