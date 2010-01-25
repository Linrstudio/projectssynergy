#include <pic18.h>
#include "Default.h"

//for now we use the internal EEPROM even though it is only 256 byte, we use 16bits addresses

#define EEPROMSIZE 256

void MemoryInit()
{
	EEPGD=0;
	CFGS=0;
}

void MemoryWait()
{
	while(RD);
	while(WR);
}

void MemorySeek(unsigned short _Addr)
{
	unsigned char*dat=&_Addr;
 	MemoryWait();
	EEADR=dat[0];
}

unsigned char MemoryRead()
{
	unsigned char data;
	MemoryWait();
	RD=1;//read data
	data=EEDATA;
	return data;
}

int8 MemoryReadInt8(int16 _Addr)
{
	MemorySeek(_Addr);
	unsigned char data;
	MemoryWait();
	RD=1;//read data
	data=EEDATA;
	return data;
}

unsigned int16 MemoryReadInt16(int16 _Addr)
{
	short bob;
	int8*dat=&bob;
	dat[1]=MemoryReadInt8(_Addr);_Addr++;
	dat[0]=MemoryReadInt8(_Addr);
	return bob;
}

void MemoryWrite(unsigned char _Data)
{
	MemoryWait();
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
