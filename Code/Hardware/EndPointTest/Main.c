#include <pic.h>

#include "UART.h"
#include "EP.h"

#define LED RC7
#define DEVICEID 1234

void main()
{
	ANS0 =0;
	ANS1 =0;
	ANS2 =0;
	ANS3 =0;
	ANS4 =0;
	ANS5 =0;
	ANS6 =0;
	ANS7 =0;
	ANS8 =0;
	ANS9 =0;
	ANS10=0;
	ANS11=0;

	ADON=0;

	TRISC0=0;
	TRISC1=0;
	TRISC2=0;

	TRISC7=0;
	TRISB4=0;
	UARTInit();
	UARTRead();
	
	Init();

	//startup glow
	for(int i=0;i<255;i++)
		for(int j=0;j<255;j++)
			RC7=i<j;
	for(int i=0;i<255;i++)
		for(int j=0;j<255;j++)
			RC7=i>j;
	RC7=1;



	int8 lastheader=0;
	while(1)
	{
		Tick();
		//find header
		//if(!UARTAvailable())continue;
		int8 header=UARTReadInt8();
		if(lastheader==0&&header==255)
		{
			int16 DeviceID=UARTReadInt16();
			int8  Length  =UARTReadInt8();
			if(DeviceID==DEVICEID)
			{
				for(int i=0;i<Length&15;i++)UARTBuffer[i]=UARTReadInt8();
				UARTBufferSize=0;
				if(Length!=0)
				{
					InvokeEvent(UARTBuffer[0],*((int16*)&UARTBuffer[1]));
				}else
					Polled();

				//either way we will answer
				UARTWrite();
				UARTWriteInt8(0);	//header
				UARTWriteInt8(255);	//header
				UARTWriteInt16(0);	//address of main station
				UARTWriteInt8(UARTBufferSize);
				for(int i=0;i<UARTBufferSize;i++)UARTWriteInt8(UARTBuffer[i]);
				UARTRead();
			}
			else
			{
				for(int i=0;i<Length;i++)UARTReadInt8();
			}
		}
		lastheader = header;
	}
}

