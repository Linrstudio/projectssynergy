#include "MainStation.h"
#include "EP.h"
#include "UART.h"
#include "Default.h"

int8 EPBuffer[16];
int8 EPBufferSize;
int8 OperationEnabled=255;

#define LED			PORTBbits.RB4
#define LED_TRIS 	TRISBbits.TRISB4

#define DEVICEID 6

int8 lastheader=0;

void MSInit()
{
	int8 dim1;
	int8 dim2;

	//turn off A lot of crap
	CM1CON0bits.C1ON=0;
	CM1CON0=0;
	CM2CON0=0;
	SRCON0bits.SRLEN=0;
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
	EPInit();


	TRISC=0;
	PORTC=0x00;
	
	//startup glow
	for(dim1=0;dim1<255;dim1++)
		for(dim2=0;dim2<255;dim2++)
			LED=dim1<dim2;
	for(dim1=0;dim1<255;dim1++)
		for(dim2=0;dim2<255;dim2++)
			LED=dim1>dim2;
	LED=1;
}

void MSUpdate()
{
	int8 i;
	int8 header;
	int16  DeviceID;
	int8  Length;
	
	USBUpdate();
	EPUpdate();
	//find header
	//if(!UARTAvailable())continue;
	header=UARTReadInt8();
	if(lastheader==0&&header==255)
	{
		DeviceID=UARTReadInt16();
		Length  =UARTReadInt8();
		if(DeviceID==DEVICEID)
		{
			for(i=0;i<Length&15;i++)EPBuffer[i]=UARTReadInt8();
			EPBufferSize=0;
			if(Length!=0)
			{
				EPInvokeEvent(EPBuffer[0],&(EPBuffer[1]));
			}else
				EPPolled();
			
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
