#include "Kismet.h"
#include "RTC.h"
#include "Memory.h"
#include "UART.h"

void KismetInit()
{
	KismetEnabled=1;
}

void KismetSetRegister(int8 _Register,int16 _Value)
{
	KismetRegisters[_Register]=_Value;
}

int16 KismetGetRegister(int8 _Register)
{
	return KismetRegisters[_Register];
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
		//UARTWriteInt8(blocktype);//debug

		//#=EEPROM DATA $=REGISTER DATA
		switch(blocktype)
		{
			case 0://end of code
			default://or we dont understand it
			{
				return;
			}
			case 1://assign int8data $reg #value
			{
				int8 reg  =MemoryReadInt8(BlockAddr+1);
				int16 value=MemoryReadInt16(BlockAddr+2);
				KismetSetRegister(reg,value);
				BlockAddr+=4;
			}break;
			case 2://set debug led1 $on
			{
				int8 reg=MemoryReadInt8(BlockAddr+1);
				int8 on =KismetGetRegister(reg);
				RA4=on!=0?0:1;
				BlockAddr+=2;
			}break;
			case 3://set debug led2 $on
			{
				int8 reg=MemoryReadInt8(BlockAddr+1);
				int8 on =KismetGetRegister(reg);
				RA5=on!=0?0:1;
				BlockAddr+=2;
			}break;
			case 4://not implemented
			{
				return;
			}break;
			case 5:// $a $b $equal?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				int8 addr3=MemoryReadInt8(BlockAddr+3);
				KismetSetRegister(addr3,KismetGetRegister(addr1)==KismetGetRegister(addr2)?1:0);
				BlockAddr+=4;
			}break;
			case 6:// GetHour $targetreg
			{
				int8 reg=MemoryReadInt8(BlockAddr+1);
				KismetSetRegister(reg,RTCHour);
				BlockAddr+=2;
			}break;
			case 7:// GetMinute $targetreg
			{
				int8 reg=MemoryReadInt8(BlockAddr+1);
				KismetSetRegister(reg,RTCMinute);
				BlockAddr+=2;
			}break;
			case 8:// GetSecond $targetreg
			{
				int8 reg=MemoryReadInt8(BlockAddr+1);
				KismetSetRegister(reg,RTCSecond);
				BlockAddr+=2;
			}break;
			case 9:// GetWeekDay $targetreg
			{
				int8 reg=MemoryReadInt8(BlockAddr+1);
				KismetSetRegister(reg,RTCDay);
				BlockAddr+=2;
			}break;
			case 10://Add $a $b $a*b?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				int8 addr3=MemoryReadInt8(BlockAddr+3);
				KismetSetRegister(addr3,KismetGetRegister(addr1)+KismetGetRegister(addr2));
				BlockAddr+=4;
			}break;
			case 11://Sub $a $b $a*b?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				int8 addr3=MemoryReadInt8(BlockAddr+3);
				KismetSetRegister(addr3,KismetGetRegister(addr1)-KismetGetRegister(addr2));
				BlockAddr+=4;
			}break;
			case 12://Mul $a $b $a*b?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				int8 addr3=MemoryReadInt8(BlockAddr+3);
				KismetSetRegister(addr3,KismetGetRegister(addr1)*KismetGetRegister(addr2));
				BlockAddr+=4;
			}break;
			case 13://Div $a $b $a/b?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				int8 addr3=MemoryReadInt8(BlockAddr+3);
				KismetSetRegister(addr3,KismetGetRegister(addr1)/KismetGetRegister(addr2));
				BlockAddr+=4;
			}break;
			case 14://not implemented
			{
				return;
			}break;
			case 15://Bitmask $a $b $a&b?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				int8 addr3=MemoryReadInt8(BlockAddr+3);
				KismetSetRegister(addr3,KismetGetRegister(addr1)&KismetGetRegister(addr2));
				BlockAddr+=4;
			}break;
			case 16://SetVariable #varaddr $reg
			{
				int8 varaddr=MemoryReadInt8(BlockAddr+1);
				int8 valreg =MemoryReadInt8(BlockAddr+2);
				KismetVariables[varaddr]=KismetGetRegister(valreg);
				BlockAddr+=3;
			}break;
			case 17://GetVariable #varaddr $reg
			{
				int8 varaddr=MemoryReadInt8(BlockAddr+1);
				int8 valreg =MemoryReadInt8(BlockAddr+2);
				KismetSetRegister(valreg,KismetVariables[varaddr]);
				BlockAddr+=3;
			}break;
			case 18:// $a $b $differs?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				int8 addr3=MemoryReadInt8(BlockAddr+3);
				KismetSetRegister(addr3,KismetGetRegister(addr1)!=KismetGetRegister(addr2)?1:0);
				BlockAddr+=4;
			}break;
			case 19:// $if not goto $here?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				BlockAddr+=3;
				if(KismetGetRegister(addr1)==0)
					BlockAddr=_MethodAddr+addr2;					
			}break;
			case 20:// $if goto $here?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				BlockAddr+=3;
				if(KismetGetRegister(addr1)==0)
					BlockAddr=_MethodAddr+addr2;					
			}break;
			case 21:// $a $b $smaller than?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				int8 addr3=MemoryReadInt8(BlockAddr+3);
				KismetSetRegister(addr3,KismetGetRegister(addr1)<KismetGetRegister(addr2)?1:0);
				BlockAddr+=4;
			}break;
			case 22:// $a $b $larger than?
			{
				int8 addr1=MemoryReadInt8(BlockAddr+1);
				int8 addr2=MemoryReadInt8(BlockAddr+2);
				int8 addr3=MemoryReadInt8(BlockAddr+3);
				KismetSetRegister(addr3,KismetGetRegister(addr1)>KismetGetRegister(addr2)?1:0);
				BlockAddr+=4;
			}break;
		}
	}
}

void KismetExecuteEvent(int16 _DeviceID,int8 _EventID,int16 _EventArgs)
{
	if(!KismetEnabled)return;
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

	//push our arguments into register one and two
	KismetSetRegister(0,_EventArgs);
	KismetExecuteMethod(_DeviceID,methodaddr);
}