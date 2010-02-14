#include"Default.h"
#include"PLC.h"
#include"UART.h"

void PLCDelay()
{
	//for(int i1=0;i1<255;i1++)for(int i2=0;i2<128;i2++)for(int i3=0;i3<1;i3++);
}

void PLCInit()
{
	PLCTXDIR=0;
	PLCRXDIR=1;
	PLCCLKDIR=0;
}

unsigned char PLCReadInt8(void)
{
	
}

unsigned char PLCAvailable(void)
{
	return 0;
}

void PLCWriteInt8(int8 c)
{
	PLCWriteBuffer=c;
	PLCWriteIndex=0;
}

void PLCUpdate()
{
	PLCTX=PLCWriteBuffer>>(PLCWriteIndex-1);//write bit ( even it it makes no sense )
	PLCDelay();
	PLCCLK=1;
	PLCDelay();
	if(PLCWriteIndex==0)PLCTX=1;else PLCTX=0;//if its the first character, send HI when CLK==LO
	PLCDelay();
	PLCCLK=0;
	PLCDelay();
	if(PLCRX==8)PLCReadIndex=1;

	if(PLCReadIndex>0 && PLCReadIndex<=8)
	{
		PLCReadIndex++;
	}
	if(PLCWriteIndex<=8)
	{
		UARTWriteInt8(PLCWriteIndex);
		PLCWriteIndex++;
	}
}