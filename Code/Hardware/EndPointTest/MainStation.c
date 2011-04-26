#include "MainStation.h"
#include "EP.h"
#include "UART.h"
#include "Default.h"

int8 EPBuffer[16];
int8 EPBufferSize;
int8 OperationEnabled=255;

#define LED			PORTBbits.RB4
#define LED_TRIS 	TRISBbits.TRISB4

int16 DeviceID=0xffff;
int8 lastheader=0;

int8 connected;

void MSInit()
{
	int8 dim1;
	int8 dim2;

	//turn off A lot of crap
	CM1CON0bits.C1ON=0;
	CM1CON0=0;
	CM2CON0=0;
	//SRCON0bits.SRLEN=0;
	SRCON0=0;
	//disable analog
	ANSEL=0;
	ANSELH=0;
	ADCON0=0;

	LED_TRIS=0;

	SSPSTAT=0x00;
	//SSPCON=0xff;

	USBInit();
	UARTInit();
	UARTRead();
	SettingsInit();
	DeviceID=SettingsReadInt16(0);
	connected=0;
	EPInit();

	LED=0;
}

void MSUpdate()
{
	int8  i;
	int8  header;
	int16 deviceid;
	int8  Length;
	USBUpdate();
	EPUpdate();
	//find header
	//if(!UARTAvailable())continue;
	header=UARTReadInt8();
	if(lastheader==0&&header==0xff)
	{
		deviceid=UARTReadInt16();
		Length  =UARTReadInt8();
		if(deviceid==DeviceID)
		{
			for(i=0;i<Length;i++)EPBuffer[i]=UARTReadInt8();
			EPBufferSize=0;
			if(Length!=0)
			{
				EPInvokeEvent(EPBuffer[0],&(EPBuffer[1]));
			}else{
				if(connected==0)
				{
					connected=255;
					EPBuffer[0]=1;
					EPBufferSize=1;
					LED=1;
				}else EPPolled();
			}
			
			//either way we will answer
			UARTWrite();
			UARTWriteInt8(0);	//header
			UARTWriteInt8(255);	//header
			UARTWriteInt16(0);	//address of main station
			UARTWriteInt8(EPBufferSize);
			for(i=0;i<EPBufferSize;i++)UARTWriteInt8(EPBuffer[i]);
			UARTRead();
		}
		else
		{
			for(i=0;i<Length;i++)UARTReadInt8();
		}
	}
	lastheader = header;
}
