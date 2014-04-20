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

/// Texture to draw

uniform extern texture Texture;

// Current intensity of the effect- from 0-1

uniform extern float Intensity;

//#################################################################################################
// Constants
//#################################################################################################

// Amount from 0-1 to darken the image as it gets blurred

const float BLUR_DARKEN = 1.0f;

// Size of the blur kernel:

const float KERNEL_SIZE = 0.005f;

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
};

struct VertexShaderOutput
{
	float4	position	: POSITION;	
	float2	texcoord	: TEXCOORD0;
};

struct VertexShaderInput
{
	float4	position	: POSITION0;
	float2	texcoord	: TEXCOORD0;
};

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// Vertex shader
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

VertexShaderOutput vertexShader( VertexShaderInput input )
{
	// Declare output 

	VertexShaderOutput output = (VertexShaderOutput)(0);
	
	// Save output
	
	output.position.xyz	= input.position.xyz;
	output.position.w	= 1.0;
	output.texcoord		= input.texcoord;
	
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
	
	output.color = tex2D( Sampler_Texture , input.texcoord );
	
	// Take four other samples of the texture:
	
	float4 s1 = tex2D( Sampler_Texture , input.texcoord + float2( - KERNEL_SIZE , + KERNEL_SIZE ) );
	float4 s2 = tex2D( Sampler_Texture , input.texcoord + float2( + KERNEL_SIZE , + KERNEL_SIZE ) );
	float4 s3 = tex2D( Sampler_Texture , input.texcoord + float2( + KERNEL_SIZE , + KERNEL_SIZE ) );
	float4 s4 = tex2D( Sampler_Texture , input.texcoord + float2( - KERNEL_SIZE , - KERNEL_SIZE ) );
	
	// Produce an average color:
	
	float4 s_avg = ( s1 + s2 + s3 + s4 ) * 0.25f;
	
	// Make grey:
	
	float grey_col = ( s_avg.r + s_avg.g + s_avg.b ) * 0.3333333333;
	
	s_avg.xyz *= 1.0 - Intensity;
	s_avg.xyz += Intensity * grey_col;
	
	// Mix between average color and real color based in intensity:
	
	output.color.xyz *= 1.0 - Intensity;
	output.color.xyz += Intensity * s_avg;
	
	// Darken image as it blurs more:
	
	output.color.xyz *= 1.0f - BLUR_DARKEN * Intensity;

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