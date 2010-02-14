#include "Default.h"
#include "Memory.h"

void MemoryInit()
{
	EEPGD=0;
	CFGS=0;

	MEMORYTXDIR	=0;
	MEMORYRXDIR	=1;
	MEMORYCLKDIR=0;
	MEMORYCSDIR	=0;
}

void MemoryWait()
{
#ifdef MEMORTEXTERNAL
#else
	while(RD);
	while(WR);
#endif
}

int8 MemoryReadInt8(int16 _Addr)
{
#ifdef MEMORYEXTERNAL
	return MemorySPIRead(_Addr);
#else
	//seek
	unsigned char*dat=&_Addr;
 	MemoryWait();
	EEADR=dat[0];
	//read stuff
	unsigned char data;
	MemoryWait();
	RD=1;//read data
	data=EEDATA;
	return data;
#endif
}

int16 MemoryReadInt16(int16 _Addr)
{
	short bob;
	int8*dat=&bob;
	dat[1]=MemoryReadInt8(_Addr);_Addr++;
	dat[0]=MemoryReadInt8(_Addr);
	return bob;
}

void MemoryWriteInt8(int16 _Addr,int8 _Data)
{
#ifdef MEMORYEXTERNAL
	MemorySPIWrite(_Addr,_Data);
#else
	//seek
	unsigned char*dat=&_Addr;
 	MemoryWait();
	EEADR=dat[0];
	//write stuff
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
#endif
}

void MemorySPIWriteEnable()
{
	MEMORYCS=0;
	MemorySPIWriteRaw(0b0110);//enable writeing
	MEMORYCS=1;
	MemorySPIDelay();
}

void MemorySPIWriteDisable()
{
	MEMORYCS=0;
	MemorySPIWriteRaw(0b0100);//disable writeing
	MEMORYCS=1;
	MemorySPIDelay();
}

void MemorySPIWrite(int16 _Addr,int8 _Data)
{
	MemorySPIWriteEnable();
	MEMORYCS=0;
	MemorySPIWriteRaw(0b0010);
	int8*addr=&_Addr;
	MemorySPIWriteRaw(addr[1]);
	MemorySPIWriteRaw(addr[0]);
	MemorySPIWriteRaw(_Data);
	MEMORYCS=1;
	MemorySPIDelay();
}

int8 MemorySPIReadStatus()
{
	MEMORYCS=0;
	MemorySPIWriteRaw(0b0101);
	int8 val=MemorySPIReadRaw();
	MEMORYCS=1;
	MemorySPIDelay();
	return val;
}

int8 MemorySPIRead(int16 _Addr)
{
	MEMORYCS=0;
	MemorySPIWriteRaw(0b0011);
	int8*addr=&_Addr;
	MemorySPIWriteRaw(addr[1]);
	MemorySPIWriteRaw(addr[0]);
	int8 val=MemorySPIReadRaw();
	MEMORYCS=1;
	MemorySPIDelay();

	return val;
}

void MemorySPIDelay()
{
#if 1
	//for(int i=0;i<16;i++);
#else
	for(int i=0;i<255;i++)for(int i1=0;i1<10;i1++);
#endif
}

void MemorySPIWriteRaw(int8 _Data)
{
	for(int i=0;i<8;i++)
	{
		MEMORYTX=(_Data>>7)?1:0;
		MemorySPIDelay();
		MEMORYCLK=1;
		MemorySPIDelay();
		MemorySPIDelay();
		MEMORYCLK=0;
		MemorySPIDelay();
		_Data<<=1;
	}
}

int8 MemorySPIReadRaw()
{
	int8 data=0;
	for(int i=0;i<8;i++)
	{
		MemorySPIDelay();
		MEMORYCLK=1;
		data<<=1;
		data+=MEMORYRX?1:0;
		MemorySPIDelay();
		MemorySPIDelay();
		MEMORYCLK=0;
		MemorySPIDelay();
	}
	return data;
}