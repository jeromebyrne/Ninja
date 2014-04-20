//#################################################################################################
//
//
// This shader helps speed up particle code. It takes a bunch of normalised particles with no 
// transforms, colors, and basic quad texture coords and modifies them by the uniform variables 
// in the program (in a predictably random manner) to produce the final particles. Random numbers 
// which are generated at program startup are stored in color vectors to help the randomisation, 
// as well in uniform variables.
//
//
//#################################################################################################

//#################################################################################################
// Uniform variables
//#################################################################################################

/// Transform matrix: converts from object space to the cameras screen space

uniform extern float4x4 WorldViewProjection;

/// Texture use for the particles

uniform extern texture Texture;

/// Random values for the particles

uniform extern float4 ParticleSizesAndLiveTimes;				// Random particle sizes and live times. Sizes in x and y, live times in z and w.
uniform extern float4 ParticleAnglesAndSpeeds;					// Random particle movement angles and speeds. Angles in x and y, speeds in z and w.
uniform extern float4 ParticleColor1;							// Random color value for the particles - number 1	
uniform extern float4 ParticleColor2;							// Random color value for the particles - number 2
uniform extern float2 ParticleRandomSeedAndTimeAlive;			// A random seed is stored in x and time alive for the system in y.

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
	float4	position		: POSITION0;	// We hold one random value in position.z
	float2	texcoord		: TEXCOORD0;	// Just a regular texture coord
	float4  random_numbers	: COLOR0;		// Random values for generating the particles
};

//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
// Vertex shader
//@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@

VertexShaderOutput vertexShader( VertexShaderInput input )
{
	// Declare output 

	VertexShaderOutput output = (VertexShaderOutput)(0);
	
	// Get the random values and transform by the seed:
	
	float r1 = input.random_numbers.x + ParticleRandomSeedAndTimeAlive.x;
	float r2 = input.random_numbers.y + ParticleRandomSeedAndTimeAlive.x;
	float r3 = input.random_numbers.z + ParticleRandomSeedAndTimeAlive.x;
	float r4 = input.random_numbers.w + ParticleRandomSeedAndTimeAlive.x;
	float r5 = input.position.z		  + ParticleRandomSeedAndTimeAlive.x;
	
	r1 = frac(r1);
	r2 = frac(r2);
	r3 = frac(r3);
	r4 = frac(r4);
	r5 = frac(r5);
	
	// Ok: set the color for the particle:
	
	output.color = r1 * ParticleColor1 + ( 1.0 - r1 ) * ParticleColor2;
	
	// Figure out live time for the particle:
	
	float live_time = ( r2 * ParticleSizesAndLiveTimes.z ) + ( ( 1.0 - r2 ) * ParticleSizesAndLiveTimes.w );
	
	// Modify particle alpha by live time:
	
	output.color.a -= ParticleRandomSeedAndTimeAlive.y / live_time;
	
	// Right: figure out the size of the particle and modify vertex accordingly:
	
	input.position.xy *= ( r3 * ParticleSizesAndLiveTimes.x ) + ( ( 1.0 - r3 ) * ParticleSizesAndLiveTimes.y );
	
	// Figure out angle of particle travel
	
	float angle = ( r4 * ParticleAnglesAndSpeeds.x ) + ( ( 1.0 - r4 ) * ParticleAnglesAndSpeeds.y );
	
	// Figure out direction of particle travel:
	
	float2 dir;
	
	dir.x = cos( angle );
	dir.y = sin( angle );
	
	// Figure out particle speed:
	
	float speed = ( r5 * ParticleAnglesAndSpeeds.z ) + ( ( 1.0 - r5 ) * ParticleAnglesAndSpeeds.w );	
	
	// Cool: now figure out particle velocity
	
	float2 velocity;
	
	velocity.x = dir.x * speed;
	velocity.y = dir.y * speed;
	
	// Modify position with velocity and time alive:
	
	input.position.xy += velocity * ParticleRandomSeedAndTimeAlive.y;
		
	// Set z value of position:
	
	input.position.z = -2;	
	
	// Transform vertex by the camera transform matrix and save:
	
	output.position = mul( input.position , WorldViewProjection );
	
	// Save texture coordinate:
	
	output.texcoord = input.texcoord;
	
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