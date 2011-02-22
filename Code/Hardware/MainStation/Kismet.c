#include "Kismet.h"
#include "RTC.h"
#include "EEPROM.h"
#include "UART.h"
#include "MainStation.h"
#include "USB.h"
#include "EP.h"

#define Reg16(_idx) (*((int16*)&(Reg[_idx])))

#define Reg SharedMemory
extern int8 SharedMemory[EPBUFFERSIZE+(KISMETBUFFERSIZE*2)];

extern int8 EPBufferSize;

extern int8 OperationEnabled;

extern int8 RTCSecond;
extern int8 RTCMinute;
extern int8 RTCHour;
extern int16 RTCDay;

void KismetInit()
{
	
}

void KismetSetRegister(int8 _Register,int16 _Value)
{
	Reg[EPBUFFERSIZE+_Register]=_Value;//TODO
}

int16 KismetGetRegister(int8 _Register)
{
	return Reg[EPBUFFERSIZE+_Register];//TODO
}

int8 KismetExecuteEvent(int16 _DeviceID,int8 _EventID)
{
	int8  a;//variable for the kismet blocks to use
	int8  read8;
	int16 read16;
	int16 eventaddr;
	int16 methodaddr;
	int16 BlockAddr;
	int16 timeout;

	if(!OperationEnabled)return 1;//return if no operating is allowed

	ToSendDataBuffer[2]=((int8*)&_DeviceID)[0];
	ToSendDataBuffer[3]=((int8*)&_DeviceID)[1];
	ToSendDataBuffer[4]=_EventID;

	MemoryBeginRead(0);

	for(timeout=0;timeout<1024;timeout++)
	{
		read16=MemoryReadInt16();
		eventaddr =MemoryReadInt16();
		if(read16==_DeviceID)break;
		if(read16==0xffff) { MemoryEndRead(); return 2; }
	}
	MemoryEndRead(); 
	MemoryBeginRead(eventaddr);
	for(timeout=0;timeout<1024;timeout++)
	{
		read8=MemoryReadInt8();
		methodaddr=MemoryReadInt16();
		
		if(read8==_EventID)break;
		if(read8==0xff) { MemoryEndRead(); return 3; }
	}
	MemoryEndRead();
	ToSendDataBuffer[12]=((int8*)&eventaddr)[0];
	ToSendDataBuffer[13]=((int8*)&eventaddr)[1];
	ToSendDataBuffer[14]=((int8*)&methodaddr)[0];
	ToSendDataBuffer[15]=((int8*)&methodaddr)[1];
	
	BlockAddr=methodaddr;
	MemoryBeginRead(BlockAddr);
	while(1)
	{
		int8 blocktype=MemoryReadInt8();
		//#=EEPROM DATA $=REGISTER DATA
		switch(blocktype)
		{
			case 0x00://end of code
			default://or we dont understand it
			{
				MemoryEndRead();
				return 0;
			}
			case 0x01://load literal int16
			{
				int8 reg	=MemoryReadInt8();
				int16 value	=MemoryReadInt16();
				Reg16(reg)=value;
			}break;
			case 0x02://load literal int8
			{
				int8 reg	=MemoryReadInt8();
				int8 value	=MemoryReadInt8();
				Reg[reg]=value;
			}break;
			case 0x0B://equal
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg16(reg3)=(Reg16(reg1)==Reg16(reg2))?0xffff:0;
			}break;
			case 0x0C://differ
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg16(reg3)=(Reg16(reg1)==Reg16(reg2))?0:0xffff;
			}break;
			case 0x0D://and
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg16(reg3)=Reg16(reg1)&Reg16(reg2);
			}break;
			case 0x0E://or
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg16(reg3)=Reg16(reg1)|Reg16(reg2);
			}break;
			case 0x0F://xor
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg16(reg3)=Reg16(reg1)^Reg16(reg2);
			}break;
			case 0x20://add
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg16(reg3)=Reg16(reg1)+Reg16(reg2);
			}break;
			case 0x21://sub
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg16(reg3)=Reg16(reg1)-Reg16(reg2);
			}break;
			case 0x22://mul
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg16(reg3)=Reg16(reg1)*Reg16(reg2);
			}break;
			case 0x23://div
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg16(reg3)=Reg16(reg1)/Reg16(reg2);
			}break;
			case 0x30://hours
			{
				int8 reg1	=MemoryReadInt8();
				Reg16(reg1) =(int16)RTCHour;
			}break;
			case 0x31://minutes
			{
				int8 reg1	=MemoryReadInt8();
				Reg16(reg1) =(int16)RTCMinute;
			}break;
			case 0x32://seconds
			{
				int8 reg1	=MemoryReadInt8();
				Reg16(reg1) =(int16)RTCSecond;
			}break;
			case 0x33://hours
			{
				int8 reg1	=MemoryReadInt8();
				Reg16(reg1) =(int16)RTCDay;
			}break;
			case 0x0A:
			{
				int8 reg	=MemoryReadInt8();
				SetLED(Reg16(reg));
			}break;
			case 0x80:// $if goto $here?
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				if(Reg16(reg1)==0)
				{
					BlockAddr=methodaddr+(int16)reg2;
					MemoryEndRead();MemoryBeginRead(BlockAddr);
				}
			}break;
			case 0x70://mov
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 amnt	=MemoryReadInt8();
				for(a=0;a<amnt;a++)
					Reg[reg2+a]=Reg[reg1+a];
			}break;
			case 0x71://EPSend
			{
				int16 dev	=MemoryReadInt16();
				EPBufferSize=MemoryReadInt8();
				EPSend(dev);
			}break;
		}
	}
	return 4;
}