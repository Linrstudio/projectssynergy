#include "EP.h"
#include "UART.h"

extern int8 EPBuffer[16];
extern int8 EPBufferSize;

//Init method for the program
void Init()
{
	TRISC0=1;
	ANS4=1;
	TRISC1=1;
	TRISC7=0;
	RC0=0;
}

int8 GetAnalog(int8 _Port)
{
	ADON=1;
	ADFM=0;
	CHS0=((_Port>>0)&1)?1:0;
	CHS1=((_Port>>1)&1)?1:0;
	CHS2=((_Port>>2)&1)?1:0;
	CHS3=((_Port>>3)&1)?1:0;
	ADCON0|=2;
	while((ADCON0&2)?1:0);
	return ADRESH;
}

//main Tick method for the program
void Tick()
{
	int8 value = GetAnalog(4);
	//RB4=!RB4;
	RB4=0;
	for(int i=0;i<value;i++);
	RB4=1;
	value=255-value;
	for(int j=0;j<value;j++);
}

int8 laststate=0;
//when polled by the main station
void Polled()
{
	if(laststate!=RC1)
	{
		laststate=RC1;
		if(laststate)
		{
			EPBuffer[0]=1;
			EPBufferSize=1;
		}else{
			EPBuffer[0]=2;
			EPBufferSize=1;
		}
	}
	//RC7=!RC7;
}

//request to invoke event
//returns wether the event was successfully invoked
int8 InvokeEvent(int8 _Event,int16 _Args)
{
	switch(_Event)
	{
		case 1:
		RC7=(_Args&1)?1:0;
		break;
	}
}
