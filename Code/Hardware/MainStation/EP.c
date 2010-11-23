#include"Default.h"
#include"UART.h"
#include"EP.h"
#include "Kismet.h"
#include "Compiler.h"
#include "EEPROM.h"
#include "HardwareProfile.h"

#include "MainStation.h"

int8  PacketID=0;
int16 DeviceAddress=0;//EEPROM Address

extern int8 SharedMemory[EPBUFFERSIZE+(KISMETBUFFERSIZE*2)];
int8 EPBufferSize;

extern int8 OperationEnabled;

void EPInit()//initialize Endpoints
{
	
}

void EPUpdate()
{
	int16 dev;
	if(OperationEnabled!=0)
	{
		MemoryBeginRead(DeviceAddress);
		DeviceAddress+=4;
		dev=MemoryReadInt16();
		MemoryEndRead();
			
		if(dev==0xffff)
		{
			DeviceAddress=0;
		}
		else
		{
			if(dev!=0)//polling the mainstation whould be stupid
			{
				if(EPPoll(dev))
				{
					//SetLED1(0);
				}
				else
				{
					SetLED1(1);
				}
			}
		}
	}
}

int8 EPSend(int16 _DeviceID)
{
	int i;
	UARTWrite();
	UARTWriteInt8(0);
	UARTWriteInt8(255);
	UARTWriteInt16(_DeviceID);
	UARTWriteInt8(EPBufferSize);
	for(i=0;i<EPBufferSize;i++)UARTWriteInt8(SharedMemory[i]);
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
		EPBufferSize=UARTReadInt8();
		for(i=0;i<EPBufferSize&15;i++)
		{
			SharedMemory[i]=UARTReadInt8();
		}
		
		if(DeviceID==0)//if this is for me ( should always be true )
		{
			return 255;
		}
	}
	
	if(UARTError)
	{
		UARTError=0;
	}
	return 0;
}

//returns 0 if device not reachable
int8 EPPoll(int16 _DeviceID)
{
	int8 result;
	int8 event;
	int8 a;
	EPBufferSize=0;
	result = EPSend(_DeviceID);
	if(EPBufferSize==0)return result;
	event=SharedMemory[0];//wait, there is more!
	if(event!=0)
	{
		for(a=0;a<EPBUFFERSIZE;a++)
			SharedMemory[a+1]=SharedMemory[EPBUFFERSIZE];//move the parameters to the right place
		KismetExecuteEvent(_DeviceID,event);
	}
	return result;
}
