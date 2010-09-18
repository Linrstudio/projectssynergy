#include"Default.h"
#include"UART.h"
#include"EP.h"

#include "Compiler.h"

#include "HardwareProfile.h"

int8 PacketID=0;
int16 NextDevice=1234;
int8 Buffer[16];

void EPSendHeader()
{
	int i;
	//for(i=0;i<5;i++)UARTWriteInt8(0);//send some spare bytes
	UARTWriteInt8(0);
	UARTWriteInt8(255);
}

void EPInit()//initialize Endpoints
{
	
}

void EPUpdate()
{
	//EPPoll(123);
	if(EPPoll(NextDevice))
	{
		TRISBbits.TRISB4=0;
		PORTBbits.RB4=0;
		TRISCbits.TRISC2=0;
		PORTCbits.RC2=1;
	}else{
		TRISBbits.TRISB4=0;
		PORTBbits.RB4=1;
		TRISCbits.TRISC2=0;
		PORTCbits.RC2=0;
	}
	//NextDevice++;
}

int8 EPInvokeEvent(int16 _DeviceID,int8 _Event,int16 _Args)
{
	int8 timeout;
	int8 i;
	int8 j;
	for(timeout=0;timeout<3;timeout++)//try three times
	{
		UARTWrite();
		UARTWriteInt8(228);
		UARTWriteInt8(42);
		UARTWriteInt16(_DeviceID);
		UARTWriteInt8(_Event);
		UARTWriteInt16(_Args);
		UARTRead();
		for(i=0;i<255;i++)for(j=0;j<128;j++)//for(k=0;k<2;k++)
		{
			if(UARTAvailable()!=0)
			{
				if(UARTReadInt8()==255)
					return 255;
			}
		}
	}
	return 0;
}

int8 EPAnswer()
{
	int idx=0;
	int i,j,k;
	for(i=0;i<255;i++)for(j=0;j<128;j++)//for(k=0;k<2;k++)
	{
		if(UARTAvailable())
		{
			int8 read = UARTReadInt8();
			switch(idx)
			{
				case 0:
					if(read==0)idx++;else idx=0;
					break;
				case 1:
					if(read==255)idx++;else idx=0;
					break;
				case 2:
					if(read==PacketID)return 255;else idx=0;
					break;
			}
		}
	}
	return 0;
}

int8 EPWrite(int16 _DeviceID,int8 _Length,int8*_Data)
{
	int i;
	UARTWrite();
	EPSendHeader();
	UARTWriteInt16(_DeviceID);
	UARTWriteInt8(_Length);
	for(i=0;i<_Length;i++)UARTWriteInt8(_Data[i]);
	UARTRead();
}

//returns 0 if device not reachable
int8 EPPoll(int16 _DeviceID)
{
	int8 timeout;
	int8 i;
	int8 j;
	int8 k;
	int8 l;

	int16 deviceID;
	int8 Length;
	
	int8 header=0;
	int8 lastheader=1;

	Buffer[0]=0;
	Buffer[1]=0;
	Buffer[2]=0;
	
	EPWrite(_DeviceID,3,&Buffer[0]);
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
		int8 length		=UARTReadInt8();
		for(l=0;l<length;l++)
		{
			Buffer[l&15]=UARTReadInt8();
		}
		
		if(DeviceID==0)//if this is for me ( should always be true )
		{
			//if(length==0)
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
