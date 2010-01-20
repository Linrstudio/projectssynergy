#include <pic18.h>

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

void MemorySeek(unsigned char _Addr)
{
 	MemoryWait();
	EEADR=_Addr;
}

unsigned char MemoryRead()
{
	unsigned char data;
	MemoryWait();
	RD=1;//read data
	data=EEDATA;
	return data;
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
