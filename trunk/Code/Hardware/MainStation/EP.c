#include "Default.h"
#include "UART.h"
#include "EP.h"
#include "Kismet.h"
#include "Compiler.h"
#include "EEPROM.h"
#include "HardwareProfile.h"

#include "MainStation.h"

int8 DevicesNotFound=0;
int8  PacketID=0;
int16 DeviceAddress=EEPROMHEADERSIZE;//EEPROM Address

int8 EPBufferSize;

extern int8 OperationEnabled;

extern int8 RTCHour;

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
			
		if(dev==0xffff)//if last device read from EEPROM 
		{
			DeviceAddress=EEPROMHEADERSIZE;//poll the first one

#if 0
			if(DevicesNotFound!=0)
				SetLED(1);
			else
				SetLED(0);
#endif

			DevicesNotFound=0;
		}
		else
		{
			if(dev!=0)//polling the mainstation would be stupid
			{
				if(EPPoll(dev))
				{
					//yay
				}
				else
				{
					DevicesNotFound++;
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
	for(i=0;i<EPBufferSize;i++)UARTWriteInt8(Get8(i));
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
			Set8(i,UARTReadInt8());
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
	event=Get8(0);//wait, there is more!
	if(event!=0)
	{
		for(a=0;a<EPBUFFERSIZE;a++)
			Set8(a,Get8(a+1));//move the parameters to the right place
		KismetExecuteEvent(_DeviceID,event);
	}
	return result;
}
