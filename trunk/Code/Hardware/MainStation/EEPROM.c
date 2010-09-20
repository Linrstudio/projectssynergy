#include 	"I2C.h"
#include	"EEPROM.h"
#include	"Default.h"

void EEPROMWrite(int8 _AddrHi,int8 _AddrLo,int8 _Data)
{
	int i,i2;
	I2CStartSlow();
	I2CWriteSlow(0b10100000);
	I2CAckSlow();
	I2CWriteSlow(_AddrHi);
	I2CAckSlow();
	I2CWriteSlow(_AddrLo);
	I2CAckSlow();
	I2CWriteSlow(_Data);
	I2CAckSlow();
	I2CStopSlow();
	//for(i=0;i<255;i++)for(i2=0;i2<2;i2++);//long delay to give the device time to write the byte
}

void EEPROMBeginRead(int8 _AddrHi,int8 _AddrLo)
{
	I2CStartSlow();
	I2CWriteSlow(0b10100000);
	I2CAckSlow();
	I2CWriteSlow(_AddrHi);
	I2CAckSlow();
	I2CWriteSlow(_AddrLo);
	I2CAckSlow();
	I2CStopSlow();
	I2CStartSlow();
	I2CWriteSlow(0b10100001);
	//this ACK will be at the beginning of ReadByte()
}

int8 EEPROMRead()
{
	int8 data;
	I2CAckSlow();
	data=I2CReadSlow();
	return data;
}

void EEPROMEndRead()
{
	I2CStopSlow();
}
