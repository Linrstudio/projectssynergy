#include <pic18.h>
#include "Default.h"

static union//4bit addressble registers for kismet execution
{
	int8  int8 [16];
	int16 int16[ 8];
}Registers;

void KismetSetReg8(int8 _Register,int8 _Value)
{
	Registers.int8[_Register]=_Value;
}

int8 KismetGetReg8(int8 _Register)
{
	return Registers.int8[_Register];
}

void KismetSetReg16(int8 _Register,int16 _Value)
{
	Registers.int16[_Register>>1]=_Value;
}

void KismetExecuteMethod(int16 _Deviceid,int16 _MethodAddr)
{
	//UARTWriteInt16(_Deviceid);
	//UARTWriteInt16(_MethodAddr);

	//for(int i=0;i<16;i++)UARTWriteInt8(Registers.int8[i]);

	int16 BlockAddr=_MethodAddr;

	while(1)
	{
		int8 blocktype=MemoryReadInt8(BlockAddr);
		//UARTWriteInt8(BlockAddr);//debug
		//#=EEPROM DATA $=REGISTER DATA
		switch(blocktype)
		{
			case 0://end of code
			{
				return;
			}break;
			case 1://assign int8data #reg #value
			{
				int8 reg  =MemoryReadInt8(BlockAddr+1);
				int8 value=MemoryReadInt8(BlockAddr+2);
				KismetSetReg8(reg,value);
				BlockAddr+=3;
			}break;
			case 2://set debug led $on ?
			{
				int8 reg=MemoryReadInt8(BlockAddr+1);
				int8 on =KismetGetReg8(reg);
				RC0=on;
				BlockAddr+=2;
			}break;
			case 3://dump registers
			{
				for(int i=0;i<16;i++)UARTWriteInt8(Registers.int8[i]);
				BlockAddr+=1;
			}break;
			case 4:// JUMP #target addr
			{
				int8 addr=MemoryReadInt8(BlockAddr+1);
				BlockAddr = _MethodAddr+addr;
			}break;
			case 5:// $a $b $equal?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				int8 addr3=MemoryReadInt8(BlockAddr+3);
				KismetSetReg8(addr3,KismetGetReg8(addr1)==KismetGetReg8(addr2));
				BlockAddr+=4;
			}break;
			case 6:// GetHour $targetreg
			{
				int8 reg=MemoryReadInt8(BlockAddr+1);
				KismetSetReg8(reg,RTCHour);
				BlockAddr+=2;
			}break;
			case 7:// GetMinute $targetreg
			{
				int8 reg=MemoryReadInt8(BlockAddr+1);
				KismetSetReg8(reg,RTCMinute);
				BlockAddr+=2;
			}break;
			case 8:// GetSecond $targetreg
			{
				int8 reg=MemoryReadInt8(BlockAddr+1);
				KismetSetReg8(reg,RTCSecond);
				BlockAddr+=2;
			}break;
			case 9:// GetWeekDay $targetreg
			{
				int8 reg=MemoryReadInt8(BlockAddr+1);
				KismetSetReg8(reg,RTCDay);
				BlockAddr+=2;
			}break;
			default:
			{
				return;
			}
			break;
		}
	}
}

void KismetExecuteEvent(int16 _DeviceID,int8 _EventID,int8 _EventArgs)
{
/*
	UARTWriteInt16(_DeviceID);
	UARTWriteInt8(_EventID);
	UARTWriteInt16(_EventArgs);
*/
	int16 deviceaddr = 0;
	int16 eventaddr = 0;
	int16 methodaddr = 0;
	while(1)
	{
		int16 readid = MemoryReadInt16(deviceaddr);

		if(COMPAREINT16(readid,_DeviceID))
		{
			eventaddr = MemoryReadInt16(deviceaddr+2);
			break;
		}
		int16 zero = 0;
		if(COMPAREINT16(readid,zero)) { UARTWriteString("failed to find device"); return; }
		deviceaddr+=4;
	}

	//UARTWriteInt16(eventaddr);

	while(1)
	{
		int8 readid = MemoryReadInt8(eventaddr);
		if(readid==_EventID)
		{
			methodaddr = MemoryReadInt16(eventaddr+1);
			break;
		}
		int8 zero = 0;
		if(readid==zero) { UARTWriteString("failed to find event"); return; }
		eventaddr+=3;
	}
	//UARTWriteInt16(methodaddr);

	//i already push our arguments into register one and two
	for(int i=0;i<16;i++)Registers.int8[i]=0;//clear all registers ( for EEPROM optimalisation reasons )
	Registers.int8[0]=_EventArgs;
	KismetExecuteMethod(_DeviceID,methodaddr);
}
