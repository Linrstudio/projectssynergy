#include "Default.h"
#include "Memory.h"
#include "UART.h"

void MemoryInit()
{
	MEMORYTXDIR	=0;
	MEMORYRXDIR	=1;
	MEMORYCLKDIR=0;
	MEMORYCSDIR	=0;

	MemoryWriteEnable();
}

int16 MemoryReadInt16(int16 _Addr)
{
	short var;
	int8*dat=&var;
	dat[1]=MemoryReadInt8(_Addr);_Addr++;
	dat[0]=MemoryReadInt8(_Addr);
	return var;
}

void MemoryWriteEnable()
{
	MEMORYCS=0;
	MemorySPIWriteRaw(0b0110);//enable writeing
	MEMORYCS=1;
	MemorySPIDelay();
}

void MemoryWriteDisable()
{
	MEMORYCS=0;
	MemorySPIWriteRaw(0b0100);//disable writeing
	MEMORYCS=1;
	//MemorySPIDelay();
}

void MemoryWriteInt8(int16 _Addr,int8 _Data)
{
	MemoryWriteEnable();
	MEMORYCS=0;
	MemorySPIWriteRaw(0b0010);
	int8*addr=&_Addr;
	MemorySPIWriteRaw(addr[1]);
	MemorySPIWriteRaw(addr[0]);
	MemorySPIWriteRaw(_Data);
	MEMORYCS=1;
	while(MemoryReadStatus()&1);//wait for write to complete
}

int8 MemoryReadStatus()
{
	MEMORYCS=0;
	MemorySPIWriteRaw(0b0101);
	int8 val=MemorySPIReadRaw();
	MEMORYCS=1;
	return val;
}

int8 MemoryReadInt8(int16 _Addr)
{
	MEMORYCLK=0;
	MEMORYCS=0;
	MemorySPIWriteRaw(0b0011);
	int8*addr=&_Addr;
	MemorySPIWriteRaw(addr[1]);
	MemorySPIWriteRaw(addr[0]);
	int8 val=MemorySPIReadRaw();
	MEMORYCS=1;
	MEMORYCLK=1;
	return val;
}

void MemorySPIDelay()
{
#if 1
	//for(int i=0;i<2;i++);
#else
	for(int i=0;i<255;i++)for(int i1=0;i1<10;i1++);
#endif
}

void MemorySPIWriteRaw(int8 _Data)
{
	for(int i=0;i<8;i++)
	{
		MEMORYTX=(_Data>>7)?1:0;
		//MemorySPIDelay();
		MEMORYCLK=1;
		//MemorySPIDelay();
		//MemorySPIDelay();
		MEMORYCLK=0;
		//MemorySPIDelay();
		_Data<<=1;
	}
	MEMORYTX=0;
}

int8 MemorySPIReadRaw()
{
	int8 data=0;
	for(int i=0;i<8;i++)
	{
		//MemorySPIDelay();
		MEMORYCLK=1;
		data<<=1;
		data+=MEMORYRX?1:0;
		//MemorySPIDelay();
		//MemorySPIDelay();
		MEMORYCLK=0;
		//MemorySPIDelay();
	}
	return data;
}