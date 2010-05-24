#include"Default.h"
#include"PLC.h"
#include"UART.h"
#include"Memory.h"

void PLCInit()
{
	PLCTXDIR=0;
	PLCRXDIR=1;
	PLCCLKDIR=0;
}

unsigned char PLCReadInt8()
{
	return 0;
}

unsigned char PLCAvailable()
{
	PLCWriteInt8(2);
	return PLCReadInt8();
}

void PLCDelay()
{
#if 1
	for(int i=0;i<8;i++);
#else
	//for(int i=0;i<255;i++)
		//for(int j=0;j<2;j++);
#endif
}

void PLCWriteInt16(int16 _Data)
{
	int8*dat = &_Data;
	PLCWriteInt8(dat[1]);
	PLCWriteInt8(dat[0]);
}

void PLCWriteStart()
{
	PLCDelay();	PLCCLK=1;	PLCDelay();
	PLCTX=1;
	PLCDelay();	PLCCLK=0;	PLCDelay();
	PLCTX=0;
}

void PLCWriteInt8(int8 _Data)
{
	for(int i=0;i<8;i++)
	{
		PLCTX=(_Data>>7)?1:0;
		PLCDelay();	PLCCLK=1;	PLCDelay();
		PLCTX=0;
		PLCDelay();	PLCCLK=0;	PLCDelay();
		_Data<<=1;
	}
	PLCTX=0;
}

void PLCUpdate()
{
	for(int i=0;i<10;i++)
	{
		int16 readid = MemoryReadInt16(deviceaddr);

		int16 zero = 0;
		if(COMPAREINT16(readid,zero)) return;
		
		PLCPoll(readid);
		
		deviceaddr+=4;
	}
}

void PLCPoll(int16 _Recipient)
{
	while(PLCRX);	//wait for the client
	PLCWriteStart();
	PLCWriteInt8(2);//instruction 1
	PLCWriteInt16(_Recipient);
}

void PLCWrite(int16 _Recipient,int16 _Data)
{
	while(PLCRX);	//wait for the client
	PLCWriteStart();
	PLCWriteInt8(1);//instruction 1
	PLCWriteInt16(_Recipient);
	PLCWriteInt16(_Data);
}
