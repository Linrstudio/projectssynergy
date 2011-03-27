#include "settings.h"

void SettingsInit()
{
	EECON1bits.EEPGD=0;
	EECON1bits.CFGS=0;
}

void SettingsWriteInt8(int8 _Addr,int8 _Data)
{
	unsigned char*dat=&_Addr;
 	while(EECON1bits.RD);
	while(EECON1bits.WR);
	EEADR=_Addr;

	while(EECON1bits.RD);
	while(EECON1bits.WR);
	EEDATA=_Data;
	INTCONbits.GIE=0;//disable interrupts
	EECON1bits.WREN=1;//enable writes
	EECON2=0x55;//required sequence for EEPROM update
	EECON2=0xAA;
	EECON1bits.WR=1;
	while(EECON1bits.WR);
	PIR2bits.EEIF=0;
	EECON1bits.WREN=0;
	INTCONbits.GIE=1;//re-enable interrupts
}

int8 SettingsReadInt8(int8 _Addr)
{
	unsigned char data;
	unsigned char*dat=&_Addr;
	EEADR=_Addr;
	while(EECON1bits.RD);
	EECON1bits.RD=1;//read data
	data=EEDATA;
	return data;
}

int16 SettingsReadInt16(int8 _Addr)
{
	int16 data;
	((int8*)&data)[0]=SettingsReadInt8(_Addr);
	((int8*)&data)[1]=SettingsReadInt8(_Addr+1);
	return data;
}
