#include "Kismet.h"
#include "RTC.h"
#include "EEPROM.h"
#include "UART.h"
#include "MainStation.h"
#include "USB.h"
#include "EP.h"

//shared memory layout:
#pragma udata memory1
int8 SharedMemoryLow[128];
#pragma udata memory2
int8 SharedMemoryHigh[128];

extern int8 EPBufferSize;

extern int8 OperationEnabled;

extern int8 RTCSecond;
extern int8 RTCMinute;
extern int8 RTCHour;
extern int16 RTCDay;

void KismetInit()
{
	
}

void Set8(int8 _Register,int8 _Value)
{
	if(_Register&128)
		SharedMemoryHigh[_Register&127]=_Value;
	else
		SharedMemoryLow[_Register]=_Value;
}

int8 Get8(int8 _Register)
{
	if(_Register&128)
		return SharedMemoryHigh[_Register&127];
	else
		return SharedMemoryLow[_Register];
}

void Set16(int8 _Register,int16 _Value)
{
	if(_Register&128)
		*(int16*)&(SharedMemoryHigh[_Register&127])=_Value;
	else
		*(int16*)&(SharedMemoryLow[_Register])=_Value;
}

int16 Get16(int8 _Register)
{
	if(_Register&128)
		return *(int16*)&(SharedMemoryHigh[_Register&127]);
	else
		return *(int16*)&(SharedMemoryLow[_Register]);
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

	//ToSendDataBuffer[2]=((int8*)&_DeviceID)[0];
	//ToSendDataBuffer[3]=((int8*)&_DeviceID)[1];
	//ToSendDataBuffer[4]=_EventID;

	MemoryBeginRead(EEPROMHEADERSIZE);

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
	//ToSendDataBuffer[12]=((int8*)&eventaddr)[0];
	//ToSendDataBuffer[13]=((int8*)&eventaddr)[1];
	//ToSendDataBuffer[14]=((int8*)&methodaddr)[0];
	//ToSendDataBuffer[15]=((int8*)&methodaddr)[1];
	
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
				Set16(reg,value);
			}break;
			case 0x02://load literal int8
			{
				int8 reg	=MemoryReadInt8();
				int8 value	=MemoryReadInt8();
				Set8(reg,value);
			}break;
			case 0x0B://equal
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Set16(reg3,(Get16(reg1)==Get16(reg2))?0xffff:0);
			}break;
			case 0x0C://differ
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Set16(reg3,(Get16(reg1)!=Get16(reg2))?0xffff:0);
			}break;
			case 0x0D://and
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Set16(reg3,Get16(reg1)&Get16(reg2));
			}break;
			case 0x0E://or
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Set16(reg3,Get16(reg1)|Get16(reg2));
			}break;
			case 0x0F://xor
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Set16(reg3,Get16(reg1)^Get16(reg2));
			}break;
			case 0x20://add
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Set16(reg3,Get16(reg1)+Get16(reg2));
			}break;
			case 0x21://sub
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Set16(reg3,Get16(reg1)-Get16(reg2));
			}break;
			case 0x22://mul
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Set16(reg3,Get16(reg1)*Get16(reg2));
			}break;
			case 0x23://div
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Set16(reg3,Get16(reg1)/Get16(reg2));
			}break;
			case 0x30://hours
			{
				int8 reg1	=MemoryReadInt8();
				Set16(reg1,(int16)RTCHour);
			}break;
			case 0x31://minutes
			{
				int8 reg1	=MemoryReadInt8();
				Set16(reg1,(int16)RTCMinute);
			}break;
			case 0x32://seconds
			{
				int8 reg1	=MemoryReadInt8();
				Set16(reg1,(int16)RTCSecond);
			}break;
			case 0x33://days
			{
				int8 reg1	=MemoryReadInt8();
				Set16(reg1,(int16)RTCDay);
			}break;
			case 0x0A:
			{
				int8 reg	=MemoryReadInt8();
				SetLED(Get16(reg));
			}break;
			case 0x80:// $if goto $here?
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				if(Get16(reg1)==0)
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
					Set8(reg2+a,Get8(reg1+a));
			}break;
			case 0x71://EPSend
			{
				int16 dev	=MemoryReadInt16();
				EPBufferSize=MemoryReadInt8();
				EPSend(dev);
			}break;
			case 0x90://Set Delay
			{
				int8 timer	=MemoryReadInt8();
				int8 event	=MemoryReadInt8();
				int8 reg	=MemoryReadInt8();
				int16 time=Get16(reg);
				SetTimer(timer,event,time);
			}break;
		}
	}
	return 4;
}
