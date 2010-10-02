#include 	"I2C.h"
#include	"EEPROM.h"
#include	"Default.h"

void MemoryBeginWrite(int16 _Addr)
{
	I2CStartSlow();
	I2CWriteSlow(0b10100000);
	I2CAckSlow();
	I2CWriteSlow(((int8*)&_Addr)[1]);	//MSB is stored last on PIC's
	I2CAckSlow();
	I2CWriteSlow(((int8*)&_Addr)[0]);	//LSB is stored first on PIC's
}

void MemoryWrite(int8 _Data)
{
	I2CAckSlow();
	I2CWriteSlow(_Data);
}

void MemoryEndWrite()
{
	int i,i2;
	I2CAckSlow();
	I2CStopSlow();
//	for(i=0;i<255;i++)for(i2=0;i2<2;i2++);//long delay to give the device time to write the byte
}

void MemoryBeginRead(int16 _Addr)
{
	I2CStartSlow();
	I2CWriteSlow(0b10100000);
	I2CAckSlow();
	I2CWriteSlow(((int8*)&_Addr)[1]);
	I2CAckSlow();
	I2CWriteSlow(((int8*)&_Addr)[0]);
	I2CAckSlow();
	I2CStopSlow();
	I2CStartSlow();
	I2CWriteSlow(0b10100001);
	//this ACK will be at the beginning of ReadByte()
}

int8 MemoryReadInt8()
{
	int8 data;
	I2CAckSlow();
	data=I2CReadSlow();
	return data;
}

int16 MemoryReadInt16()
{
	int8 dat[2];
	dat[0]=MemoryReadInt8();
	dat[1]=MemoryReadInt8();
	return *(int16*)&dat[0];
}

void MemoryEndRead()
{
	I2CStopSlow();
}
