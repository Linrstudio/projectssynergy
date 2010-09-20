#include"Default.h"
#include"I2C.h"

void I2CInit()
{
	SCL_TRIS=0;
	SDA_TRIS=0;
	SCL=1;
	SDA=1;
}

void I2CStart()
{
	SDA_TRIS=0;
	SDA=0;
	//Delay();
	SCL=0;
	//Delay();
}

void I2CStop()
{
	SDA_TRIS=0;
	SCL=1;
	//Delay();
	SDA=1;
}

void I2CWrite(int8 _Byte)
{
	int i=128;
	SDA_TRIS=0;
	do
	{
		SCL=0;
		//Delay();
		if((_Byte&i)!=0)SDA=1;else SDA=0;
		SCL=1;
		//Delay();
		SCL=0;
		i>>=1;
  	}while(i!=0);
}

int8 I2CRead()
{
	int8 data;
	int i=128;
	SDA_TRIS=1;
	//SDA_PULLUP=1;
	data=0;
	do
	{
		SCL=0;
		//Delay();
		SCL=1;
		if(SDA)data|=i;
		//Delay();
		SCL=0;
		i>>=1;
  	}while(i!=0);
	SDA_TRIS=0;
	return data;
}

int8 I2CAck()
{
#if 0
	int res;
	SDA_TRIS=1;
	//SDA_PULLUP=1;
	//Delay();
	SCL=1;
	res=SDA;
	//Delay();
	SCL=0;
	SDA_TRIS=0;	
	return res?0:1;
#else
	SDA=0;
	SCL=1;
	SCL=0;
	return 1;
#endif
}


//SLOW FUNCTIONS for calendar IC etc


void I2CDelaySlow()
{
	int i,j;
	//for(i=0;i<25;i++);
}

void I2CStartSlow()
{
	SDA_TRIS=0;
	SDA=0;
	I2CDelaySlow();
	SCL=0;
	I2CDelaySlow();
}

void I2CStopSlow()
{
	SDA_TRIS=0;
	SCL=1;
	I2CDelaySlow();
	SDA=1;
}

void I2CWriteSlow(int8 _Byte)
{
	int i=128;
	SDA_TRIS=0;
	do
	{
		SCL=0;
		I2CDelaySlow();
//ok,, here magic happens these two lines should to the same, strangely enough it only works when I put both..., I just died a little
		SDA=((_Byte&i)!=0)?1:0;
		if((_Byte&i)!=0)SDA=1;else SDA=0;

		SCL=1;
		I2CDelaySlow();
		SCL=0;
		i>>=1;
  	}while(i!=0);
}

int8 I2CReadSlow()
{
	int8 data;
	int i=128;
	data=0;
	SDA_TRIS=1;
	do
	{
		SCL=0;
		I2CDelaySlow();
		SCL=1;
		if(SDA)data|=i;
		I2CDelaySlow();
		SCL=0;
		i>>=1;
  	}while(i!=0);
	SDA_TRIS=0;
	return data;
}

int8 I2CAckSlow()
{
#if 0
	int res;
	SDA_TRIS=1;
	//SDA_PULLUP=1;
	I2CDelaySlow();
	SCL=1;
	res=SDA;
	I2CDelaySlow();
	SCL=0;
	SDA_TRIS=0;	
	return res?0:255;
#else
	SDA=0;
	I2CDelaySlow();
	SCL=1;
	I2CDelaySlow();
	SCL=0;
	return 255;
#endif
}
