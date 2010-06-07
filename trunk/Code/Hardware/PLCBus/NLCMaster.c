#include "NLCMaster.h"

void NLCInit()
{

}

void NLCDelay()
{
	for(int i=0;i<32;i++);
}

void NLCWriteByte(int8 _Data)
{
	for(int i=0;i<8;i++)
	{
		DAT=(_Data>>7)?1:0;
		NLCDelay();	CLK=1;	NLCDelay();
		DAT=0;
		NLCDelay();	CLK=0;	NLCDelay();
		_Data<<=1;
	}
	DAT=0;
}

int8 NLCReadByte()
{
#asm
	BSF _TRIS, DATBIT
#endasm
	int8 i=128;
	int8 dat=0;
	do
	{
		NLCDelay();CLK=1;NLCDelay();
		if(DAT)dat|=i;
		NLCDelay();CLK=0;NLCDelay();
		i>>=1;
	}while(i!=0);
#asm
	BCF _TRIS, DATBIT
#endasm
	return dat;
}

void NLCWriteStart()
{
	NLCDelay();	CLK=1;	NLCDelay();
	DAT=1;
	NLCDelay();	CLK=0;	NLCDelay();
	DAT=0;
}
