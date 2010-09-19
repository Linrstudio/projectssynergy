#include"Default.h"
#include"UART.h"
#include"EP.h"

#include "Compiler.h"

#include "HardwareProfile.h"

#include "MainStation.h"

int8 PacketID=0;
int16 NextDevice=1234;


int8 EPBuffer[16];
int16 EPBufferSize;


void EPInit()//initialize Endpoints
{
	
}

void EPUpdate()
{
	return;
	//EPPoll(123);
	if(EPPoll(NextDevice))
	{
		SetLED2(1);
	}else{
		SetLED2(2);
	}
	//NextDevice++;
}

int8 EPSend(int16 _DeviceID)
{
	int i;
	UARTWrite();
	UARTWriteInt8(0);
	UARTWriteInt8(255);
	UARTWriteInt16(_DeviceID);
	UARTWriteInt8(EPBufferSize);
	for(i=0;i<EPBufferSize;i++)UARTWriteInt8(EPBuffer[i]);
	UARTRead();
	
	for(i=0;i<2;i++)
	{
		TMR0L=0;
		INTCONbits.TMR0IF=0;
		while(!INTCONbits.TMR0IF)
		{
			if(UARTAvailable())break;
		}
	}
	
	UARTError=0;
	if(UARTReadInt8()==0&&UARTReadInt8()==255)
	{
		int16 DeviceID	=UARTReadInt16();
		int8 EPBufferSize=UARTReadInt8();
		for(i=0;i<EPBufferSize;i++)
		{
			EPBuffer[i&15]=UARTReadInt8();
		}
		
		if(DeviceID==0)//if this is for me ( should always be true )
		{
			return 255;
		}
	}
	if(UARTError)
	{
		UARTError=0;
		return 0;
	}

	return 0;
}

//returns 0 if device not reachable
int8 EPPoll(int16 _DeviceID)
{
	EPBufferSize=0;
	return EPSend(_DeviceID);
}
