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

void KismetSetReg16(int8 _Register,int16 _Value)
{
	Registers.int16[_Register>>1]=_Value;
}






void KismetDo(int8 _Op,int16 _ParameterAddr)
{
	int8 reg1 = 0;//_Registers&15;
	int8 reg2 = 0;//_Registers>>4;
	
	switch(_Op)
	{
		case 1://BADD int8 add
		{
			Registers.int8[reg1]+=Registers.int8[reg2];
		}break;
		case 2://SADD int16 add
		{
			Registers.int16[reg1>>1]+=Registers.int16[reg2>>1];
		}break;
		case 3://BSUB int8 sub
		{
			Registers.int8[reg1]-=Registers.int8[reg2];
		}break;
		case 4://SSUB int16 sub
		{
			Registers.int16[reg1>>1]-=Registers.int16[reg2>>1];
		}break;
	}
}

void KismetExecuteMethod(int16 _Deviceid,int16 _MethodAddr)
{
	//bla
	//UARTWriteString("about to execute");
	UARTWriteInt16(_Deviceid);
	UARTWriteInt16(_MethodAddr);
	for(int i=0;i<16;i++)UARTWriteInt8(Registers.int8[i]);
}

void KismetExecuteEvent(int16 _DeviceID,int8 _EventID,int16 _EventArgs)
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
	RC0=1;
	//UARTWriteInt16(methodaddr);

	//i already push our arguments into register one and two
	Registers.int16[0]=_EventArgs;
	KismetExecuteMethod(_DeviceID,methodaddr);
}
