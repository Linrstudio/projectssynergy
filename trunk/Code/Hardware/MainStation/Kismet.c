#include "Kismet.h"
#include "RTC.h"
#include "EEPROM.h"
#include "UART.h"
#include "MainStation.h"
#include "USB.h"

#if 1
#define Reg KismetRegisters
extern int16*KismetRegisters;
#else
int16 Reg[16];
#endif

int8 KismetEnabled;

void KismetInit()
{
	KismetEnabled=1;
}

void KismetSetRegister(int8 _Register,int16 _Value)
{
	Reg[_Register]=_Value;
}

int16 KismetGetRegister(int8 _Register)
{
	return Reg[_Register];
}
/*
void KismetExecuteMethod(int16 _Deviceid,int16 _MethodAddr)
{
	int16 BlockAddr=_MethodAddr;
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
				SetLED1(1);
				return;
			}
			case 0x01://assign int16data $reg #value
			{
				int8 reg	=MemoryReadInt8();
				int16 value	=MemoryReadInt16();
				Reg[reg]=value;
				BlockAddr+=4;
				//SetLED1(1);
			}break;
			case 0x0A:
			{
				int8 reg	=MemoryReadInt8();
				SetLED1(Reg[reg]);
				BlockAddr+=2;
			}break;
			case 4://not implemented
			{
				return;
			}break;
			case 5:// $a $b $equal?
			{
				int8 addr1=MemoryReadInt8();
				int8 addr2=MemoryReadInt8();
				int8 addr3=MemoryReadInt8();
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
			case 100://Add $a $b $a*b?
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
				//KismetVariables[varaddr]=KismetGetRegister(valreg);
				BlockAddr+=3;
			}break;
			case 17://GetVariable #varaddr $reg
			{
				int8 varaddr=MemoryReadInt8(BlockAddr+1);
				int8 valreg =MemoryReadInt8(BlockAddr+2);
				//KismetSetRegister(valreg,KismetVariables[varaddr]);
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
*/

int8 KismetExecuteEvent(int16 _DeviceID,int8 _EventID)
{
	int8  read8;
	int16 read16;
	int16 eventaddr;
	int16 methodaddr;
	int16 BlockAddr;
	if(!KismetEnabled)return 1;

	ToSendDataBuffer[2]=((int8*)&_DeviceID)[0];
	ToSendDataBuffer[3]=((int8*)&_DeviceID)[1];
	ToSendDataBuffer[4]=_EventID;

	MemoryBeginRead(0);
	while(1)
	{
		read16=MemoryReadInt16();
		eventaddr =MemoryReadInt16();
		if(read16==_DeviceID)break;
		if(read16==0xffff) { MemoryEndRead(); return 2; }
	}
	MemoryEndRead(); 
	MemoryBeginRead(eventaddr);
	while(1)
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
			case 0x01://assign int16data $reg #value
			{
				int8 reg	=MemoryReadInt8();
				int16 value	=MemoryReadInt16();
				Reg[reg]=value;
			}break;
			case 0x0B://equal
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg[reg3]=(Reg[reg1]==Reg[reg2])?0xffff:0;
			}break;
			case 0x0C://differ
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg[reg3]=(Reg[reg1]==Reg[reg2])?0:0xffff;
			}break;
			case 0x0D://and
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg[reg3]=Reg[reg1]&Reg[reg2];
			}break;
			case 0x0E://or
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg[reg3]=Reg[reg1]|Reg[reg2];
			}break;
			case 0x0F://xor
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg[reg3]=Reg[reg1]^Reg[reg2];
			}break;
			case 0x20://add
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg[reg3]=Reg[reg1]+Reg[reg2];
			}break;
			case 0x21://sub
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg[reg3]=Reg[reg1]-Reg[reg2];
			}break;
			case 0x22://mul
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg[reg3]=Reg[reg1]*Reg[reg2];
			}break;
			case 0x23://div
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				int8 reg3	=MemoryReadInt8();
				Reg[reg3]=Reg[reg1]/Reg[reg2];
			}break;
			case 0x0A:
			{
				int8 reg	=MemoryReadInt8();
				SetLED1(Reg[reg]);
			}break;
			case 0x80:// $if goto $here?
			{
				int8 reg1	=MemoryReadInt8();
				int8 reg2	=MemoryReadInt8();
				if(Reg[reg1]==0)
				{
					BlockAddr=methodaddr+(int16)reg2;
					MemoryEndRead();MemoryBeginRead(BlockAddr);
				}
			}break;
		}
	}
	return 4;
}