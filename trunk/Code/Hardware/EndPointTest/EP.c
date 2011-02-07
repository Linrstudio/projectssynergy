#include "EP.h"
#include "UART.h"

extern int8 EPBuffer[16];
extern int8 EPBufferSize;

#define DEVICE_RELAY8
#define DEVICE_DIGITALIN8

#ifdef DEVICE_DIGITALIN8
int8 PrevState[8];
int8 ReportState[8];
#endif
//Init method for the program
void EPInit()
{

}

//main Update method for the program
void EPUpdate()
{

}

//when polled by the main station
void EPPolled()
{
#ifdef DEVICE_DIGITALIN8
	int8 i;
	for(i=0;i<8;i++)
	{
		if(ReportState[i])
		{
			
			//PrevState[i]=state[i];
		}
	}


#endif
}

//request to invoke event
//returns wether the event was successfully invoked
int8 EPInvokeEvent(int8 _Event,int8* _Args)
{
#ifdef DEVICE_RELAY8
	if(_Event==1)
	{
		if(_Args[0])PORTCbits.RC4=0;else PORTCbits.RC4=1;
	}
	if(_Event==2)
	{
		if(_Args[0])PORTCbits.RC5=0;else PORTCbits.RC5=1;
	}
	if(_Event==3)
	{
		if(_Args[0])PORTCbits.RC3=0;else PORTCbits.RC3=1;
	}
	if(_Event==4)
	{
		if(_Args[0])PORTCbits.RC6=0;else PORTCbits.RC6=1;
	}
	if(_Event==5)
	{
		if(_Args[0])PORTCbits.RC7=0;else PORTCbits.RC7=1;
	}
	if(_Event==6)
	{
		if(_Args[0])PORTCbits.RC0=0;else PORTCbits.RC0=1;
	}
	if(_Event==7)
	{
		if(_Args[0])PORTCbits.RC1=0;else PORTCbits.RC1=1;
	}
	if(_Event==8)
	{
		if(_Args[0])PORTCbits.RC2=0;else PORTCbits.RC2=1;
	}

	if(_Event==9)
	{
		PORTCbits.RC4=!PORTCbits.RC4;
	}
	if(_Event==10)
	{
		PORTCbits.RC5=!PORTCbits.RC5;
	}
	if(_Event==11)
	{
		PORTCbits.RC3=!PORTCbits.RC3;
	}
	if(_Event==12)
	{
		PORTCbits.RC6=!PORTCbits.RC6;
	}
	if(_Event==13)
	{
		PORTCbits.RC7=!PORTCbits.RC7;
	}
	if(_Event==14)
	{
		PORTCbits.RC0=!PORTCbits.RC0;
	}
	if(_Event==15)
	{
		PORTCbits.RC1=!PORTCbits.RC1;
	}
	if(_Event==16)
	{
		PORTCbits.RC2=!PORTCbits.RC2;
	}
	return 255;
#endif
#ifdef DEVICE_DIGITALIN8
	
#endif
}
