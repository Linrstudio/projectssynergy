#include "settings.h"

#ifdef DIGITAL_IN8

#include "EP.h"
#include "UART.h"

extern int8 EPBuffer[16];
extern int8 EPBufferSize;

int8 State[8];
int8 PrevState[8];
int8 ReportState[8];

int8 EPGetType()
{
	return 3;
}

//Init method for the program
void EPInit()
{
	//make everything a input
	TRISC=0xff;
	LATC =0xff;

	State[0]=PrevState[0]=~(PORTCbits.RC7?255:0);
	State[1]=PrevState[1]=~(PORTCbits.RC6?255:0);
	State[2]=PrevState[2]=~(PORTCbits.RC3?255:0);
	State[3]=PrevState[3]=~(PORTCbits.RC4?255:0);
	State[4]=PrevState[4]=~(PORTCbits.RC5?255:0);
	State[5]=PrevState[5]=~(PORTCbits.RC0?255:0);
	State[6]=PrevState[6]=~(PORTCbits.RC1?255:0);
	State[7]=PrevState[7]=~(PORTCbits.RC2?255:0);

	ReportState[0]=0;
	ReportState[1]=0;
	ReportState[2]=0;
	ReportState[3]=0;
	ReportState[4]=0;
	ReportState[5]=0;
	ReportState[6]=0;
	ReportState[7]=0;
}

//main Update method for the program
void EPUpdate()
{
	int8 i;

	//make everything a input
	TRISC=0xff;
	LATC =0xff;

	State[0]=(PORTCbits.RC7?255:0);
	State[1]=(PORTCbits.RC6?255:0);
	State[2]=(PORTCbits.RC3?255:0);
	State[3]=(PORTCbits.RC4?255:0);
	State[4]=(PORTCbits.RC5?255:0);
	State[5]=(PORTCbits.RC0?255:0);
	State[6]=(PORTCbits.RC1?255:0);
	State[7]=(PORTCbits.RC2?255:0);

	for(i=0;i<8;i++)
	{
		if(!ReportState[i] && PrevState[i]!=State[i]){ PrevState[i]=State[i]; ReportState[i]=255; }
	}
}

//when polled by the main station
void EPPolled()
{
	int8 i;
	for(i=0;i<8;i++)
	{
		if(ReportState[i]!=0)
		{
			EPBuffer[0]=8+i+i+(PrevState[i]?0:1);//calculate event idx
			PORTBbits.RB4=(PrevState[i]?0:1);
			EPBufferSize=1;
			ReportState[i]=0;
			return;
		}
	}
}

//request to invoke event
//returns wether the event was successfully invoked
int8 EPInvokeEvent(int8 _Event,int8* _Args)
{
	if(_Event>=8&&_Event<=15)
	{
		EPBuffer[0]=State[_Event-8];
		EPBuffer[1]=State[_Event-8];
		EPBufferSize=2;
		return 255;
	}
	return 0;
}
#endif
//EOF
void boreme();