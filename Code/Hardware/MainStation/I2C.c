#include"Default.h"
#include"I2C.h"

#define I2CDelay {Nop();Nop();Nop();Nop();Nop();}

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
	int8 i=128;
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
	int8 i=128;
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
	int8 res;
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
	I2CDelay;SDA=0;I2CDelay;
	I2CDelay;SCL=0;I2CDelay;
}

void I2CStopSlow()
{
	SDA_TRIS=0;
	I2CDelay;SCL=1;I2CDelay;
	I2CDelaySlow();
	I2CDelay;SDA=1;I2CDelay;
}

void I2CWriteSlow(int8 _Byte)
{
	int8 i=128;
	SDA_TRIS=0;
	do
	{
		if((_Byte&i)!=0)SDA=1;else SDA=0;
		I2CDelay;SCL=1;I2CDelay;
		I2CDelay;SCL=0;I2CDelay;
		i>>=1;
  	}while(i!=0);
}

int8 I2CReadSlow()
{
	int8 data;
	int8 i=128;
	data=0;
	SDA_TRIS=1;
	do
	{
		I2CDelay;SCL=1;I2CDelay;
		if(SDA)data|=i;
		I2CDelay;SCL=0;I2CDelay;
		i>>=1;
  	}while(i!=0);
	SDA_TRIS=0;
	return data;
}

int8 I2CAckSlow()
{
	SDA=0;
	I2CDelay;SCL=1;I2CDelay;
	I2CDelay;SCL=0;I2CDelay;
	return 255;
}
