#include "Default.h"
#include "Settings.h"
#include "UART.h"

void SettingsInit()
{

	EEPGD=0;
	CFGS=0;
}

void SettingsWriteInt8(int8 _Addr,int8 _Data)
{
	unsigned char*dat=&_Addr;
 	while(RD);
	while(WR);
	EEADR=_Addr;

	while(RD);
	while(WR);
	EEDATA=_Data;
	GIE=0;//disable interrupts
	WREN=1;//enable writes
	EECON2=0x55;//required sequence for EEPROM update
	EECON2=0xAA;
	WR=1;
	while(WR);
	EEIF=0;
	WREN=0;
	GIE=1;//re-enable interrupts
}

void SettingsWriteInt16(int8 _Addr,int16 _Data)
{
	int8*dat=&_Data;
	SettingsWriteInt8(_Addr,dat[1]);_Addr++;
	SettingsWriteInt8(_Addr,dat[0]);
}

int8 SettingsReadInt8(int8 _Addr)
{
	unsigned char*dat=&_Addr;
	EEADR=_Addr;

	unsigned char data;
	while(RD);
	RD=1;//read data
	data=EEDATA;
	return data;
}

int16 SettingsReadInt16(int8 _Addr)
{
	int16 var;
	int8*dat=&var;
	dat[1]=SettingsReadInt8(_Addr);_Addr++;
	dat[0]=SettingsReadInt8(_Addr);
	return var;
}
