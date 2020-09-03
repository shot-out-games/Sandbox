// Fantasy Adventure Environment
// staggart.xyz

float4 _WindDirection;
float _TrunkWindSpeed;
float _TrunkWindSwinging;
float _TrunkWindWeight;
float _WindSpeed;
float _WindAmplitude;
float _WindStrength;

TEXTURE2D(_WindVectors); SAMPLER(sampler_WindVectors);

float WindSpeed() {
	return _WindSpeed * _TimeParameters.x * 0.25; //10x faster than legacy _Time.x
}

float4 WindDirection() {
	return _WindDirection + 0.001;
}
void GetGlobalParams_float(out float4 windDir, out float trunkSpeed, out float trunkSwinging, out float trunkWeight, out float windSpeed)
{
	windDir = WindDirection();
    trunkSpeed = _TrunkWindSpeed ;
    trunkSwinging = _TrunkWindSwinging;
    trunkWeight = _TrunkWindWeight;
    windSpeed = WindSpeed();
};

void GetLocalParams_float(in float3 wPos, in float windFreqMult, out float4 windDir, out float trunkSpeed, out float trunkSwinging, out float trunkWeight, out float windSpeed, out float windFreq, out float windStrength)
{
    windDir = WindDirection();
    trunkSpeed = _TrunkWindSpeed;
    trunkSwinging = _TrunkWindSwinging;
    trunkWeight = _TrunkWindWeight;
    windSpeed = WindSpeed();
    windFreq = (wPos.xz * 0.01) * (_WindAmplitude * windFreqMult);
    windStrength = _WindStrength;
};

float3 GetPivotPos() {
	return float3(UNITY_MATRIX_M[0][3], UNITY_MATRIX_M[1][3] + 0.25, UNITY_MATRIX_M[2][3]);
}

float ObjectPosRand01() {
	return frac(UNITY_MATRIX_M[0][3] + UNITY_MATRIX_M[1][3] + UNITY_MATRIX_M[2][3]);
}

void ApplyFoliageWind_float(in float3 wPos, in float maxStrength, in float mask, in float leafFlutter, in float globalMotion, in float swinging, in float freqMult, in out float3 positionOS, out float3 offset)
{
	float speed = WindSpeed();

	float2 windUV = (wPos.xz * 0.01) * _WindAmplitude * freqMult;
	windUV += (WindDirection().xz * (speed));

	float2 windVec = SAMPLE_TEXTURE2D_LOD(_WindVectors, sampler_WindVectors, windUV, 0).rg;

	float sine = sin(ObjectPosRand01() + WindDirection().xz * speed);
	sine = lerp(sine * 0.5 + 0.5, sine, swinging);

	windVec = maxStrength * mask * ((sine * globalMotion * 0.5) + (windVec * leafFlutter));

	offset = float3(windVec.x, 0, windVec.y) + positionOS;
};

void SampleWind_float(in float2 wPos, out float3 vec)
{
    float2 v = SAMPLE_TEXTURE2D_LOD(_WindVectors, sampler_WindVectors, wPos, 0).rg;

    vec = float3(v.x, 0, v.y);
};

void GetLODFactor_float(out float f) {
	f = unity_LODFade.x;
}

void LODDithering_float(in float alpha, in float4 clipPos, out float dither) {

	LODDitheringTransition(clipPos.xyz, unity_LODFade.x);

	dither = alpha;
};

void LODCrossFade_float(in float alpha, float2 clipPos, out float dither) 
{
	dither = alpha;

//#ifdef LOD_FADE_CROSSFADE //Keyword is never set by SG
	float p = GenerateHashedRandomFloat(clipPos.xy * 1);
	float fade = 1 - unity_LODFade.x;
	dither *= (fade );
//#endif
};

void GetSunColor_float(out float3 color) 
{
#ifdef UNIVERSAL_LIGHTING_INCLUDED
	Light mainLight = GetMainLight();
	color = mainLight.color;
#else
	color = 0;
#endif
}
void MainLight_half(float3 WorldPos, out half3 Direction, out half3 Color, out half DistanceAtten, out half ShadowAtten)
{
#ifdef UNIVERSAL_LIGHTING_INCLUDED
	#if SHADERGRAPH_PREVIEW
		Direction = half3(0.5, 0.5, 0);
		Color = 1;
		DistanceAtten = 1;
		ShadowAtten = 1;
	#else
	#if SHADOWS_SCREEN
		half4 clipPos = TransformWorldToHClip(WorldPos);
		half4 shadowCoord = ComputeScreenPos(clipPos);
	#else
		half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
#endif
#endif
	Light mainLight = GetMainLight(shadowCoord);
	Direction = mainLight.direction;
	Color = mainLight.color;
	DistanceAtten = mainLight.distanceAttenuation;
	ShadowAtten = mainLight.shadowAttenuation;
#else
	Direction = half3(0.5, 0.5, 0);
	Color = 1;
	DistanceAtten = 1;
	ShadowAtten = 1;
#endif
}